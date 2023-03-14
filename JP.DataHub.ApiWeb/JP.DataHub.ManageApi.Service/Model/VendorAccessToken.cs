using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class VendorAccessToken
    {
        private JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();
        private TokenValidationParameters _validationParameters;

        /// <summary>アクセストークン</summary>
        public string Value { get; }

        /// <summary>
        /// クラスを初期化します。
        /// </summary>
        /// <param name="value">アクセストークン</param>
        public VendorAccessToken(string value)
        {
            Value = value;

            var key = UnityCore.Resolve<VendorAuthenticationJwtKeyConfig>();

            // 署名検証用のキーを作成
            // TDO
            var signingKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(key.AudienceSecret));

            // 検証パラメータを作成
            _validationParameters = new TokenValidationParameters
            {
                ValidIssuer = key.Issuer,
                ValidAudience = key.Audience,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.FromSeconds(1)
            };
        }

        /// <summary>
        /// ベンダー/システム認証のアクセストークンを検証します。
        /// </summary>
        /// <returns>ClaimsPrincipal</returns>
        public ClaimsPrincipal Validate()
            => _handler.ValidateToken(Value, _validationParameters, out SecurityToken validatedToken);
    }
}
