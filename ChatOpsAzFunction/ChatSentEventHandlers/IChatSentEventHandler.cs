using ChatCommon.DTO;

namespace ChatOpsAzFunction.ChatSentEventHandlers
{
    internal interface IChatSentEventHandler
    {
        Task ProcessAsync(GenericChatMessageDTO message);
    }
}
