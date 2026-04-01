using Microsoft.Extensions.Configuration;
using System.Text;

namespace ChatOpsAzFunction.Services
{
    //singleton
    public class ChatServerClient(IConfiguration configuration)
    {
        public HttpClient _client { get; } = new HttpClient();

        private string _baseUrl;
        public string BaseUrl
        {
            get
            {
                if (_baseUrl == null)
                {
                    _baseUrl = configuration["ChatServerUrl"]?.TrimEnd('/')
                        ?? throw new InvalidOperationException("ChatServerUrl not configured");
                }
                return _baseUrl;
            }
        }

        public async Task SendGroupChatProcessAckAsync(string payload)
        {
            var groupChatEndpoint = _GetGroupChatEndpoint();
            using (var httpMsg = new HttpRequestMessage(HttpMethod.Post, groupChatEndpoint)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            })
            {
                var res = await _client.SendAsync(httpMsg);
                res.EnsureSuccessStatusCode();
            }

        }

        private Uri _GetGroupChatEndpoint()
        {
            return new Uri($"{BaseUrl}/api/v1/groupchat");
        }
    }
}
