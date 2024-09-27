namespace DS_Project.Auth.Entity
{
    public class IdentifiedUser
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public Guid SessionKey { get; set; }
    }
}
