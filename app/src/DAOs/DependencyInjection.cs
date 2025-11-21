using Agora.DAOs.ContentScanners.DataScanner;
using Agora.DAOs.ContentScanners.VirusScanner;
using Agora.DAOs.Esdh;
using Agora.DAOs.Esdh.Sbsip;
using Agora.DAOs.Esdh.Sbsip.V12;
using Agora.DAOs.Esdh.Sbsip.V12.Interface;
using Agora.DAOs.Esdh.Sbsip.V12.Mock;
using Agora.DAOs.Esdh.Stub;
using Agora.DAOs.ExternalProcesses.Services.ExternalPdfService;
using Agora.DAOs.Files;
using Agora.DAOs.Files.Csv;
using Agora.DAOs.Files.Excel;
using Agora.DAOs.Files.Excel.Themes;
using Agora.DAOs.Files.Pdf;
using Agora.DAOs.Files.Pdf.Themes;
using Agora.DAOs.Files.Pdf.Utils;
using Agora.DAOs.Identity;
using Agora.DAOs.KleHierarchy;
using Agora.DAOs.Mappings;
using Agora.DAOs.Messages;
using Agora.DAOs.Messages.EBoks;
using Agora.DAOs.Messages.Email;
using Agora.DAOs.Messages.RemotePrint;
using Agora.DAOs.Models;
using Agora.DAOs.OAuth2;
using Agora.DAOs.Persistence;
using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.DAOs.Security;
using Agora.DAOs.Services;
using Agora.DAOs.Statistics;
using Agora.DAOs.UserDataEnrichment.CPR;
using Agora.DAOs.UserDataEnrichment.CVR;
using Agora.DAOs.Utility;
using Agora.DAOs.Utility.CustomFormatters;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.Cpr;
using Agora.Operations.Common.Interfaces.Cvr;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.ExternalProcesses.Services;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Files.Excel;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.Interfaces.Security;
using Agora.Primitives.Logic;
using AutoMapper;
using Azure.Core;
using Azure.Identity;
using EasyCaching.Core.Configurations;
using EasyCaching.Redis;
using EFCoreSecondLevelCacheInterceptor;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using nClam;
using NovaSec;
using NovaSec.Compiler;
using NovaSec.Compiler.Resolvers;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Agora.DAOs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessObjects(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Should be added before DbContext
            services.AddEncryptionValueConverters(configuration);

            services.AddScoped<ICommandCountStatistics, CommandCountStatistics>();

            if (!configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddSecondLevelCacheInterceptor(configuration);

                // Add database context
                services.AddDbContext<ApplicationDbContext>((provider, options) =>
                {
                    string connectionString;
                    // Use entraId to retrieve password for connecting string if enabled. 
                    // Ensure the ConnectionStrings__AzureDbConnection is configured
                    var azureOptions = provider.GetService<IOptions<AzureOptions>>();
                    if (azureOptions.Value.UseAzureDbAuth)
                    {
                        var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                        {
                            ManagedIdentityClientId = azureOptions.Value.ManagedIdentityClientId
                        });

                        var tokenRequestContext = new TokenRequestContext(
                            new[] { "https://ossrdbms-aad.database.windows.net/.default" });

                        var tokenResponse = azureCredentials.GetToken(tokenRequestContext);

                        connectionString = configuration.GetConnectionString("AzureConnection") + $";Password={tokenResponse.Token}";
                    }
                    else
                    {
                        connectionString = configuration.GetConnectionString("DefaultConnection");
                    }

                    var serverVersion = ServerVersion.AutoDetect(connectionString);

                    // Default: latest version of MySQL Server
                    options
                        .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
                        .UseMySql(connectionString, serverVersion,
                        mySqlOptions =>
                        {
                            mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                            mySqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null
                            );
                        });
                    options.AddInterceptors(
                        new CommandCountInterceptor(provider.GetService<ICommandCountStatistics>()));
                    options.AddInterceptors(provider.GetRequiredService<SecondLevelCacheInterceptor>());
                });
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            // Allow injection of AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddAutoMapper(typeof(Esdh.Mappings.MappingProfile).Assembly);
            services.AddAutoMapper(typeof(KleHierarchy.Mappings.MappingProfile).Assembly);

            // Allow injection of ApplicationDbContext
            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            // Adds the basic Identity system up, and store it in the database
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Adds authentication to the application, and configures JWT Bearer tokens
            // This will set up the pipeline to check all incoming requests if they require authentication
            // The key must be signed with the correct secret, and it must not be expired
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration
                        .GetSection(JwtSettingsOptions.JwtSettings).GetSection(JwtSettingsOptions.SecretSubSection)
                        .Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var apiKey = context.Request.Headers["x-api-key"];

                        if (!Primitives.Logic.Environment.IsProduction() && string.IsNullOrEmpty(apiKey))
                        {
                            apiKey = context.Request.Query?.FirstOrDefault(x => x.Key == "apiKey").Value ??
                                     StringValues.Empty;
                        }

                        if (apiKey == StringValues.Empty)
                        {
                            return Task.CompletedTask;
                        }

                        var internalApiKey = configuration.GetSection(AuthenticationOptions.Authentication)
                            .GetValue<string>("InternalApiKey");
                        var publicApiKey = configuration.GetSection(AuthenticationOptions.Authentication)
                            .GetValue<string>("PublicApiKey");

                        if (internalApiKey == apiKey || publicApiKey == apiKey)
                        {
                            context.Token = context.Request.Cookies[$"{JWT.Cookie.AccessCookieName}.{apiKey}"];
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            // Configuration and registration of ClamClient. 
            services.AddSingleton(serviceProvider =>
            {
                var options = serviceProvider.GetService<IOptions<ClamAvOptions>>();
                var maxStreamSize = 26214400L;
                if (options != null && long.TryParse(options.Value.MaxStreamSizeInMb, out var maxStreamSizeInMb))
                {
                    maxStreamSize = maxStreamSizeInMb * 1024 * 1024;
                }

                return new ClamClient(options.Value.Server, int.Parse(options.Value.Port)) { MaxStreamSize = maxStreamSize };
            });

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddSingleton<IVirusScanner>(serviceProvider =>
            {
                var options = serviceProvider.GetService<IOptions<MicrosoftDefenderForStorageOptions>>();
                var azureOptions = serviceProvider.GetService<IOptions<AzureOptions>>();
                if (options.Value.Enabled)
                {
                    return new MicrosoftDefenderForStorage(options, azureOptions);
                }
                else
                {
                    return new VirusScanner((ClamClient)serviceProvider.GetService(typeof(ClamClient)));
                }
            });

            services.AddTransient<IDigitalPostService>(serviceProvider =>
            {
                var remotePrintOptions = serviceProvider.GetService<IOptions<RemotePrintOptions>>();
                var eboksOptions = serviceProvider.GetService<IOptions<EBoksOptions>>();

                if (!remotePrintOptions.Value.Disabled)
                {
                    var logger = serviceProvider.GetService<ILogger<RemotePrintService>>();
                    var client = serviceProvider.GetService<RemotePrintClient>();
                    var cprService = serviceProvider.GetService<ICprInformationService>();
                    var cvrService = serviceProvider.GetService<ICvrInformationService>();
                    return new RemotePrintService(client, cprService, cvrService, logger);
                }
                if (!eboksOptions.Value.Disabled)
                {
                    var client = serviceProvider.GetService<EBoksClient>();
                    return new EBoksService(client);
                }
                return new DigitalPostStubService();
            });

            services.AddSingleton<IFileService, FileService>();
            services.AddTransient<IKleService, KleService>();
            services.AddTransient<IOauth2Service, Oauth2Service>();
            services.AddTransient<ICertificateService, CertificateService>();
            services.AddCprInformationService();
            services.AddCvrInformationServiceAndClient();
            services.AddDataScanner();
            services.AddEmailService();

            services.AddTransient<ICityAreaDao, CityAreaDao>();
            services.AddTransient<ISubjectAreaDao, SubjectAreaDao>();
            services.AddTransient<IUserDao, UserDao>();
            services.AddTransient<IUserCapacityDao, UserCapacityDao>();
            services.AddTransient<IHearingStatusDao, HearingStatusDao>();
            services.AddTransient<IHearingRoleDao, HearingRoleDao>();
            services.AddTransient<IHearingDao, HearingDao>();
            services.AddTransient<IHearingTemplateDao, HearingTemplateDao>();
            services.AddTransient<IHearingTypeDao, HearingTypeDao>();
            services.AddTransient<IUserHearingRoleDao, UserHearingRoleDao>();
            services.AddTransient<IContentDao, ContentDao>();
            services.AddTransient<IContentTypeDao, ContentTypeDao>();
            services.AddTransient<ICommentDeclineInfoDao, CommentDeclineInfoDao>();
            services.AddTransient<ICommentTypeDao, CommentTypeDao>();
            services.AddTransient<ICommentStatusDao, CommentStatusDao>();
            services.AddTransient<ICommentDao, CommentDao>();
            services.AddTransient<IKleHierarchyDao, KleHierarchyDao>();
            services.AddTransient<IKleMappingDao, KleMappingDao>();
            services.AddTransient<IFieldTemplateDao, FieldTemplateDao>();
            services.AddTransient<IFieldDao, FieldDao>();
            services.AddTransient<IConsentDao, ConsentDao>();
            services.AddTransient<IGlobalContentDao, GlobalContentDao>();
            services.AddTransient<IGlobalContentTypeDao, GlobalContentTypeDao>();
            services.AddTransient<INotificationDao, NotificationDao>();
            services.AddTransient<INotificationTypeDao, NotificationTypeDao>();
            services.AddTransient<INotificationQueueDao, NotificationQueueDao>();
            services.AddTransient<ICsvService, CsvService>();
            services.AddTransient<IInvitationGroupDao, InvitationGroupDao>();
            services.AddTransient<IInvitationGroupMappingDao, InvitationGroupMappingDao>();
            services.AddTransient<IInvitationKeyDao, InvitationKeyDao>();
            services.AddTransient<IInvitationSourceDao, InvitationSourceDao>();
            services.AddTransient<IInvitationSourceMappingDao, InvitationSourceMappingDao>();
            services.AddTransient<IEventDao, EventDao>();
            services.AddTransient<IEventMappingDao, EventMappingDao>();
            services.AddTransient<INotificationContentDao, NotificationContentDao>();
            services.AddTransient<INotificationContentSpecificationDao, NotificationContentSpecificationDao>();
            services.AddTransient<INotificationContentTypeDao, NotificationContentTypeDao>();
            services.AddTransient<INotificationTemplateDao, NotificationTemplateDao>();

            services.AddSingleton<IHostingEnvironmentPath, WebHostEnvironmentPath>();
            services.AddPdfServices();
            services.AddExcelTheme();
            services.AddTransient<IExternalPdfService, ExternalPdfService>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<IRefreshTokenDao, RefreshTokenDao>();
            services.AddTransient<IJournalizeStatusDao, JournalizedStatusDao>();
            services.AddTransient<ICompanyDao, CompanyDao>();
            services.AddTransient<ICompanyHearingRoleDao, CompanyHearingRoleDao>();

            services.AddTransient<ISecurityExpressionRoot, SecurityExpressionRoot>();
            services.AddTransient<ISecurityExpressions, SecurityExpressions>();
            services.AddTransient(x =>
            {
                var securityExpressionRoot = x.GetService<ISecurityExpressionRoot>();
                var securityExpressions = x.GetService<ISecurityExpressions>();
                var injectResolver = new StaticInjectResolver(new Dictionary<string, object>
                {
                    {"Security", securityExpressions}
                });
                return new SecurityContext(securityExpressionRoot, injectResolver);
            });

            services.AddTransient<HttpLoggingDelegatingHandler>();
            services.AddHttpClientWithCircuitBreakerAndRetryPattern<TokenService>();

            var sbsipOptions = services.BuildServiceProvider().GetService<IOptions<SbsipOptions>>();
            if (!sbsipOptions.Value.Disabled)
            {
                services.AddTransient<IEsdhService, SbsipService>();
            }
            else
            {
                services.AddTransient<IEsdhService, EsdhStubService>();
            }

            var esdhOptions = services.BuildServiceProvider().GetService<IEsdhServiceOptions>();
            if (esdhOptions is { IsMocked: true })
            {
                services.AddScoped<ICaseServiceV12, CaseServiceV12Mock>();
                services.AddScoped<IUserServiceV12, UserServiceV12Mock>();
                services.AddScoped<IDocumentServiceV12, DocumentServiceV12Mock>();
            }
            else
            {
                services.AddHttpClientWithCircuitBreakerAndRetryPattern<CaseServiceV12>();
                services.AddHttpClientWithCircuitBreakerAndRetryPattern<UserServiceV12>();
                services.AddHttpClientWithCircuitBreakerAndRetryPattern<DocumentServiceV12>();

                services.AddScoped<ICaseServiceV12>(ServiceCollection => ServiceCollection.GetRequiredService<CaseServiceV12>());
                services.AddScoped<IUserServiceV12>(ServiceCollection => ServiceCollection.GetRequiredService<UserServiceV12>());
                services.AddScoped<IDocumentServiceV12>(ServiceCollection => ServiceCollection.GetRequiredService<DocumentServiceV12>());
            }

            services.AddHttpClientWithCircuitBreakerAndRetryPattern<OAuth2.OAuth2Client>();
            services.AddHttpClientWithCircuitBreakerAndRetryPattern<DataScannerClient>();
            services.AddHttpClientWithCircuitBreakerAndRetryPattern<EBoksClient>();
            services.AddHttpClientWithCircuitBreakerAndRetryPattern<RemotePrintClient>();

            return services;
        }

        public static void AddPdfServices(this IServiceCollection services)
        {
            if (MunicipalityProfile.IsCopenhagenMunicipalityProfile())
            {
                services.AddTransient<IPdfTheme, KobenhavnPdfTheme>();
            }
            else
            {
                services.AddTransient<IPdfTheme, BasePdfTheme>();
            }

            services.AddTransient<IPdfService, PdfService>();
        }

        /// <summary>
        /// Registers an implementation of the content scanner. Defaults to the DataScanner implementation
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddDataScanner(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var dataScannerOptions = serviceProvider.GetService<IOptions<DataScannerOptions>>().Value;

            if (dataScannerOptions.UseSimpleDataScanner)
            {
                services.AddTransient<IDataScanner, SimpleDataScanner>();
            }
            else
            {
                services.AddTransient<IDataScanner, DataScanner>();
            }

            return services;
        }

        private static IServiceCollection AddCprInformationService(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var cprInformationOptions = serviceProvider.GetService<IOptions<CprInformationOptions>>().Value;

            if (cprInformationOptions.Enable)
            {
                services.AddTransient<ICprInformationService, CprInformationService>();
            }
            else
            {
                services.AddTransient<ICprInformationService, CprInformationServiceStub>();
            }

            return services;
        }

        private static IServiceCollection AddCvrInformationServiceAndClient(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var cvrInformationOptions = serviceProvider.GetService<IOptions<CvrInformationOptions>>().Value;
            var azureOptions = serviceProvider.GetService<IOptions<AzureOptions>>().Value;
            var certificateService = serviceProvider.GetService<ICertificateService>();

            if (cvrInformationOptions.Enable)
            {
                // Register httpClient with client certificate
                X509Certificate2 certificate;
                if (azureOptions.IsRunningInAzure)
                {
                    certificate =
                        certificateService.GetPrivateCertificateFromKeyVault(cvrInformationOptions
                            .CertificateKeyVaultReference);
                }
                else
                {
                    certificate = certificateService.GetPrivateCertificateFromDisc(
                        cvrInformationOptions.CertificatePath, cvrInformationOptions.CertificatePassword);
                }

                services.AddTransient<ICvrInformationService, CvrInformationService>();
                services.AddHttpClientWithCircuitBreakerAndRetryPatternAndCertAuth<CvrInformationClient>(certificate);
            }
            else
            {
                services.AddTransient<ICvrInformationService, CvrInformationServiceStub>();
            }

            return services;
        }

        /// <summary>
        /// Registers the Email service. Can be configured to use either MS Graph or SMTP server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static IServiceCollection AddEmailService(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var emailOptions = serviceProvider.GetService<IOptions<EmailOptions>>().Value;
            var azureOptions = serviceProvider.GetService<IOptions<AzureOptions>>().Value;

            // Configure MS Graph Email Service
            if (azureOptions.IsRunningInAzure && emailOptions.UseMsGraph)
            {
                services.AddTransient<IEmailService, MsGraphEmailService>();
            }
            else // Use Smtp email service pr. default
            {
                services.AddTransient<IEmailService, SmtpEmailService>();
                // https://github.com/lukencode/FluentEmail
                services.AddFluentEmail(emailOptions.DefaultFromEmail, emailOptions.DefaultFromName)
                    .AddSmtpSender(() =>
                        emailOptions.UseCredentials
                            ? new SmtpClient(emailOptions.SmtpHost, emailOptions.SmtpPort)
                            { Credentials = new NetworkCredential(emailOptions.UserName, emailOptions.Password) }
                            : new SmtpClient(emailOptions.SmtpHost, emailOptions.SmtpPort));

            }

            return services;
        }

        /*
        * What is going on here?
        * We are setting up 3 policies which are chained. A Retry pattern wrapping a Circuit breaker with a Timeout inside of it.
        * This will wrap the HttpClient and make sure no calls takes longer than 3*15 seconds.
        * If we see that 50% of the calls during a 30 seconds period fails, we will open the circuit for 30 seconds.
        * After a cooldown period of 30 seconds, we will half-open the circuit and allow a single call to go through.
        * If that succeeds we close the circuit. If it fails we open the circuit again for another 30 seconds.
        */
        static void AddHttpClientWithCircuitBreakerAndRetryPattern<T>(this IServiceCollection services) where T : class
        {
            services.AddHttpClient<T>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy())
                .AddPolicyHandler(GetTimeoutPolicy())
                .AddHttpMessageHandler<HttpLoggingDelegatingHandler>();
        }

        static void AddHttpClientWithCircuitBreakerAndRetryPatternAndCertAuth<T>(this IServiceCollection services, X509Certificate2 certificate) where T : class
        {
            services.AddHttpClient<T>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ClientCertificates = { certificate }
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy())
                .AddPolicyHandler(GetTimeoutPolicy())
                .AddHttpMessageHandler<HttpLoggingDelegatingHandler>();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(3);
        }

        static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(15);
        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(30),
                    minimumThroughput: 10,
                    durationOfBreak: TimeSpan.FromSeconds(30));
        }

        private static IServiceCollection AddSecondLevelCacheInterceptor(this IServiceCollection services, IConfiguration configuration)
        {
            const string easyCachingProviderName = "EasyCachingProvider";
            var expirationTimeoutMinutes = configuration.GetSection("Cache:EFSecondLevel")
                .GetValue<int>("ExpirationTimeoutMinutes");

            services.AddEFSecondLevelCache(options => options
                .UseEasyCachingCoreProvider(easyCachingProviderName, isHybridCache: false)
                .DisableLogging(true)
                .UseCacheKeyPrefix("EF_")
                .CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(expirationTimeoutMinutes))
            );

            if (configuration.GetValue<bool>("Cache:UseRedis"))
            {
                const string serializerName = "redisSerializer";

                return services.AddEasyCaching(options => options.UseRedis(config =>
                {
                    config.MaxRdSecond = configuration.GetValue<int>("Cache:RedisEasyCaching:MaxRdSecond");
                    config.EnableLogging = configuration.GetValue<bool>("Cache:RedisEasyCaching:EnableLogging");
                    config.LockMs = configuration.GetValue<int>("Cache:RedisEasyCaching:LockMs");
                    config.SleepMs = configuration.GetValue<int>("Cache:RedisEasyCaching:SleepMs");
                    config.SerializerName = serializerName;

                    config.DBConfig = new RedisDBOptions
                    {
                        IsSsl = configuration.GetValue<bool>("Cache:RedisEasyCaching:DBConfig:IsSsl"),
                        ConnectionTimeout = configuration.GetValue<int>("Cache:RedisEasyCaching:DBConfig:ConnectionTimeout"),
                        Password = configuration.GetValue<string>("Cache:RedisEasyCaching:DBConfig:Password"),
                        AbortOnConnectFail = configuration.GetValue<bool>("Cache:RedisEasyCaching:DBConfig:AbortOnConnectFail"),
                        Endpoints = { new ServerEndPoint(
                            configuration.GetValue<string>("Cache:RedisEasyCaching:DBConfig:Host"),
                            configuration.GetValue<int>("Cache:RedisEasyCaching:DBConfig:Port")
                            )
                        }
                    };
                }, easyCachingProviderName).WithMessagePack(so =>
                    {
                        so.EnableCustomResolver = true;
                        so.CustomResolvers = CompositeResolver.Create(
                            new IMessagePackFormatter[]
                            {
                                DbNullFormatter.Instance
                            },
                            new IFormatterResolver[]
                            {
                                NativeDateTimeResolver.Instance,
                                ContractlessStandardResolver.Instance,
                                StandardResolverAllowPrivate.Instance,
                            });
                    }, serializerName));
            }


            return services.AddEasyCaching(options =>
                options.UseInMemory(configuration, easyCachingProviderName, "Cache:InMemoryEasyCaching")
            );
        }

        private static void AddEncryptionValueConverters(this IServiceCollection services, IConfiguration configuration)
        {
            var appOptions = configuration.GetSection("App").Get<AppOptions>();
            if (appOptions.DisableDatabaseEncryption)
            {
                services.AddSingleton<IEncryptionValueConverterFactory, DisabledEncryptionValueConverterFactory>();
            }
            else
            {
                services.AddDataProtection().SetApplicationName("/app");
                services.AddSingleton<IEncryptionValueConverterFactory, EncryptionValueConverterFactory>();
            }
        }

        private static void AddExcelTheme(this IServiceCollection services)
        {
            if (MunicipalityProfile.IsBallerupMunicipalityProfile())
            {
                services.AddTransient<IExcelTheme, BallerupExcelTheme>();
            }
            else
            {
                services.AddTransient<IExcelTheme, BaseExcelTheme>();
            }
        }
    }
}