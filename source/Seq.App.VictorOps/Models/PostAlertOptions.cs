using System;
using System.Collections.Generic;

namespace Seq.App.VictorOps.Models
{
    public class PostAlertOptions
    {
        public AlertType Type { get; set; }
        public string Title { get; set; }
        public string RestApiKey { get; set; }
        public string RoutingKey { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public bool TestMode { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public string Exception { get; set; }
    }
}