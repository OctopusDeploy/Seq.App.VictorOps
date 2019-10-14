using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace Seq.App.VictorOps
{
    public class VictorOpsService
    {
        private readonly Uri _uri;
        private readonly ILogger _logger;

        public VictorOpsService(string uri, ILogger logger)
        {
            _uri = new Uri(uri);
            _logger = logger;
        }

        public async Task PostAlert(PostAlertOptions options)
        {
            string routeKey = string.IsNullOrWhiteSpace(options.RoutingKey) ? string.Empty : options.RoutingKey;
            UriBuilder uriBuilder = new UriBuilder(_uri);
            StringBuilder sb = new StringBuilder(options.RestApiKey).Append("/");
            if (!string.IsNullOrWhiteSpace(options.RoutingKey))
            {
                sb.Append(options.RoutingKey).Append("/");
            }

            uriBuilder.Path += sb.ToString();
            var uri = uriBuilder.ToString();

            var payload = new VictorOpsAlert()
            {
                Tool = AlertSource.Seq,
                MessageType = options.Type,
                Title = options.Title,
                Message = options.Message,
                Id = options.Id
            };

            var payloadJson = JsonConvert.SerializeObject(payload);

            _logger.Verbose($"Sending VictorOps alert: {payloadJson} to {uri}");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(uri, new StringContent(payloadJson, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("Successfully sent request to VictorOps");
                }
                else
                {
                    var log = _logger.ForContext("statusCode", response.StatusCode);
                    var responseMessage = await response.Content.ReadAsStringAsync();
                    log.Error($"Failed to send request to VictorOps: {responseMessage}");
                }
            }
        }
    }
}