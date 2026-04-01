using ChatCommon.Constants;
using ChatCommon.DTO;
using ChatOpsAzFunction.Services;
using MasterDB;
using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ChatOpsAzFunction.ChatSentEventHandlers.Implementations
{
    public class GroupChatSentEventHandler(MasterDBContext masterDBContext,
        ChatServerClient chatServerClient) : IChatSentEventHandler
    {
        public async Task ProcessAsync(GenericChatMessageDTO message)
        {
            //option1
            //step - 1
            //begin transac
            //1) seq = find last seq number from group chat
            //2) create chat message  with seq + 1
            //3) update last seq in group chat
            // 
            //step - 2
            //add the msg to group chat msg log 
            //end transac

            //option 2
            //seq = get and inc
            //create chat msg and chat msg log with this


            //going with option2 
            //tradeoff - no sequential id but less blocking

            int nextSeqNumber = await _UpdateAndGetSequenceNumberAsync(message.ToUserId);
            var chatMessage = await _CreateChatMessageAsync(message, nextSeqNumber);
            await _InsertIntoGroupChatLogAsync(chatMessage, message);

            //send ack to chat server (transactionid, messageId)
            await _SendAckToWebServerAsync(chatMessage.MessageId, message.TransactionId);
        }

        private async Task _SendAckToWebServerAsync(Guid messageId, Guid transactionId)
        {
            var ack = new ChatMessageOpsDTO()
            {
                MessageId = messageId,
                TransactionId = transactionId
            };
            var payLoad = JsonSerializer.Serialize(ack);
            await chatServerClient.SendGroupChatProcessAckAsync(payLoad);
        }

        private async Task _InsertIntoGroupChatLogAsync(ChatMessage chatMessage, GenericChatMessageDTO message)
        {
            var GroupChatMessageLog = new GroupChatMessageLog()
            {
                ToGroupId = message.ToUserId,
                FromUserId = message.FromUserId,
                ChatMessageId = chatMessage.MessageId
            };
            masterDBContext.GroupChatMessageLog.Add(GroupChatMessageLog);
            await masterDBContext.SaveChangesAsync();
        }

        private async Task<ChatMessage> _CreateChatMessageAsync(GenericChatMessageDTO message, int nextSeqNumber)
        {
            var msgObj = new ChatMessage()
            {
                MessageId = Guid.NewGuid(),
                SequenceNumber = nextSeqNumber,
                Message = message.Message,
                Status = ChatMessageStausEnum.Created.ToString(),
            };
            masterDBContext.ChatMessage.Add(msgObj);
            await masterDBContext.SaveChangesAsync();
            return msgObj;
        }

        private async Task<int> _UpdateAndGetSequenceNumberAsync(Guid groupId)
        {
            return await masterDBContext.Database
                .SqlQuery<int>($@"
                    UPDATE GroupChat
                    SET CurrentSequenceNumber = CurrentSequenceNumber + 1
                    OUTPUT INSERTED.CurrentSequenceNumber
                    WHERE GroupChatId = {groupId}")
                .FirstAsync();
        }
    }
}
