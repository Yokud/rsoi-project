using DS_Project.Auth.Entity;
using System.Security.Claims;

namespace DS_Project.Auth.Utils
{
    public static class ClaimsCreator
    {
        public static ClaimsIdentity CreateClaimsIdentity(IdentifiedUser user)
        {
            var claims = new List<Claim>()
            {
                new(nameof(IdentifiedUser.Id), user.Id.ToString()),
                new(nameof(IdentifiedUser.UserName), user.UserName),
                new(nameof(IdentifiedUser.Role), user.Role.GetRoleNameString()),
                new(nameof(IdentifiedUser.SessionKey), user.SessionKey.ToString())
            };

            return new ClaimsIdentity(claims, "Token", nameof(IdentifiedUser.UserName), nameof(IdentifiedUser.Role));
        }

        public static IdentifiedUser? ParseClaimsToIdentityUser(IEnumerable<Claim> claims)
        {
            if (!Guid.TryParse(claims.First(c => c.Type == nameof(IdentifiedUser.Id)).Value, out Guid userId))
                return null;

            var userName = claims.First(c => c.Type == nameof(IdentifiedUser.UserName)).Value;
            var role = claims.First(c => c.Type == nameof(IdentifiedUser.Role)).Value;

            if (!Guid.TryParse(claims.First(c => c.Type == nameof(IdentifiedUser.SessionKey)).Value, out Guid sessionKey))
                return null;

            return new IdentifiedUser() { Id = userId, UserName = userName, Role = UserRoleExtension.GetRoleNameFromString(role), SessionKey = sessionKey };
        }
    }
}
