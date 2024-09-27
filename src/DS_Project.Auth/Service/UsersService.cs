using DS_Project.Auth.Config;
using DS_Project.Auth.DTO;
using DS_Project.Auth.Entity;
using DS_Project.Auth.Repository;
using DS_Project.Auth.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DS_Project.Auth.Service
{
    public class UsersService : IUsersService
    {
        IUsersRepository _usersRepository;
        UserSessionKeysStorage _sessionKeysStorage;
        JwtConfiguration _jwtConfiguration;
        //ILogger<IUsersService> _logger;

        public UsersService(IUsersRepository usersRepository, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _usersRepository = usersRepository;
            _sessionKeysStorage = new UserSessionKeysStorage();
            _jwtConfiguration = jwtConfiguration.Value;
            //_logger = logger;
        }

        public async Task<string> Login(UserLoginRequest loginRequest)
        {
            var user = (await _usersRepository.GetAllAsync()).FirstOrDefault(u => u.UserName == loginRequest.UserName && u.Password == loginRequest.Password);

            if (user is null)
                return string.Empty;

            var sessionKey = _sessionKeysStorage.GenerateNewSessionKey(user.Id);

            var claimsIdentity = ClaimsCreator.CreateClaimsIdentity(new IdentifiedUser() { Id = user.Id, UserName = user.UserName, Role = user.Role, SessionKey = sessionKey });

            var token = CreateJwtToken(claimsIdentity);

            return token;
        }

        public async Task<Guid> Register(UserCreateRequest userCreateRequest)
        {
            var newId = Guid.NewGuid();
            var newUser = new User()
            {
                Id = newId,
                UserName = userCreateRequest.UserName,
                Password = userCreateRequest.Password,
                Role = UserRole.User.GetRoleNameString(),
                Email = userCreateRequest.Email,
                FirstName = userCreateRequest.FirstName,
                LastName = userCreateRequest.LastName,
                PhoneNumber = userCreateRequest.PhoneNumber,
            };

            await _usersRepository.CreateAsync(newUser);
            await _usersRepository.SaveAsync();

            return newId;
        }

        string CreateJwtToken(ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (_jwtConfiguration.SecurityKey == null)
            {
                //_logger.LogCritical("JwtConfiguration not loaded in {service}.", nameof(UsersService));

                throw new ArgumentNullException(nameof(_jwtConfiguration), "Security key is null!");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(_jwtConfiguration.LifetimeHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfiguration.SecurityKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
