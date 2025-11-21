using Agora.DAOs.Esdh;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.ApplicationOptions.OperationsOptions;
using Agora.Operations.Common.Behaviours;
using Agora.Operations.Common.CustomRequests.Validators;
using Agora.Operations.Common.FilterAndSorting.Factories;
using Agora.Operations.Common.FilterAndSorting.Handlers;
using Agora.Operations.Common.FilterAndSorting.PropertyFilters.Hearing;
using Agora.Operations.Common.FilterAndSorting.PropertySortings.Hearing;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.Filters;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Common.Interfaces.Services;
using Agora.Operations.Common.Interfaces.Sorting;
using Agora.Operations.Common.Utility.RedLock;
using Agora.Operations.Models.Fields.Commands.UpdateFields;
using Agora.Operations.Plugins.Plugins;
using Agora.Operations.Plugins.Service;
using Agora.Operations.Resolvers;
using Agora.Operations.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Collections.Generic;
using System.Net;
using Agora.Operations.Common.Interfaces.Notifications;
using Agora.Operations.Services.Notifications;

namespace Agora.Operations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOperations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.Configure<ClamAvOptions>(options => configuration.GetSection(ClamAvOptions.ClamAv).Bind(options));
            services.Configure<MicrosoftDefenderForStorageOptions>(options => configuration.GetSection(MicrosoftDefenderForStorageOptions.MicrosoftDefenderForStorage).Bind(options));
            services.Configure<JwtSettingsOptions>(options => configuration.GetSection(JwtSettingsOptions.JwtSettings).Bind(options));
            services.Configure<FileDrivesOptions>(options => configuration.GetSection(FileDrivesOptions.FileDrives).Bind(options));
            services.Configure<SbsipOptions>(options => configuration.GetSection(SbsipOptions.Sbsip).Bind(options));
            services.Configure<PluginOptions>(options => configuration.GetSection(PluginOptions.Plugin).Bind(options));
            services.Configure<OAuth2Options>(options => configuration.GetSection(OAuth2Options.OAuth2).Bind(options));
            services.Configure<AppOptions>(options => configuration.GetSection(AppOptions.App).Bind(options));
            services.Configure<DataScannerOptions>(options => configuration.GetSection(DataScannerOptions.DataScanner).Bind(options));
            services.Configure<EBoksOptions>(options => configuration.GetSection(EBoksOptions.EBoks).Bind(options));
            services.Configure<EmailOptions>(options => configuration.GetSection(EmailOptions.Email).Bind(options));
            services.Configure<JaegerOptions>(options => configuration.GetSection(JaegerOptions.Jaeger).Bind(options));
            services.Configure<AzureOptions>(options => configuration.GetSection(AzureOptions.Azure).Bind(options));
            services.Configure<AuthenticationOptions>(options =>
                configuration.GetSection(AuthenticationOptions.Authentication).Bind(options));
            services.Configure<RemotePrintOptions>(options =>
                configuration.GetSection(RemotePrintOptions.RemotePrint).Bind(options));
            services.Configure<CprInformationOptions>(options =>
                configuration.GetSection(CprInformationOptions.CprInformation).Bind(options));
            services.Configure<CvrInformationOptions>(options =>
                configuration.GetSection(CvrInformationOptions.CvrInformation).Bind(options));
            services.Configure<SecurityOptions>(options =>
                configuration.GetSection(SecurityOptions.Security).Bind(options));
            services.Configure<EntraIdOptions>(options =>
                configuration.GetSection(EntraIdOptions.EntraId).Bind(options));

            // Operation Options
            services.ConfigureOperationOptions(configuration);

            // Filters
            services.AddScoped<IPropertyFilter<Hearing>, TitleFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, SubjectAreaFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, CityAreaFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, HearingTypeFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, HearingStatusFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, UserHearingRoleFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, CompanyHearingRoleFilter>();
            services.AddScoped<IPropertyFilter<Hearing>, HearingOwnerFilter>();

            // Sortings
            services.AddScoped<IPropertySorting<Hearing>, TitleSorting>();
            services.AddScoped<IPropertySorting<Hearing>, DeadlineSorting>();
            services.AddScoped<IPropertySorting<Hearing>, StartDateSorting>();

            //Validators
            services.AddScoped<IPaginationValidator, PaginationValidator>();
            services.AddScoped<ISortingValidator, SortingValidator>();
            services.AddScoped<IFilterValidator, FilterValidator>();

            //FilterHandlers
            services.AddScoped<IFilterHandler<List<Hearing>>, FilterHandler<Hearing>>();
            services.AddScoped<ISortingHandler<List<Hearing>>, SortingHandler<Hearing>>();

            services.AddScoped<IFilterHandlerFactory, FilterHandlerFactory>();
            services.AddScoped<ISortingHandlerFactory, SortingHandlerFactory>();

            // MediatR behaviours
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetryBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PaginationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SortingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SecurityBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FilterBehaviour<,>));

            services.AddTransient<IPluginService, PluginService>();
            services.AddTransient<IFieldsValidator, FieldsValidator>();

            services.AddTransient<IInvitationService, InvitationService>();
            services.AddTransient<IInvitationHandler, InvitationHandler>();

            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<INotificationBuilder, BaseNotificationBuilder>();

            services.AddScoped<IFieldSystemResolver, FieldSystemResolver>();
            services.AddScoped<IHearingRoleResolver, HearingRoleResolver>();
            services.AddScoped<IHearingAccessResolver, HearingAccessResolver>();
            services.AddScoped<IUserHearingRoleResolver, UserHearingRoleResolver>();
            services.AddScoped<ICompanyHearingRoleResolver, CompanyHearingRoleResolver>();
            services.AddSingleton<ITextResolver, TextResolver>();
            services.AddPluginOptions();

            services.AddRedLockFactory(configuration);
            
            
            return services;
        }

        public static void AddPluginOptions(this IServiceCollection services)
        {
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
        }

        public static IServiceCollection AddRedLockFactory(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetSection("RedLock").GetValue<bool>("Enabled"))
            {
                services.AddSingleton<IDistributedLockFactory>(serviceProvider =>
                {
                    var dbConnection = configuration.GetSection("RedLock:DBConfig");
                    var endpoints = new List<RedLockEndPoint>();
                    var logger = serviceProvider.GetRequiredService<ILoggerFactory>();

                    endpoints.Add(new RedLockEndPoint
                    {
                        EndPoint = new DnsEndPoint(dbConnection.GetValue<string>("Host"),
                            dbConnection.GetValue<int>("Port")),
                        Password = dbConnection.GetValue<string>("Password"),
                        Ssl = dbConnection.GetValue<bool>("IsSsl")
                    });

                    return RedLockFactory.Create(endpoints, logger);
                });
            }
            else
            {
                services.AddSingleton<IDistributedLockFactory, RedLockFactoryStub>();
            }
            return services;
        }
    }
}
