using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agora.Operations.ApplicationOptions.OperationsOptions
{
    public static class OperationsConfigurations
    {
        private const string BaseConfigurationSection = "Operations";


        private static string GetOperationConfigurationSection(string operation)
        {
            return $"{BaseConfigurationSection}:{operation}";
        }

        private static IServiceCollection ConfigureOperationOption<T>(this IServiceCollection services,
            IConfiguration configuration, string sectionName) where T : class
        {
            return services.Configure<T>(options =>
                configuration.GetSection(GetOperationConfigurationSection(sectionName))
                    .Bind(options));
        }

        public static IServiceCollection ConfigureOperationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .ConfigureOperationOption<CommentOperationsOptions>(configuration, CommentOperationsOptions.Comments)
                .ConfigureOperationOption<HearingOperationsOptions>(configuration, HearingOperationsOptions.Hearings)
                .ConfigureOperationOption<NotificationQueueOperationsOptions>(configuration, NotificationQueueOperationsOptions.NotificationQueues);
        }
    }
}
