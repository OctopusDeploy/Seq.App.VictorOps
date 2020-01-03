using Seq.Apps.LogEvents;

namespace Seq.App.VictorOps
{
    public static class VictorOpsAlertTypeMapper
    {
        public static AlertType Map(LogEventLevel seqLevel)
        {
            switch (seqLevel)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                case LogEventLevel.Information:
                    return AlertType.Info;
                case LogEventLevel.Warning:
                    return AlertType.Warning;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return AlertType.Critical;
                default:
                    return AlertType.Info;
            }
        }
    }
}
