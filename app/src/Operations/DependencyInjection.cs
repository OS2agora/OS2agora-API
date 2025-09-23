using BallerupKommune.Models.Models;
using BallerupKommune.DAOs.Esdh;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Behaviours;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using BallerupKommune.Operations.Models.Fields.Commands.UpdateFields;
using BallerupKommune.Operations.Plugins.Plugins;
using BallerupKommune.Operations.Plugins.Service;
using BallerupKommune.Operations.Resolvers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BallerupKommune.Operations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOperations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.Configure<ClamAvOptions>(options => configuration.GetSection(ClamAvOptions.ClamAv).Bind(options));
            services.Configure<JwtSettingsOptions>(options => configuration.GetSection(JwtSettingsOptions.JwtSettings).Bind(options));
            services.Configure<IdPOptions>(options => configuration.GetSection(IdPOptions.IdP).Bind(options));
            services.Configure<FileDriveOptions>(options => configuration.GetSection(FileDriveOptions.FileDrive).Bind(options));
            services.Configure<SbsipOptions>(options => configuration.GetSection(SbsipOptions.Sbsip).Bind(options));
            services.Configure<PluginOptions>(options => configuration.GetSection(PluginOptions.Plugin).Bind(options));
            services.Configure<OAuth2Options>(options => configuration.GetSection(OAuth2Options.OAuth2).Bind(options));
            services.Configure<AppOptions>(options => configuration.GetSection(AppOptions.App).Bind(options));
            services.Configure<DataScannerOptions>(options => configuration.GetSection(DataScannerOptions.DataScanner).Bind(options));
            services.Configure<EBoksOptions>(options => configuration.GetSection(EBoksOptions.EBoks).Bind(options));
            services.Configure<EmailOptions>(options => configuration.GetSection(EmailOptions.Email).Bind(options));
            services.Configure<JaegerOptions>(options => configuration.GetSection(JaegerOptions.Jaeger).Bind(options));

            // MediatR behaviours
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetryBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SecurityBehaviour<,>));

            services.AddTransient<IPluginService, PluginService>();
            services.AddTransient<IFieldsValidator, FieldsValidator>();
            services.AddScoped<IFieldSystemResolver, FieldSystemResolver>();

            services.AddScoped<IHearingRoleResolver, HearingRoleResolver>();

            services.AddScoped<IHearingAccessResolver, HearingAccessResolver>();

            services.AddScoped<IUserHearingRoleResolver, UserHearingRoleResolver>();

            services.AddScoped<ICompanyHearingRoleResolver, CompanyHearingRoleResolver>();

            // This section exists for the esdh service to have capability to mock it's function by overriding which Daos will be applied after this injection.
            var pluginOptions = services.BuildServiceProvider().GetService<IOptions<PluginOptions>>();
            if (pluginOptions != null && pluginOptions.Value.Configurations.ContainsKey(nameof(EsdhPlugin)))
            {
                var esdhConfig = pluginOptions.Value.Configurations[nameof(EsdhPlugin)];
                if (esdhConfig != null)
                {
                    services.AddTransient<IEsdhServiceOptions>(s => new EsdhServiceOptions(esdhConfig.Mocked));
                }
            }
            
            return services;
        }
    }
}
