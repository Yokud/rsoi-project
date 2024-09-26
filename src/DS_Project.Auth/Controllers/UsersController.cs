using DS_Project.Auth.DTO;
using DS_Project.Auth.Service;
using Microsoft.AspNetCore.Mvc;

namespace DS_Project.Auth.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UsersController : Controller
    {
        readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var res = await _usersService.Login(loginRequest);

            if (string.IsNullOrEmpty(res))
                return NotFound();

            return Ok(new JWT.JwtToken() { Token = res });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest userCreateRequest)
        {
            var id = await _usersService.Register(userCreateRequest);

            return CreatedAtAction(nameof(Register), id);
        }
    }
}
