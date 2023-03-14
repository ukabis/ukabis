using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record CreateJwtSecurityTokenCommand : IValueObject
    {
        public string Issuer { get; }
        public string Audience { get; }
        public IEnumerable<Claim> Claims { get; }
        public DateTime? NotBefore { get; }
        public DateTime? Expires { get; }
        public string AudienceSecret { get; }

        public CreateJwtSecurityTokenCommand(string issuer, string audience, IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null, string audienceSecret = null)
        {
            this.Issuer = issuer;
            this.Audience = audience;
            this.Claims = claims;
            this.NotBefore = notBefore;
            this.Expires = expires;
            this.AudienceSecret = audienceSecret;
        }

        public JwtSecurityTokenValue CreateJwtSecurityToken()
        {
            //共通鍵となっているが問題ないのか？公開鍵と秘密鍵にしないとクライアント側でJWTの検証ができないのではないのか?
            // 署名用のキーを作成
            var signingKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(AudienceSecret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            return new JwtSecurityTokenValue(new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(Issuer, Audience, Claims,
                NotBefore, Expires, signingCredentials)));
        }

        public static bool operator ==(CreateJwtSecurityTokenCommand me, object other) => me?.Equals(other) == true;

        public static bool operator !=(CreateJwtSecurityTokenCommand me, object other) => !me?.Equals(other) == true;
    }
}
