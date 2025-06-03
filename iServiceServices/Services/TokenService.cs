using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace iServiceServices.Services
{
    public class TokenInfo
    {
        public int UserId { get; set; }
        public int UserProfileId { get; set; }
        public int AddressId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
    public static class TokenService
    {
        public const string UserId = "UserId";
        public const string UserProfileId = "UserProfileId";
        public const string AddressId = "AddressId";

        public static string GenerateToken((User User, UserRole UserRole, UserProfile UserProfile) user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.User.Email),
                new Claim(ClaimTypes.Role, user.UserRole.Name),
                new Claim(UserId, user.User.UserId.ToString()),
                new Claim(UserProfileId, user.UserProfile.UserProfileId.ToString()),
                new Claim(AddressId, user.UserProfile.AddressId?.ToString() ?? "")
            }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string GetJwtToken(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            return string.Empty;
        }

        public static TokenInfo GetTokenInfo(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();

                var tokenInfo = new TokenInfo
                {
                    UserId = int.TryParse(claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out int userId) ? userId : 0,
                    UserProfileId = int.TryParse(claims.FirstOrDefault(c => c.Type == "UserProfileId")?.Value, out int userProfileId) ? userProfileId : 0,
                    AddressId = int.TryParse(claims.FirstOrDefault(c => c.Type == "AddressId")?.Value, out int addressId) ? addressId : 0,
                    Name = claims.FirstOrDefault(c => c.Type == "unique_name")?.Value,
                    Role = claims.FirstOrDefault(c => c.Type == "role")?.Value
                };

                return tokenInfo;
            }
            return null;
        }
    }

    public interface ITokenInfoService
    {
        TokenInfo TokenInfo { get; set; }
    }

    public class TokenInfoService : ITokenInfoService
    {
        public TokenInfo TokenInfo { get; set; }
    }
    public class TokenInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITokenInfoService tokenInfoService)
        {
            var token = GetJwtToken(context);

            if (!string.IsNullOrEmpty(token))
            {
                var tokenInfo = GetTokenInfo(token);
                tokenInfoService.TokenInfo = tokenInfo;
            }

            await _next(context);
        }

        private static string GetJwtToken(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }
            return string.Empty;
        }

        private static TokenInfo GetTokenInfo(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();

                return new TokenInfo
                {
                    UserId = TryParseClaimToInt(claims, "UserId"),
                    UserProfileId = TryParseClaimToInt(claims, "UserProfileId"),
                    AddressId = TryParseClaimToInt(claims, "AddressId"),
                    Name = claims.FirstOrDefault(c => c.Type == "unique_name")?.Value,
                    Role = claims.FirstOrDefault(c => c.Type == "role")?.Value
                };
            }
            return null;
        }
        private static int TryParseClaimToInt(IEnumerable<Claim> claims, string claimType)
        {
            var claimValue = claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            int result;
            return int.TryParse(claimValue, out result) ? result : 0; // Retorna 0 se a conversão falhar
        }
    }
}
