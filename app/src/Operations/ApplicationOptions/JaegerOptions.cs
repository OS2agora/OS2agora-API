using System;

namespace Agora.Operations.ApplicationOptions
{
    public class JaegerOptions
    {
        public const string Jaeger = "Jaeger";
        public Uri Endpoint { get; set; }
        public bool EnableRuntimeMetrics { get; set; }
    }
}