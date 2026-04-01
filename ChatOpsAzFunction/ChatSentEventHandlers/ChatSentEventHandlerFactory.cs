using ChatCommon.DTO;
using ChatOpsAzFunction.ChatSentEventHandlers;
using ChatOpsAzFunction.ChatSentEventHandlers.Implementations;

namespace ChatOpsAzFunction.ChatEventHandlers
{
    public class ChatSentEventHandlerFactory(GroupChatSentEventHandler groupChatSentEventHandler)
    {
        public IChatSentEventHandler GetHandler(GenericChatMessageDTO msg)
        {
            IChatSentEventHandler handler = null;

            switch (msg.IsFromGroup)
            {
                case true:
                    handler = groupChatSentEventHandler;
                    break;
                default:
                    throw new InvalidOperationException("Unknown Event");
            }
            return handler;

        }
    }
}
