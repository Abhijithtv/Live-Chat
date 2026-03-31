using ChatCommon.Constants;
using ChatCommon.DTO;
using ChatCommon.Models;
using MasterDB;
using MasterDB.Entity;

namespace ChatServer.EventHandlers
{
    public class UserSentEvent(MasterDBContext masterDBContext)
    {
        internal Task<UserSentMsgAck> HandleMessage(Guid userId, GenericChatMessageDTO msg)
        {
            //todo - sent it to Queue
            //store transaction data and payload in db
            var msgInfo = new ClientMessageTracker
            {
                ClientTransactionId = msg.TransactionId,
                UserId = userId,
                Status = MessageStatusEnum.InQueue.ToString(),
                ToUserId = msg.ToUserId,
                IsFromGroup = msg.IsFromGroup
            };

            //todo - db schema and save changes
            Console.WriteLine("Send TO Azure Queue");

            var ack = new UserSentMsgAck()
            {
                TransactionId = msg.TransactionId,
                Status = MessageStatusEnum.InQueue.ToString(),
                Msg = "Got Your Message and it will be handled by us"
            };
            return Task.FromResult(ack);
        }
    }
}
