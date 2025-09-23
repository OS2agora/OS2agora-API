using System.Diagnostics;

namespace BallerupKommune.Operations.Common.Telemetry
{
    public static class Instrumentation
    {
        public static readonly ActivitySource Source = new ActivitySource(Constants.Telemetry.ActivitySourceName);
    }
}