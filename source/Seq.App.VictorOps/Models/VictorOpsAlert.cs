using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Seq.App.VictorOps
{

    public class VictorOpsInstanceAlert : VictorOpsAlert
    {
        [JsonProperty("CustomerId")]
        public string CustomerId { get; set; }

        [JsonProperty("Environment")]
        public string Environment { get; set; }

        [JsonProperty("Region")]
        public string Region { get; set; }
    }

    public class VictorOpsAlert
    {
        [JsonProperty("message_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertType MessageType { get; set; }

        [JsonProperty("monitoring_tool")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertSource Tool { get; set; }

        [JsonProperty("entity_id")]
        public string Id { get; set; }

        [JsonProperty("entity_display_name")]
        public string Title { get; set; }

        [JsonProperty("state_message")]
        public string Message { get; set; }
    }

    public enum AlertType
    {
        Critical,
        Warning,
        Acknowledgement,
        Info,
        Recovery
    }

    public enum AlertSource
    {
        HostedPortal,
        Manual
    }
}
