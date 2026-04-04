using Azure.Messaging.ServiceBus;
using ChatCommon.DTO;
using System.Text.Json;

namespace ChatServer.Queue
{
    public class UserSentMsgQueue(IConfiguration configuration)
    {
        private ServiceBusSender _sender;
        public bool InitSender()
        {
            if (_sender != null)
            {
                return true;
            }

            lock (this)
            {
                string connectionString = configuration.GetValue<string>("ServiceBusSettings:ConnectionString")!;
                string queueName = configuration.GetValue<string>("ServiceBusSettings:QueueName")!;

                try
                {
                    var client = new ServiceBusClient(connectionString);
                    _sender = client.CreateSender(queueName);
                }
                catch (Exception ex)
                {

                    throw new Exception($"Failed to Create Sender to Queue - {queueName}", ex);

                }
            }
            return true;
        }

        public async Task<bool> AddAsync(GenericChatMessageDTO msg)
        {
            InitSender();
            string body = JsonSerializer.Serialize(msg);
            var message = new ServiceBusMessage(body);
            try
            {
                await _sender.SendMessageAsync(message);
            }
            catch (ServiceBusException ex)
            {
                //in future: Add a log for this error
                return false;
            }

            return true;
        }
    }
}
