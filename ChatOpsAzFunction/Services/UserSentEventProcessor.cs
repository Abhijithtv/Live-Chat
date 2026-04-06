using Azure.Messaging.ServiceBus;
using ChatCommon.DTO;
using ChatOpsAzFunction.ChatEventHandlers;
using MasterDB;
using MasterDB.Entity;
using System.Text.Json;

namespace ChatOpsAzFunction.Services
{
    public class UserSentEventProcessor(
        ChatSentEventHandlerFactory chatSentEventHandlerFactory,
        MasterDBContext masterDBContext)
    {
        internal async Task HandleAsync(ServiceBusReceivedMessage message)
        {
            var payLoad = JsonSerializer.Deserialize<GenericChatMessageDTO>(message.Body)!;
            await StoreMessageInfoAsync(message, payLoad);
            await chatSentEventHandlerFactory
                .GetHandler(payLoad)
                .ProcessAsync(payLoad);
        }

        private async Task StoreMessageInfoAsync(ServiceBusReceivedMessage message, GenericChatMessageDTO payLoad)
        {
            var info = new SentEventToQueueMapping
            {
                ClientTransactionId = payLoad.TransactionId,
                AzureMessageId = message.MessageId,
                AzureCorrelationId = message.CorrelationId,
                ProcessCount = message.DeliveryCount
            };

            masterDBContext.SentEventToQueueMappings.Add(info);
            await masterDBContext.SaveChangesAsync();
        }
    }
}
