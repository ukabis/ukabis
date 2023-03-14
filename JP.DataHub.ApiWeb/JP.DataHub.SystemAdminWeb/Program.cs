using JP.DataHub.AdminWeb.Authentication;
using JP.DataHub.AdminWeb.Core.Authentication;
using JP.DataHub.AdminWeb.Core.Misc;
using JP.DataHub.AdminWeb.Core.Settings;
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
using JP.DataHub.SystemAdminWeb.Authetication;
using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NLog.Config;
using Oci.Common;
using Oci.CoreService.Models;
using Radzen;
using System.Text;
using Unity;
using Unity.Microsoft.DependencyInjection;

IServiceProvider services = null;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<List<AdditionalHttpResponseHeader>>(builder.Configuration.GetSection("HttpResponseHeader"));
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// ENVIRONMENT��Development�ȊO�̏ꍇ�Ablazor���Ȃ��Ȃ��Ă��܂��A��ʂ̃��C�A�E�g�������ׁA�ǉ��ݒ�(blazor�̎d�l)
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseStaticWebAssets();
}

// OpenID Connect�ݒ�
IdentityModelEventSource.ShowPII = true;
var adb2cSection = builder.Configuration.GetSection("IdentityDomains");
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
            }
        };
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        var oidcOptions = builder.Configuration.GetSection("IdentityDomains").Get<OidcSettings>();
        // metadata URL��ݒ肷�邱�ƂŎ����I�ɓǂݍ���ł����i���J���̃��[�e�[�V�����ɂ��Ή����Ă���炵���j
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
// �T�C���A�E�g���Ƀ��[�g�Ƀ��_�C���N�g����̂ɕK�v
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnSignedOutCallbackRedirect += context =>
    {
        context.HttpContext.Response.Redirect(context.Options.SignedOutRedirectUri);
        context.HandleResponse();

        return Task.CompletedTask;
    };
});

// �Z�b�V�����ݒ�
builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "JP.DataHub.SystemAdminWeb";
});

//if (builder.Configuration.GetSection("Profiler").GetValue<bool>("UseProfiler"))
//{
//    builder.Services.AddMiniProfiler();
//}

// �����ݒ�
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
            // Write����������ꍇ�̃|���V�[�ǉ�
            options.AddPolicy(policy.ToString(), builder =>
            {
                builder.RequireClaim(AdminFunc.GetAdminFunc(policy), "Write");
            });
        }
        else
        {
            // Read�܂���Write����������ꍇ�̃|���V�[�ǉ�
            options.AddPolicy(policy.ToString(), builder =>
            {
                builder.RequireClaim(AdminFunc.GetAdminFunc(policy), "Read", "Write");
            });
        }
    }
});
builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, AddSystemAdminRolesClaimsTransformation>();

// Add services to the container.
builder.Services.AddRazorPages()
                .AddMicrosoftIdentityUI()
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(typeof(SetOAuthInfoFilter));
                });
builder.Services.AddServerSideBlazor();

// Radzen�ݒ�
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// SignalR�̃T�C�Y���(�f�t�H���g32KB)�ɂ��SessionStorage�ŃG���[�ɂȂ邽�߁A�b��Ή��Ƃ��ď����1MB�ɕύX
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 1024 * 1024;
});

//�L���b�V���ݒ�
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCaching(builder.Configuration);

// ApplicationInsights�ݒ�
var instrumentationKey = builder.Configuration.GetSection("ApplicationInsights")["InstrumentationKey"];
if (!string.IsNullOrEmpty(instrumentationKey))
{
    // �A�v���P�[�V�������O�ݒ�
    var target = new ApplicationInsightsTarget();
    target.InstrumentationKey = instrumentationKey;

    var rule = new LoggingRule("*", NLog.LogLevel.FromString(builder.Configuration.GetSection("ApplicationInsights")["LogLevel"]), target);
    var config = new LoggingConfiguration();
    config.LoggingRules.Add(rule);

    NLog.LogManager.Configuration = config;

    // �e�����g���ݒ�
    builder.Services.AddApplicationInsightsTelemetry();
}

// DI�ݒ�
builder.Host.UseUnityServiceProvider();
builder.Host.ConfigureContainer<IUnityContainer>(container =>
{
    UnityCore.DefaultLifetimeManager = UnityCore.DataContainerLifetimeManager = new PerRequestLifetimeManager();
    UnityCore.Buildup(container, "UnityBuildup.json", builder.Configuration);
    container.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>();
    container.RegisterType<IOAuthContext, OAuthContext>(new PerRequestLifetimeManager());
    container.RegisterType<IGroupSessionManager, GroupSessionManager>(new PerRequestLifetimeManager());
    container.RegisterType<IAuthorizationSessionManager, AuthorizationSessionManager>(new PerRequestLifetimeManager());

    // Cache�̃^�C���X�p����S�̂ŗ��p���邽��
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

    // ��������A�F�؁i�x���_�[�j�̏��擾�A�F�؂𓾂鏈���B�Ȃ����ꂼ���Container�ɓo�^����i�ǂ��ł����p�ł���悤�ɂ��邽�߁j
    // �ڑ��悪�P��̏ꍇ�́A���̕����֗��BResolve����΂��ł����邽��
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
        throw new System.Exception("��Ղ̊����w�肵�Ă�������");
    }
    container.RegisterInstance<IServerEnvironment>(environment);
    var authenticationService = AuthenticationServiceFactory.Create(environment.Parent.AuthenticationType);
    container.RegisterInstance<JP.DataHub.Com.Web.Authentication.IAuthenticationService>(authenticationService);

    // �V�X�e�����ʂŃA�N�Z�X���邽�߂̂��́i�F�؏��ƔF�؃g�[�N���j�����ꂪ�f�t�H���g
    var authenticationInfo = AuthenticationInfoFactory.Create(environment);
    container.RegisterInstance<IAuthenticationInfo>(authenticationInfo);
    var authenticationResult = authenticationService.Authentication(environment, authenticationInfo);
    container.RegisterInstance<IAuthenticationResult>(authenticationResult);

    // ���O�C�����[�U�[�ŃA�N�Z�X���邽�߂�DynamicApiClient
    var loginedDynamicApiClient = new DynamicApiClient(environment, authenticationResult);
    UnityCore.RegisterInstance<IDynamicApiClient>(CommonDynamicApiConst.LoginUserKey, loginedDynamicApiClient);

    // 1. ���O�C������OpenId�p�̔F�؃Z�b�g�iVendor�F��+OpenId�F�؁j
    // 2. OpenId���V�X�e���Ăяo���p�Ƃ��ė��p����F�؃Z�b�g�iVendor�F��+OpenId�F�؁j
    // API�Ăяo�����ɂ������g��������K�v������

    var accountManager = UnityCore.Resolve<IAuthenticationManager>(builder.Configuration.GetValue<string>("AccountFileName").ToCI());
    var commonAuthenticationInfo = accountManager.Find(builder.Configuration.GetValue<string>("Account"));
    if (commonAuthenticationInfo == null)
    {
        throw new System.Exception("Account�Ŏw�肵����񂪂݂���܂���");
    }
    UnityCore.RegisterInstance<IAuthenticationInfo>(CommonDynamicApiConst.CommonKey, commonAuthenticationInfo);
    var commonAuthenticationResult = authenticationService.Authentication(environment, commonAuthenticationInfo);
    container.RegisterInstance<IAuthenticationResult>(CommonDynamicApiConst.CommonKey, commonAuthenticationResult);

    // DynamicAPI�̋���API�擾�p
    var client = new DynamicApiClient(environment, commonAuthenticationResult);
    UnityCore.RegisterInstance<IDynamicApiClient>(CommonDynamicApiConst.CommonKey, client);
});

var app = builder.Build();
services = app.Services;
app.UseUnityMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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
app.UseSession();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
