namespace Agora.Primitives.Constants
{
    public static class EnvironmentVariables
    {
        public static readonly string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        public static readonly string AppSettingsProfile = "APPSETTINGS_PROFILE";
        public static readonly string RunningInDocker = "RUNNING_IN_DOCKER";

        public static readonly string AspNetCoreEnvironmentDevelopmentValue = "Development";
        public static readonly string AspNetCoreEnvironmentProductionValue = "Production";

        public static readonly string RunningInDockerTrueValue = "yes";
    }
}
