using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace TasksApi.Helpers
{
    public class TokenHelper
    {
        public const string Issuer = "http://codingsonata.com";
        public const string Audience = "http://codingsonata.com";
        public const string Secret = "p0GXO6VuVZLRPef0tyO9jCqK4uZufDa6LP4n8Gj+8hQPB30f94pFiECAnPeMi5N6VT3/uscoGH7+zJrv4AuuPg==";
        public static string GenerateAccessToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(Secret);

            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = Issuer,
                Audience = Audience,
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = signingCredentials,

            };
            try
            {
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(securityToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }            

        }
        public static string GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[32];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(secureRandomBytes);

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }
    }

    public static class TokenLifetimeValidator
    {
        public static bool Validate(
            DateTime? notBefore,
            DateTime? expires,
            SecurityToken tokenToValidate,
            TokenValidationParameters @param
        )
        {
            return (expires != null && expires > DateTime.UtcNow);
        }
    }
}
