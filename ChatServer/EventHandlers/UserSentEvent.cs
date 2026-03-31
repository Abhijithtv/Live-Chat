using ChatCommon.Constants;
using ChatCommon.DTO;
using ChatCommon.Models;
using ChatServer.Queue;
using MasterDB;
using MasterDB.Entity;

namespace ChatServer.EventHandlers
{
    public class UserSentEvent(MasterDBContext masterDBContext, UserSentMsgQueue userSentMsgQueue)
    {
        internal async Task<UserSentMsgAck> HandleMessage(Guid userId, GenericChatMessageDTO msg)
        {
            //store transaction data and payload in db
            var msgInfo = new ClientMessageTracker
            {
                ClientTransactionId = msg.TransactionId,
                UserId = userId,
                Status = MessageStatusEnum.InQueue.ToString(),
                ToUserId = msg.ToUserId,
                IsFromGroup = msg.IsFromGroup
            };

            await userSentMsgQueue.AddAsync(msg);

            Console.WriteLine("Sent TO Azure Queue");

            //todo - db schema and save changes

            var ack = new UserSentMsgAck()
            {
                TransactionId = msg.TransactionId,
                Status = MessageStatusEnum.InQueue.ToString(),
                Msg = "Got Your Message and it will be handled by us"
            };
            return ack;
        }
    }
}
