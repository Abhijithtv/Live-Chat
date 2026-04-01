using Azure.Messaging.ServiceBus;
using ChatOpsAzFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChatOpsAzFunction;

public class UserSentEventAzFunc
{
    private readonly ILogger<UserSentEventAzFunc> _logger;
    private readonly UserSentEventProcessor _processor;

    public UserSentEventAzFunc(ILogger<UserSentEventAzFunc> logger, UserSentEventProcessor userSentEventProcessor)
    {
        _logger = logger;
        _processor = userSentEventProcessor;
    }

    [Function(nameof(UserSentEventAzFunc))]
    public async Task Run(
        [ServiceBusTrigger("user-chat-queue", Connection = "ChatServiceBusConnStr")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        await _processor.HandleAsync(message);

        await messageActions.CompleteMessageAsync(message);
    }
}