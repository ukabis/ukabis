using JP.DataHub.AdminWeb.Authentication;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.MVC.Authentication;
using JP.DataHub.MVC.Filters;
using JP.DataHub.MVC.Http;
using JP.DataHub.MVC.Session;
using JP.DataHub.MVC.Unity;
using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NLog.Config;
using Radzen;
using Unity;
using Unity.Microsoft.DependencyInjection;
using Newtonsoft.Json;
using JP.DataHub.AdminWeb.Core.Authentication;
using JP.DataHub.AdminWeb.Core.Misc;
using JP.DataHub.AdminWeb.Core.Settings;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Oci.Common;
using Oci.CoreService.Models;
using Microsoft.IdentityModel.Logging;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Protocols;
using JP.DataHub.Com.DataContainer;
using System.Net.Http;
using NLog;
using NLog.Web;

// Nlog
//var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var logger = LogManager.Setup().LoadConfigurationFromXml("nlog.config").GetCurrentClassLogger();
logger.Debug("init main");

IServiceProvider services = null;
var builder = WebApplication.CreateBuilder(args);

// Nlog
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Host.UseNLog();

builder.Services.Configure<List<AdditionalHttpResponseHeader>>(builder.Configuration.GetSection("HttpResponseHeader"));
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// ヘルスチェック
builder.Services.AddHealthChecks();

// ENVIRONMENTがDevelopment以外の場合、blazorがなくなってしまい、画面のレイアウトが崩れる為、追加設定(blazorの仕様)
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseStaticWebAssets();
}

// OpenID Connect設定
IdentityModelEventSource.ShowPII = true;
var adb2cSection = builder.Configuration.GetSection("IdentityDomains");
//var oidcOptions = builder.Configuration.GetSection("IdentityDomains").Get<OidcSettings>();
builder.Services.Configure<OpenIdConnectOptions>(adb2cSection);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Events.OnValidatePrincipal = async (context) =>
        {
            var tokenAcquisition = services?.GetService<ITokenAcquisition>() as IdcsTokenAcquisition;
            if (tokenAcquisition != null)
            {
                var token = context.Properties.GetTokenValue("access_token");
                tokenAcquisition.AccessToken = token;
                //if (バリデーション)
                //{
                //    context.RejectPrincipal();
                //    await context.HttpContext.SignOutAsync();
                //}
            }
            //return Task.CompletedTask;
        };
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        var oidcOptions = builder.Configuration.GetSection("IdentityDomains").Get<OidcSettings>();
        // metadata URLを設定することで自動的に読み込んでくれる（公開鍵のローテーションにも対応しているらしい）
        options.MetadataAddress = oidcOptions.MetadataAddress;
        options.ClientId = oidcOptions.ClientId;
        options.ClientSecret = oidcOptions.ClientSecret;

        options.ResponseType = OpenIdConnectResponseType.CodeToken;
        options.SignInScheme = "Cookies";
        options.UseTokenLifetime = true;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Add(oidcOptions.Scope);

        options.Events.OnTokenResponseReceived = (context) =>
        {
            var tokenAcquisition = services?.GetService<ITokenAcquisition>() as IdcsTokenAcquisition;
            if (tokenAcquisition != null)
            {
                tokenAcquisition.AccessToken = context.ProtocolMessage.AccessToken;
            }
            return Task.CompletedTask;
        };
    });

// サインアウト時にルートにリダイレクトするのに必要
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnSignedOutCallbackRedirect += context =>
    {
        context.HttpContext.Response.Redirect(context.Options.SignedOutRedirectUri);
        context.HandleResponse();

        return Task.CompletedTask;
    };
});

// セッション設定
builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "JP.DataHub.AdminWeb";
});

//if (builder.Configuration.GetSection("Profiler").GetValue<bool>("UseProfiler"))
//{
//    builder.Services.AddMiniProfiler();
//}

// 権限設定
builder.Services.AddAuthorization(options =>
{
    foreach (FunctionPolicy policy in Enum.GetValues(typeof(FunctionPolicy)))
    {
        if (policy == FunctionPolicy.SystemAdministrator)
        {
            options.AddPolicy(policy.ToString(), builder =>
            {
                builder.RequireClaim(AdminClaimTypes.IsSystemAdministrator, "true");
            });
        }
        else if (policy.ToString().Contains("Write"))
        {
            // Write権限がある場合のポリシー追加
            options.AddPolicy(policy.ToString(), builder =>
            {
                builder.RequireClaim(AdminFunc.GetAdminFunc(policy), "Write");
            });
        }
        else
        {
            // ReadまたはWrite権限がある場合のポリシー追加
            options.AddPolicy(policy.ToString(), builder =>
            {
                builder.RequireClaim(AdminFunc.GetAdminFunc(policy), "Read", "Write");
            });
        }
    }
});
builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, AddRolesClaimsTransformation>();

// Add services to the container.
builder.Services.AddRazorPages()
                .AddMicrosoftIdentityUI()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(typeof(SetOAuthInfoFilter));
                });
builder.Services.AddServerSideBlazor();

// Radzen設定
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// SignalRのサイズ上限(デフォルト32KB)によりSessionStorageでエラーになるため、暫定対応として上限を1MBに変更
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 1024 * 1024;
});

//キャッシュ設定
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCaching(builder.Configuration);

// ApplicationInsights設定
var instrumentationKey = builder.Configuration.GetSection("ApplicationInsights")["InstrumentationKey"];
if (!string.IsNullOrEmpty(instrumentationKey))
{
    // アプリケーションログ設定
    var target = new ApplicationInsightsTarget();
    target.InstrumentationKey = instrumentationKey;

    var rule = new LoggingRule("*", NLog.LogLevel.FromString(builder.Configuration.GetSection("ApplicationInsights")["LogLevel"]), target);
    var config = new LoggingConfiguration();
    config.LoggingRules.Add(rule);

    NLog.LogManager.Configuration = config;

    // テレメトリ設定
    builder.Services.AddApplicationInsightsTelemetry();
}

// DI設定
builder.Host.UseUnityServiceProvider();
builder.Host.ConfigureContainer<IUnityContainer>(container =>
{
    UnityCore.DefaultLifetimeManager = UnityCore.DataContainerLifetimeManager = new PerRequestLifetimeManager();
    UnityCore.Buildup(container, "UnityBuildup.json", builder.Configuration);
    container.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>();
    container.RegisterType<IOAuthContext, OAuthContext>(new PerRequestLifetimeManager());
    container.RegisterType<IGroupSessionManager, GroupSessionManager>(new PerRequestLifetimeManager());
    container.RegisterType<IAuthorizationSessionManager, AuthorizationSessionManager>(new PerRequestLifetimeManager());

    // Cacheのタイムスパンを全体で利用するため
    CacheServiceCollectionExtensions.TimeSpanDictionary.ToList().ForEach(x => container.RegisterInstance<TimeSpan>(x.Key, x.Value));

    // Configuration
    //container.RegisterInstance(builder.Configuration.GetSection(AzureAdB2CSettings.SECTION_NAME).Get<AzureAdB2CSettings>());
    container.RegisterInstance(builder.Configuration.GetSection("IdentityDomains").Get<OidcSettings>());
    container.RegisterInstance(builder.Configuration.GetSection("IdentityDomains").Get<AzureAdB2CSettings>());
    container.RegisterInstance(builder.Configuration.GetSection(ServerSettings.SECTION_NAME).Get<ServerSettings>());
    container.RegisterInstance(builder.Configuration.GetSection(SystemAdminSettings.SECTION_NAME).Get<SystemAdminSettings>());
    container.RegisterInstance(builder.Configuration.GetSection(DefaultRoleSettings.SECTION_NAME).Get<DefaultRoleSettings>());
    container.RegisterInstance(builder.Configuration.GetSection(VendorDataUseAndOfferSettings.SECTION_NAME).Get<VendorDataUseAndOfferSettings>());
    container.RegisterInstance(builder.Configuration.GetSection(ErrorHandlingSettings.SECTION_NAME).Get<ErrorHandlingSettings>());
    container.RegisterInstance<ITokenAcquisition>(new IdcsTokenAcquisition());

    // smartfoodchainの環境向き先、認証（ベンダー）の情報取得、認証を得る処理。なおそれぞれをContainerに登録する（どこでも利用できるようにするため）
    // 接続先が単一の場合は、この方が便利。Resolveすればいつでも取れるため
    var env = builder.Configuration.GetSection("Server").Get<ServerSettings>().Environment;
    var hostenv = container.Resolve<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
    var serverbuilder = new ConfigurationBuilder()
        .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile(path: "server.json")
        .AddJsonFile($"server.{hostenv.EnvironmentName}.json", true)
        .Build();
    var serverlist = serverbuilder.FindFile().FileToJson<ServerList>();

    IServerEnvironment environment = null;
    if (serverlist.IsOnce())
    {
        environment = serverlist.FindOnce();
    }
    else
    {
        var settings = container.Resolve<ServerSettings>();
        environment = serverlist.Find(settings.Server, settings.Environment);
    }
    if (environment == null)
    {
        throw new System.Exception("基盤の環境を指定してください");
    }
    container.RegisterInstance<IServerEnvironment>(environment);
    var authenticationService = AuthenticationServiceFactory.Create(environment.Parent.AuthenticationType);
    container.RegisterInstance<JP.DataHub.Com.Web.Authentication.IAuthenticationService>(authenticationService);

    // システム共通でアクセスするためのもの（認証情報と認証トークン）※これがデフォルト
    var authenticationInfo = AuthenticationInfoFactory.Create(environment);
    container.RegisterInstance<IAuthenticationInfo>(authenticationInfo);
    var authenticationResult = authenticationService.Authentication(environment, authenticationInfo);
    container.RegisterInstance<IAuthenticationResult>(authenticationResult);

    // ログインユーザーでアクセスするためのDynamicApiClient
    var loginedDynamicApiClient = new DynamicApiClient(environment, authenticationResult);
    UnityCore.RegisterInstance<IDynamicApiClient>(CommonDynamicApiConst.LoginUserKey, loginedDynamicApiClient);

    // 1. ログインしたOpenId用の認証セット（Vendor認証+OpenId認証）
    // 2. OpenIdがシステム呼び出し用として利用する認証セット（Vendor認証+OpenId認証）
    // API呼び出し時にこれらを使い分ける必要がある

    var accountManager = UnityCore.Resolve<IAuthenticationManager>(builder.Configuration.GetValue<string>("AccountFileName").ToCI());
    var commonAuthenticationInfo = accountManager.Find(builder.Configuration.GetValue<string>("Account"));
    if (commonAuthenticationInfo == null)
    {
        throw new System.Exception("Accountで指定した情報がみつかりません");
    }
    UnityCore.RegisterInstance<IAuthenticationInfo>(CommonDynamicApiConst.CommonKey, commonAuthenticationInfo);
    var commonAuthenticationResult = authenticationService.Authentication(environment, commonAuthenticationInfo);
    container.RegisterInstance<IAuthenticationResult>(CommonDynamicApiConst.CommonKey, commonAuthenticationResult);

    // DynamicAPIの共通API取得用
    var client = new DynamicApiClient(environment, commonAuthenticationResult);
    UnityCore.RegisterInstance<IDynamicApiClient>(CommonDynamicApiConst.CommonKey, client);
});

var app = builder.Build();
services = app.Services;
app.UseUnityMiddleware();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//Azureや他サービスだとSSL終端するかんけいでhttpsにリダイレクトされると不都合があるので
//ローカルのみ、https使う
//if (app.Environment.IsDevelopment())
//{
app.UseHttpsRedirection();
//}

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions()
    {
        OnPrepareResponse = context =>
        {
            context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            context.Context.Response.Headers.Add("Pragma", "no-cache");
            context.Context.Response.Headers.Add("Expires", "-1");
        }
    });
}

app.UseStaticFiles();
app.UseRouting();
//app.UseMiniProfiler();

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(options =>
{
    //サポートするカルチャの設定
    string[] supportedCultures = new string[] { "ja", "en" };

    options
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
        .SetDefaultCulture(supportedCultures[0])
        ;
});

app.UseSession();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// ヘルスチェック
app.MapHealthChecks("/healthz");

app.Run();
