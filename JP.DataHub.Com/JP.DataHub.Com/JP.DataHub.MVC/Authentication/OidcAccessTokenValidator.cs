using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.MVC.Authentication
{
    public class OidcAccessTokenValidator
    {
        private Dictionary<string, OidcParameter> _settings;

        private static Lazy<IConfiguration> _lazyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        protected static IConfiguration Configuration { get => _lazyConfiguration.Value; }

        /// <summary>
        ///  OpenId Connectのアクセストークン検証設定
        /// </summary>
        private class OidcParameter
        {
            public OidcAccessTokenValidatorSetting ValidatorSetting { get; set; }
            public OpenIdConnectConfiguration OidcConfig { get; set; }
            public OpenIdConnectConfiguration OidcConfigForSwitching { get; set; }
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="settings">Validatorの設定</param>
        public OidcAccessTokenValidator(IEnumerable<OidcAccessTokenValidatorSetting> settings)
        {
            _settings = new Dictionary<string, OidcParameter>();

            foreach (var setting in settings)
            {
                _settings.Add(setting.Audience, new OidcParameter { ValidatorSetting = setting });
            }
        }

        public static OidcAccessTokenValidator CreateFromConfig()
        {
            var configSection = Configuration.GetSection("OpenId");
            var settings = new List<OidcAccessTokenValidatorSetting>();

            // OpenId認証設定
            var aadEndPoint = string.Format(configSection.GetValue<string>("AadEndPoint"), configSection.GetValue<string>("Tenant"));
            var endpointUb = new UriBuilder(aadEndPoint);
            var setting = new OidcAccessTokenValidatorSetting
            {
                TenantUrl = endpointUb.ToString(),
                Audience = configSection.GetValue<string>("WebApiClientId"),
                Policy = configSection.GetValue<string>("SignUpInPolicyId")
            };

            // endpoint切替用設定
            var endpointForSwitching = configSection.GetValue<string>("AadEndpointForSwitching");
            if (!string.IsNullOrEmpty(endpointForSwitching))
            {
                var aadEndPointForSwitching = string.Format(configSection.GetValue<string>("AadEndpointForSwitching"), configSection.GetValue<string>("Tenant"));
                var endpointForSwitchingUb = new UriBuilder(aadEndPointForSwitching);
                setting.TenantUrlForSwitching = endpointForSwitchingUb.ToString();
            }

            settings.Add(setting);

            // 追加の設定
            var authentications = configSection.GetSection("OAuthBearerAuthentications").Get<IEnumerable<OidcAccessTokenValidatorSetting>>();
            if (authentications is not null)
            {
                settings.AddRange(authentications);
            }

            return new OidcAccessTokenValidator(settings);
        }

        /// <summary>
        /// OpenId Connectのアクセストークンを検証します。
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ClaimsPrincipal</returns>
        public ClaimsPrincipal Validate(string accessToken)
        {
            var audiences = new JwtSecurityToken(accessToken).Audiences;
            if (!audiences.Any())
            {
                throw new SecurityTokenException("aud is missing.");
            }
            string audience = null;
            OidcParameter setting = null;
            foreach (var currentValue in audiences)
            {
                if (string.IsNullOrEmpty(currentValue))
                {
                    throw new SecurityTokenException("aud is missing.");
                }
                if (_settings.TryGetValue(currentValue, out OidcParameter _setting))
                {
                    audience = currentValue;
                    setting = _setting;
                    break;
                }
            }
            if (setting == null)
            {
                throw new SecurityTokenException("aud is invalid.");
            }

            var securityKeys = new List<SecurityKey>();
            var validIssuers = new List<string>();

            var endpoint = $"{setting.ValidatorSetting.TenantUrl}";
            var taskOidcConfig = GetOpenIdConnectConfiguration(endpoint);

            Task<OpenIdConnectConfiguration> taskOidcConfigForSwitching = null;
            if (!string.IsNullOrEmpty(setting.ValidatorSetting.TenantUrlForSwitching))
            {
                var endpointForSwitching = $"{setting.ValidatorSetting.TenantUrlForSwitching}";
                taskOidcConfigForSwitching = GetOpenIdConnectConfiguration(endpointForSwitching);
            }

            Task.WaitAll();

            setting.OidcConfig = taskOidcConfig.Result;
            validIssuers.Add(setting.OidcConfig.Issuer);
            securityKeys.AddRange(setting.OidcConfig.SigningKeys);

            if (taskOidcConfigForSwitching != null)
            {
                setting.OidcConfigForSwitching = taskOidcConfigForSwitching.Result;
                validIssuers.Add(setting.OidcConfigForSwitching.Issuer);
                securityKeys.AddRange(setting.OidcConfigForSwitching.SigningKeys);
            }

            var validationParams = new TokenValidationParameters
            {
                ValidIssuers = validIssuers, // テナントIDs
                ValidAudiences = new[] { setting.ValidatorSetting.Audience }, // ApiWebのアプリケーションID
                IssuerSigningKeys = securityKeys,
                NameClaimType = "name"
            };

            // 検証実行
            return new JwtSecurityTokenHandler().ValidateToken(accessToken, validationParams, out _);
        }

        /// <summary>
        /// OpenId認証の検証に必要な情報を取得します
        /// </summary>
        /// <returns></returns>
        private Task<OpenIdConnectConfiguration> GetOpenIdConnectConfiguration(string endpoint)
        {
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(endpoint, new OpenIdConnectConfigurationRetriever());
            return configManager.GetConfigurationAsync();
        }
    }
}