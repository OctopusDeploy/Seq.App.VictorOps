using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seq.App.VictorOps.Models;
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
            var uri = GetVictorOpsUri(options);

            var payloadJson = GetPayloadJson(options);

            _logger.Verbose("Sending VictorOps alert: {PayloadJson} to base Uri: {Uri}", payloadJson, _uri);

            if (options.TestMode)
            {
                _logger.Information($"Test mode enabled. Not sending incident.");
            }
            else
            {
                await CreateIncident(uri, payloadJson);
            }
        }

        private static string GetPayloadJson(PostAlertOptions options)
        {
            var payload = new VictorOpsAlert()
            {
                Tool = AlertSource.Seq,
                MessageType = options.Type,
                Title = options.Title,
                Message = options.Message,
                Id = options.Id
            };

            var jo = (JObject) JToken.FromObject(payload);
            foreach (var property in options.Properties)
            {
                jo.Add(property.Key, property.Value);
            }

            var payloadJson = JsonConvert.SerializeObject(jo);
            return payloadJson;
        }

        private string GetVictorOpsUri(PostAlertOptions options)
        {
            UriBuilder uriBuilder = new UriBuilder(_uri);
            StringBuilder sb = new StringBuilder(options.RestApiKey).Append("/");
            if (!string.IsNullOrWhiteSpace(options.RoutingKey))
            {
                sb.Append(options.RoutingKey).Append("/");
            }

            uriBuilder.Path += sb.ToString();
            var uri = uriBuilder.ToString();
            return uri;
        }

        private async Task SendIncident(string uri, string payloadJson)
        {
            using (var httpClient = new HttpClient())
            {
                var response =
                    await httpClient.PostAsync(uri, new StringContent(payloadJson, Encoding.UTF8, "application/json"));
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
