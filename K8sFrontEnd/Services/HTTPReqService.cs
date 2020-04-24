using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using K8sFrontEnd.Models;

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

        public async Task<string> CallGetApiAsync(string url)
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
                         await CreateAndSendGetMessageAsync(url)
                    );
                responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task<HttpResponseMessage> CreateAndSendGetMessageAsync(string url)
        {
            HttpResponseMessage response;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            response = await _HttpClient.SendAsync(requestMessage);
            return response;
        }

        public async Task<string> CallGetPollylessAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("url is empty or null", nameof(url));
            }
            string responseString;
            try
            {
                HttpResponseMessage response;
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                response = await _HttpClient.SendAsync(requestMessage);
                responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> CallPostApiAsync(string url, IEnumerable<WeatherForecast> requestvariables)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("Username is empty or null", nameof(url));
            }
            string response;
            HttpResponseMessage responseMessage;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            try
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestvariables),
                    Encoding.UTF8, "application/json");
                responseMessage = await _HttpClient.SendAsync(requestMessage);
                response = await responseMessage.Content.ReadAsStringAsync();
                return response;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("One or more errors"))
                {
                    response = ex.InnerException.Message;
                }
                else
                {
                    response = ex.Message;
                }
                return response;
            }
        }


    }
}
