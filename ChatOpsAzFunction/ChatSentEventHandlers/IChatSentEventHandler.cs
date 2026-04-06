using ChatCommon.DTO;

namespace ChatOpsAzFunction.ChatSentEventHandlers
{
    public interface IChatSentEventHandler
    {
        Task ProcessAsync(GenericChatMessageDTO message);
    }
}
