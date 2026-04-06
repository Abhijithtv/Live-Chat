using ChatCommon.Constants;
using ChatCommon.DTO;
using ChatServer.Managers;
using MasterDB;
using MasterDB.Entity;
using System.Net.WebSockets;
using System.Text.Json;

namespace ChatServer.Handlers.SocketHandler
{
    public class MessageFwdHandler(ConnectionManager connectionManager, MasterDBContext masterDBContext)
    {
        internal async Task FwdAsync(GroupChatMessageLog msgLog, List<Guid> members, Guid transactionId)
        {
            var fwdMessageDTO = new FwdMessageDTO()
            {
                SenderId = msgLog.FromUserId,
                GroupId = msgLog.ToGroupId,
                TransactionID = transactionId,
                SequenceNumber = msgLog.ChatMessage.SequenceNumber,
                CreatedOnUTC = msgLog.CreatedOnUTC,
                Message = msgLog.ChatMessage.Message,
                MessageId = msgLog.ChatMessageId
            };

            var resp = JsonSerializer.SerializeToUtf8Bytes(fwdMessageDTO);

            foreach (var member in members)
            {
                var connection = connectionManager.GetConnection(member);
                await connection!.SendAsync(new ArraySegment<byte>(resp), WebSocketMessageType.Text, true, default);
            }

            var clientMessage = new ClientMessage()
            {
                TransactionId = transactionId,
                Status = MessageStatusEnum.Processed.ToString()
            };
            masterDBContext.ClientMessage.Attach(clientMessage);
            masterDBContext.ClientMessage.Entry(clientMessage).Property(x => x.Status).IsModified = true;
            await masterDBContext.SaveChangesAsync();
        }
    }
}
