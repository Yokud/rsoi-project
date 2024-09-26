using DS_Project.Auth.Entity;

namespace DS_Project.Auth.DTO
{
    public class UserDTO
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public UserRole Role { get; set; }
    }
}
