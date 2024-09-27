namespace DS_Project.Rentals.Middleware
{
    public class JwtConfiguration
    {
        /// <summary>
        /// Secret Key для генерации JWT токенов.
        /// Минимальная длинна - 16 символов.
        /// </summary>
        public string? SecurityKey { get; set; }

        /// <summary>
        /// Длительность жизни одного токена (часы).
        /// </summary>
        public int LifetimeHours { get; set; }
    }
}
