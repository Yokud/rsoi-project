using DS_Project.Auth.DTO;

namespace DS_Project.Auth.Service
{
    public interface IUsersService
    {
        Task<string> Login(UserLoginRequest loginRequest);

        Task<Guid> Register(UserCreateRequest userCreateRequest);
    }
}
