using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace JP.DataHub.Api.Core.Authentication
{
    public class CreateJwtSecurityTokenCommand
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? Expires { get; set; }
        public string AudienceSecret { get; set; }

        public CreateJwtSecurityTokenCommand(string issuer, string audience, IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null, string audienceSecret = null)
        {
            this.Issuer = issuer;
            this.Audience = audience;
            this.Claims = claims;
            this.NotBefore = notBefore;
            this.Expires = expires;
            this.AudienceSecret = audienceSecret;
        }

        public string CreateJwtSecurityToken()
        {
            //共通鍵となっているが問題ないのか？公開鍵と秘密鍵にしないとクライアント側でJWTの検証ができないのではないのか?
            // 署名用のキーを作成
            var signingKey = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(AudienceSecret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(Issuer, Audience, Claims, NotBefore, Expires, signingCredentials));
        }
    }
}
