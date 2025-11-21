using Agora.Primitives.Constants;

namespace Agora.Primitives.Logic
{
    public static class Environment
    {
        public static bool IsRunningInDocker() =>
            System.Environment.GetEnvironmentVariable(EnvironmentVariables.RunningInDocker) == EnvironmentVariables.RunningInDockerTrueValue;

        public static bool IsDevelopment() =>
            GetAspNetCoreEnvironment() == EnvironmentVariables.AspNetCoreEnvironmentDevelopmentValue;

        public static bool IsProduction() =>
            GetAspNetCoreEnvironment() == EnvironmentVariables.AspNetCoreEnvironmentProductionValue;

        public static string GetAspNetCoreEnvironment() =>
            System.Environment.GetEnvironmentVariable(EnvironmentVariables.AspNetCoreEnvironment);
    }
}