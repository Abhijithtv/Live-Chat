using Azure.Messaging.ServiceBus;
using ChatCommon.DTO;
using ChatOpsAzFunction.ChatEventHandlers;
using MasterDB.Entity;
using System.Text.Json;

namespace ChatOpsAzFunction.Services
{
    public class UserSentEventProcessor(ChatSentEventHandlerFactory chatSentEventHandlerFactory)
    {
        internal async Task HandleAsync(ServiceBusReceivedMessage message)
        {
            var payLoad = JsonSerializer.Deserialize<GenericChatMessageDTO>(message.Body)!;
            StoreMessageInfo(message, payLoad);
            await chatSentEventHandlerFactory
                .GetHandler(payLoad)
                .ProcessAsync(payLoad);
        }

        private void StoreMessageInfo(ServiceBusReceivedMessage message, GenericChatMessageDTO payLoad)
        {
            var info = new SentEventToQueueMapping
            {
                ClientTransactionId = payLoad.TransactionId,
                AzureMessageId = message.MessageId,
                AzureCorrelationId = message.CorrelationId,
                ProcessCount = message.DeliveryCount
            };

            //todo - store to db this info
        }
    }
}
