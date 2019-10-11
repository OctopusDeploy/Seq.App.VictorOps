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

        public async Task PostAlert(AlertSource source, AlertType alertType, string title, string customerId, string environment, string region, string message)
        {
            var uri = new Uri(_uri, source.ToString()); // source is the routing key

            var payload = new VictorOpsInstanceAlert
            {
                Tool = AlertSource.HostedPortal,
                MessageType = alertType,
                Title = title,
                Message = message,
                CustomerId = customerId,
                Environment = environment,
                Region = region
            };

            var payloadJson = JsonConvert.SerializeObject(payload);

            _logger.Information($"Sending VictorOps alert: {payloadJson}");

            using (var httpClient = new HttpClient())
            {
                await httpClient.PostAsync(uri, new StringContent(payloadJson, Encoding.UTF8, "application/json"));
            }
        }


    }
}