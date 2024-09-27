using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DS_Project.Statistics.Middleware
{
    public class SymmetricSecurityKeysHelper
    {
        /// <summary>
        /// Генерирует симметричный ключ по <paramref name="rawString"/>
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string rawString)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(rawString));
        }
    }
    public class JwtValidator
    {

        JwtConfiguration _jwtConfiguration;
        public JwtValidator(IOptions<JwtConfiguration> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SymmetricSecurityKeysHelper.GetSymmetricSecurityKey(_jwtConfiguration.SecurityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero,
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IdentifiedUser? GetUserDataFromToken(string token)
        {
            if (ValidateToken(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SymmetricSecurityKeysHelper.GetSymmetricSecurityKey(_jwtConfiguration.SecurityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero,
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var userIdClaim = principal.FindFirst("Id");
                var userNameClaim = principal.FindFirst("UserName");
                var roleClaim = principal.FindFirst("Role");
                var sessionKeyClaim = principal.FindFirst("SessionKey");

                if (userIdClaim != null && userNameClaim != null && roleClaim != null && sessionKeyClaim != null)
                {
                    return new IdentifiedUser
                    {
                        Id = Guid.Parse(userIdClaim.Value),
                        UserName = userNameClaim.Value,
                        Role = roleClaim.Value,
                        SessionKey = new Guid(sessionKeyClaim.Value)
                    };
                }
            }

            return null;
        }
    }
}
