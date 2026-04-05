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
            //1. Check UserSentMsgAck
            //      1.1) if processed -> skip and return
            //      1.2) if failed -> change to in progress and continue
            //      1.3) if in queue -> change to in progress and continue
            //      1.4) if in progress -> skip and return (some one is already processing it)
            //
            //2. Sequence Number
            //      2.1) Assign a sequence number if its not yet given 
            //          2.1.1) Find group chat, inc last seq no
            //          2.2.2) Create new chat msg and set its seq no 
            //          2.2.3) Assign msg id to transaction id
            //       2.2) if given , msg and transaction id already set
            //      
            // 3. Add message to GroupChatMessageLog if not done

            bool shouldProcess = await _IsCandidateToProcessAsync(message);

            if (!shouldProcess)
            {
                return;
            }

            var chatMessage = await CreateChatMessageWithSeqNoAsync(message);
            await _InsertIntoGroupChatLogAsync(chatMessage, message);
            //send ack to chat server (transactionid, messageId)
            await _SendAckToWebServerAsync(chatMessage.MessageId, message.TransactionId);
        }

        private async Task<ChatMessage> CreateChatMessageWithSeqNoAsync(GenericChatMessageDTO message)
        {
            var res = await masterDBContext.Database.SqlQuery<ChatMessage>($@"
                        BEGIN TRY
                            BEGIN TRAN;

                            DECLARE @SeqId INT;
                            DECLARE @MessageChatId UNIQUEIDENTIFIER;

                            SELECT @MessageChatId = ChatMessageId
                            FROM ClientMessage WITH (UPDLOCK, ROWLOCK)
                            WHERE TransactionId = {message.TransactionId};

                            IF @MessageChatId IS NULL
                            BEGIN
                                UPDATE GroupChat
                                SET @SeqId = CurrentSequenceNumber = CurrentSequenceNumber + 1
                                WHERE GroupChatId = {message.ToUserId};

                                SET @MessageChatId = NEWID();

                                INSERT INTO ChatMessage (MessageId, SequenceNumber, Message, STATUS)
                                VALUES (@MessageChatId, @SeqId, {message.Message}, 'Created');

                                UPDATE ClientMessage
                                SET ChatMessageId = @MessageChatId,
                                    STATUS = 'Processed'
                                WHERE TransactionId = {message.TransactionId};
                            END

                            SELECT *
                            FROM ChatMessage
                            WHERE MessageId = @MessageChatId;

                            COMMIT;
                        END TRY
                        BEGIN CATCH
                            ROLLBACK;
                            THROW;
                        END CATCH
                        ").FirstAsync();
            return res;
        }

        private async Task<bool> _IsCandidateToProcessAsync(GenericChatMessageDTO message)
        {
            var rows = await masterDBContext.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE ClientMessage
                        SET Status = {MessageStatusEnum.InProcessing.ToString()}
                        WHERE TransactionId = {message.TransactionId}
                        AND SenderId = {message.FromUserId}
                        AND Status NOT IN (
                        {MessageStatusEnum.InProcessing.ToString()},
                        {MessageStatusEnum.Processed.ToString()})");
            return rows == 1;
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
            //todo - add unique constraint
            var GroupChatMessageLog = new GroupChatMessageLog()
            {
                ToGroupId = message.ToUserId,
                FromUserId = message.FromUserId,
                ChatMessageId = chatMessage.MessageId
            };
            masterDBContext.GroupChatMessageLog.Add(GroupChatMessageLog);
            try
            {
                await masterDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private ChatMessage _CreateChatMessageAsync(GenericChatMessageDTO message, int nextSeqNumber)
        {
            var msgObj = new ChatMessage()
            {
                MessageId = Guid.NewGuid(),
                SequenceNumber = nextSeqNumber,
                Message = message.Message,
                Status = ChatMessageStausEnum.Created.ToString(),
            };
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
