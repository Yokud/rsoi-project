using DS_Project.Auth.DTO;
using DS_Project.Auth.Service;
using Microsoft.AspNetCore.Mvc;

namespace DS_Project.Auth.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UsersController(IUsersService usersService, LogsProducer producer) : Controller
    {
        readonly IUsersService _usersService = usersService;
        readonly LogsProducer _producer = producer;

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var res = await _usersService.Login(loginRequest);

            if (string.IsNullOrEmpty(res))
            {
                await _producer.Produce($"[AuthService] User authorization failed {loginRequest.UserName}");
                return NotFound();
            }

            await _producer.Produce($"[AuthService] User authorization succeed {loginRequest.UserName}");
            return Ok(new JWT.JwtToken() { Token = res });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest userCreateRequest)
        {
            var id = await _usersService.Register(userCreateRequest);
            await _producer.Produce($"[AuthService] New user has registered ({id})");
            return CreatedAtAction(nameof(Register), id);
        }
    }
}
