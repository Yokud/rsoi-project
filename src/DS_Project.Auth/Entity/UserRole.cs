namespace DS_Project.Auth.Entity
{
    public enum UserRole
    {
        User,
        Admin
    }

    public static class UserRoleExtension
    {
        public static string GetRoleNameString(this UserRole role) => role.ToString().ToLowerInvariant();

        public static UserRole GetRoleNameFromString(string str)
        {
            if (str.ToLowerInvariant() == UserRole.User.GetRoleNameString())
                return UserRole.User;
            else if (str.ToLowerInvariant() == UserRole.Admin.GetRoleNameString())
                return UserRole.Admin;
            else
                throw new ArgumentException("Unknown role");
        }
    }
}