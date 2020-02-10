using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace K8sFrontEnd.Services
{
    public class HTTPReqService
    {
        private readonly HttpClient _HttpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        public HTTPReqService()
        {
            _HttpClient = new HttpClient();
            HttpStatusCode[] httpStatusCodesWorthRetrying = {
               HttpStatusCode.RequestTimeout, // 408
               HttpStatusCode.InternalServerError, // 500
               HttpStatusCode.BadGateway, // 502
               HttpStatusCode.ServiceUnavailable, // 503
               HttpStatusCode.GatewayTimeout // 504
            };
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrInner<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                  .WaitAndRetryAsync(new[]
                  {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8)
                  });
        }

        public async Task<string> CallApiAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url is empty or null", nameof(url));
            }
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            string responseString;
            try
            {
                HttpResponseMessage response;
                response = await _retryPolicy.ExecuteAsync(async () =>
                         await CreateAndSendMessageAsync(url)
                    );
                responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task<HttpResponseMessage> CreateAndSendMessageAsync(string url)
        {
            HttpResponseMessage response;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            response = await _HttpClient.SendAsync(requestMessage);
            return response;
        }


    }
}
