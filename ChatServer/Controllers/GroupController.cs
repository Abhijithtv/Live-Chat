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
        public async Task<IActionResult> JoinGroupAsync([FromQuery] Guid groupId, [FromQuery] Guid userId)
        {
            await groupService.JoinGroup(groupId, userId);
            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupChatDTO groupInfo)
        {
            await groupService.CreateGroup(groupInfo);
            return Created();
        }
    }
}
