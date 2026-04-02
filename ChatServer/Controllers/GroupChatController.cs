using ChatCommon.DTO;
using ChatServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [Route("api/v1/groupChat")]
    public class GroupChatController(GroupService groupService) : Controller
    {
        public async Task<IActionResult> PostAsync([FromBody] ChatMessageOpsDTO chatMessageOpsDTO)
        {
            await groupService.SendMessageAsync(chatMessageOpsDTO);
            return Ok();
        }
    }
}
