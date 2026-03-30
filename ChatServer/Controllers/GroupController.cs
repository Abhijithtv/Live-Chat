using ChatCommon.DTO;
using ChatServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [Route("api/v1/group")]
    public class GroupController(GroupService groupService) : Controller
    {

        [HttpGet]
        public IActionResult GetGroup([FromQuery] Guid groupId)
        {
            var res = groupService.GetGroup(groupId);
            return Ok(res);
        }


        [HttpPost("user-join")]
        public IActionResult JoinGroup([FromQuery] Guid groupId, [FromQuery] Guid userId)
        {
            groupService.JoinGroup(groupId, userId);
            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDTO groupInfo)
        {
            await groupService.CreateGroup(groupInfo);
            return Created();
        }
    }
}
