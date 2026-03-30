using ChatCommon.DTO;
using ChatServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [Route("api/v1/user")]
    public class UserController(UserService userService) : Controller
    {

        [HttpGet]
        public IActionResult GetUser([FromQuery] Guid userId)
        {
            var res = userService.GetUser(userId);
            return Ok(res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserDTO userDTO)
        {
            await userService.CreateUser(userDTO);
            return Ok();
        }
    }
}
