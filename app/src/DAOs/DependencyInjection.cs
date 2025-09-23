using AutoMapper;
using BallerupKommune.DAOs.ContentScanners.DataScanner;
using BallerupKommune.DAOs.ContentScanners.VirusScanner;
using BallerupKommune.DAOs.Esdh.Sbsip;
using BallerupKommune.DAOs.Esdh.Sbsip.V12;
using BallerupKommune.DAOs.Files;
using BallerupKommune.DAOs.Files.Pdf;
using BallerupKommune.DAOs.Identity;
using BallerupKommune.DAOs.KleHierarchy;
using BallerupKommune.DAOs.Messages;
using BallerupKommune.DAOs.Messages.EBoks;
using BallerupKommune.DAOs.Models;
using BallerupKommune.DAOs.OAuth2;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DAOs.Security;
using BallerupKommune.DAOs.Services;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using nClam;
using NovaSec;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Statistics;
using BallerupKommune.DAOs.Utility;
using Microsoft.AspNetCore.DataProtection;
using Polly.Timeout;
using BallerupKommune.DAOs.Esdh.Sbsip.V12.Interface;
using BallerupKommune.DAOs.Esdh.Sbsip.V12.Mock;
using EFCoreSecondLevelCacheInterceptor;
using BallerupKommune.DAOs.Mappings;
using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using Microsoft.Extensions.Logging;
using BallerupKommune.DAOs.Esdh;
using BallerupKommune.DAOs.Notifications;
using NovaSec.Compiler;
using NovaSec.Compiler.Resolvers;

namespace BallerupKommune.DAOs
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

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var serverVersion = ServerVersion.AutoDetect(connectionString);
                // Add database context
                services.AddDbContext<ApplicationDbContext>((provider, options) =>
                {
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
                    RequireExpirationTime = false,
                    ClockSkew = TimeSpan.FromMinutes(2)
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

                        var internalApiKey = configuration.GetSection(OAuth2Options.OAuth2)
                            .GetValue<string>("InternalApiKey");
                        var publicApiKey = configuration.GetSection(OAuth2Options.OAuth2)
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
                return new ClamClient(options.Value.Server, int.Parse(options.Value.Port)){ MaxStreamSize = maxStreamSize };
            });

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddSingleton<IVirusScanner, VirusScanner>();
            services.AddSingleton<IFileService, FileService>();
            services.AddTransient<IKleService, KleService>();
            services.AddTransient<IOauth2Service, Oauth2Service>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEBoksService, EBoksService>();
            services.AddTransient<IDataScanner, DataScanner>();
            services.AddTransient<INotificationContentBuilder, NotificationContentBuilder>();

            services.AddTransient<ISubjectAreaDao, SubjectAreaDao>();
            services.AddTransient<IValidationRuleDao, ValidationRuleDao>();
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
            services.AddTransient<ICommentTypeDao, CommentTypeDao>();
            services.AddTransient<ICommentStatusDao, CommentStatusDao>();
            services.AddTransient<ICommentDao, CommentDao>();
            services.AddTransient<ICommentDeclineInfoDao, CommentDeclineInfoDao>();
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
            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IEsdhService, SbsipService>();
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

            // https://github.com/lukencode/FluentEmail
            var emailOptions = configuration.GetSection(EmailOptions.Email).Get<EmailOptions>();

            services.AddFluentEmail(emailOptions.DefaultFromEmail, emailOptions.DefaultFromName)
                .AddSmtpSender(() =>
                    emailOptions.UseCredentials
                        ? new SmtpClient(emailOptions.SmtpHost, emailOptions.SmtpPort)
                        { Credentials = new NetworkCredential(emailOptions.UserName, emailOptions.Password) }
                        : new SmtpClient(emailOptions.SmtpHost, emailOptions.SmtpPort));
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
    }
}