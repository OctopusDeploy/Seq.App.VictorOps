using System.Collections;
using System.Linq;
using System.Text;
using Seq.Apps;
using Seq.Apps.LogEvents;
using Newtonsoft.Json.Converters;
using Seq.App.VictorOps.Models;

namespace Seq.App.VictorOps
{
    [SeqApp("VictorOps")]
    public class VictorOpsSeqApp : SeqApp, ISubscribeTo<LogEventData>
    {
        private VictorOpsService _service;

        public void On(Event<LogEventData> evt)
        {
            EnsureService();

            AlertType type = VictorOpsAlertTypeMapper.Map(evt.Data.Level);
            StringBuilder sb = new StringBuilder(evt.Data.RenderedMessage);

            var postTask = _service.PostAlert(new PostAlertOptions
            {
                Message = evt.Data.RenderedMessage,
                RestApiKey = RestApiKey,
                Title = Title,
                RoutingKey = RoutingKey,
                Type = type,
                Id = evt.Id,
                TestMode = TestMode,
                Properties = evt.Data.Properties.ToDictionary(x => x.Key, x => x.Value?.ToString())
            });
            
            postTask.Wait();
        }

        private void EnsureService()
        {
            if (_service == null)
            {
                _service = new VictorOpsService(Url, Log);
            }
        }

        [SeqAppSetting(DisplayName = "Victor Ops URL", IsOptional = false, InputType = SettingInputType.Text)]
        public string Url { get; set; }

        [SeqAppSetting(DisplayName = "Incident Title", IsOptional = false, InputType = SettingInputType.Text)]
        public string Title { get; set; }

        [SeqAppSetting(DisplayName = "REST API Key", IsOptional = false, InputType = SettingInputType.Password)]
        public string RestApiKey { get; set; }

        [SeqAppSetting(DisplayName = "Routing Key", IsOptional = true, InputType = SettingInputType.Text)]
        public string RoutingKey { get; set; }

        [SeqAppSetting(DisplayName = "Test Mode", IsOptional = true, InputType = SettingInputType.Checkbox, HelpText = "Incidents will not be raised in test mode")]
        public bool TestMode { get; set; }

    }
}
