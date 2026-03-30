using ChatCommon.DTO;

namespace ChatServer.EventHandlers
{
    public class UserSentEvent
    {
        internal Task HandleMessage(Guid userId, ChatMessageDTO? msg)
        {
            //todo - sent it to Queue
            Console.WriteLine("Send TO Azure Queue");
            return Task.CompletedTask;
        }
    }
}
