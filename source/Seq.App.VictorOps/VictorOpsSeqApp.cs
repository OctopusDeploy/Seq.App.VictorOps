using System.Linq;
using System.Text;
using Seq.Apps;
using Seq.Apps.LogEvents;
using Newtonsoft.Json.Converters;

namespace Seq.App.VictorOps
{
    [SeqApp("VictorOps")]
    public class VictorOpsSeqApp : SeqApp, ISubscribeTo<LogEventData>
    {
        private VictorOpsService service;

        public void On(Event<LogEventData> evt)
        {
            EnsureService();

            AlertType type = VictorOpsAlertTypeMapper.Map(evt.Data.Level);

            StringBuilder sb = new StringBuilder(evt.Data.RenderedMessage);

            sb.AppendLine("Properties:");
            foreach (var property in evt.Data.Properties)
            {
                sb.AppendLine($"{property.Key} = {property.Value}");
            }

            var customer = IdentifierParser.GetCustomerIdentifier(evt.Data);
            var environment = IdentifierParser.GetEnvironmentIdentifier(evt.Data);
            var region = IdentifierParser.GetRegionIdentifier(evt.Data);

            var postTask = service.PostAlert(AlertSource.HostedPortal, type, Title, sb.ToString(), customer,
                environment, region);
            
            postTask.Wait();
        }

        private void EnsureService()
        {
            if (service == null)
            {
                service = new VictorOpsService(Url, Log);
            }
        }

        [SeqAppSetting(DisplayName = "Victor Ops URL", IsOptional = false, InputType = SettingInputType.Text)]
        public string Url { get; set; }

        public string Title {get;set;}

    }
}
