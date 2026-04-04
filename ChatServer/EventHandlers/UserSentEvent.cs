using ChatCommon.Constants;
using ChatCommon.DTO;
using ChatCommon.Models;
using ChatServer.Queue;
using MasterDB;
using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.EventHandlers
{
    public class UserSentEvent(MasterDBContext masterDBContext, UserSentMsgQueue userSentMsgQueue)
    {
        //todo - make it config
        static double InQueueTime = 60000; //1 mins;

        internal async Task<UserSentMsgAck> HandleMessage(Guid userId, GenericChatMessageDTO msg)
        {
            // 1) user id and transaction id -> keep the item in db
            // 2) if already in db
            //      2.1) processed -> sent ack already done
            //      2.2) in queue
            //          2.2.1) less than x time -> can assume if may process
            //          2.2.1) time exceeds x time -> might have lost and need reprocessing(upate queue time)
            //      2.3) failed -> retry
            // concern -> read and write will be a bottle neck?

            var clientMessage = await masterDBContext
                .ClientMessage
                .Where(x => x.TransactionId == msg.TransactionId && x.SenderId == userId)
                .FirstOrDefaultAsync();

            MessageStatusEnum status = MessageStatusEnum.None; //new msg

            if (clientMessage != null)
            {
                Enum.TryParse(clientMessage.Status, out status);
            }

            switch (status)
            {
                case MessageStatusEnum.Processed:
                case MessageStatusEnum.InProcessing:
                    return new UserSentMsgAck
                    {
                        TransactionId = msg.TransactionId,
                        Status = MessageStatusEnum.Processed.ToString(),
                        Msg = "Message has under processing or processed successfully"
                    };

                case MessageStatusEnum.InQueue:
                    var lastUpdated = clientMessage!.LastUpdatedOnUTC;
                    var diffInMilliSec = (DateTime.UtcNow - lastUpdated).TotalMilliseconds;
                    if (diffInMilliSec < InQueueTime)
                    {
                        return new UserSentMsgAck
                        {
                            TransactionId = msg.TransactionId,
                            Status = MessageStatusEnum.InQueue.ToString(),
                            Msg = "Message is under processing"
                        };
                    }
                    break;
            }

            var newClientMessage = new ClientMessage()
            {
                TransactionId = msg.TransactionId,
                Status = MessageStatusEnum.InQueue.ToString(),
                SenderId = userId,
                LastUpdatedOnUTC = DateTime.UtcNow,
            };

            //add or update
            if (clientMessage != null)
            {
                masterDBContext.ClientMessage.Update(newClientMessage);
            }
            else
            {
                masterDBContext.ClientMessage.Add(newClientMessage);
            }

            try
            {
                //this is prone to reace condition
                await masterDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);

                var existing = await masterDBContext
                .ClientMessage
                .Where(x => x.TransactionId == msg.TransactionId && x.SenderId == userId)
                .FirstOrDefaultAsync();

                return new UserSentMsgAck
                {
                    TransactionId = msg.TransactionId,
                    Status = existing!.Status,
                    Msg = "Existing entry"
                };
            }

            var isSuccess = await userSentMsgQueue.AddAsync(msg);

            var ack = new UserSentMsgAck()
            {
                TransactionId = msg.TransactionId,
                Status = isSuccess ?
                            MessageStatusEnum.InQueue.ToString() :
                            MessageStatusEnum.Failed.ToString(),
                Msg = "Got Your Message and it will be handled by us"
            };
            return ack;
        }
    }
}
