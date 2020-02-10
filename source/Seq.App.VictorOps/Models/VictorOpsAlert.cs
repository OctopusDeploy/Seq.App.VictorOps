using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Seq.App.VictorOps
{

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

        [JsonProperty("vo_annotate.s.Exception")]
        public string Exception { get; set; }
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
        Unknown,
        Seq,
    }
}
