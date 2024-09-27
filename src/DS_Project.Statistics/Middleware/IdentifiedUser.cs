namespace DS_Project.Statistics.Middleware
{
    public class IdentifiedUser
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public Guid SessionKey { get; set; }
    }
}
