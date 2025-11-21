using Agora.Api.Configuration;
using Agora.Api.Filters;
using Agora.Api.Models.Elmah;
using Agora.Api.Services;
using Agora.Api.Services.AuthenticationHandlers;
using Agora.Api.Services.ClaimsServices;
using Agora.Api.Services.Interfaces;
using Agora.DAOs;
using Agora.DAOs.Persistence;
using Agora.DTOs;
using Agora.Operations;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Interfaces;
using ElmahCore.Mvc;
using FluentValidation.AspNetCore;
using Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Linq;
using System.Reflection;
using Agora.Api.Middleware;
using OpenTelemetry.Metrics;

namespace Agora.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Initialize service registries for Operation and DAO projects.
            services.AddOperations(Configuration);
            services.AddDataAccessObjects(Configuration);
            services.AddDataTransferObjects(Configuration);
            
            var appOptions = Configuration.GetSection("App").Get<AppOptions>();
            if (!appOptions.DisableBackgroundJobs)
            {
                services.AddJobs(Configuration);
            }
            
            services.AddApiDependencies();

            services.AddNemLogin3SamlConfiguration(Configuration);

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<ICookieService, CookieService>();
            services.AddSingleton<IAuthenticationClientService, AuthenticationClientService>();

            services.AddTransient<IClaimsService<CodeFlowAuthenticationHandler>, CodeFlowClaimsService>();
            services.AddTransient<IAuthenticationHandler, CodeFlowAuthenticationHandler>();

            if (Configuration.GetValue<bool>("Saml2:Enable"))
            {
                services.AddTransient<IClaimsService<NemLoginAuthenticationHandler>, NemLoginClaimsService>();
                services.AddTransient<IAuthenticationHandler, NemLoginAuthenticationHandler>();
            }
            if (Configuration.GetValue<bool>("EntraId:Enable"))
            {
                services.AddTransient<IClaimsService<EntraIdAuthenticationHandler>, EntraIdClaimsService>();
                services.AddTransient<IAuthenticationHandler, EntraIdAuthenticationHandler>();
            }
            services.AddTransient<IAuthenticationHandlerFactory, AuthenticationHandlerFactory>();

            // Allow use of HttpContext
            services.AddHttpContextAccessor();

            // Allow use of HttpContext.Session
            if (Configuration.GetValue<bool>("Session:UseRedis"))
            {
                // A distributed session cache used for multi-server scenarios 
                services.AddStackExchangeRedisCache(config =>
                {
                    config.Configuration = Configuration.GetConnectionString("RedisConnection");
                    config.InstanceName = "Session_";
                });
            }
            else
            {
                // An in-memory cache that is not distributed. Used for single-server scenarios
                services.AddDistributedMemoryCache();
            }
            services.AddSession();

            services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

            services.AddCors();

            // AddJsonApi will replace all input and output formatters for NewtonsoftJson
            // This have the effect that the Api layer will be able to 
            services
                .AddControllers(options =>
                {
                    options.Filters.Add<ApiExceptionFilterAttribute>();
                })
                .AddFluentValidation()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .AddJsonApi();

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Hearingportal API";
                configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });
                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                configure.OperationProcessors.Add(new XApiKeyHeaderParameter());
            });

            services.AddElmah<ElmahMemoryErrorLog>();
            
            string openTelemetryServiceName = Assembly.GetExecutingAssembly().GetName().Name!;
            var jaegerOptions = Configuration.GetSection(JaegerOptions.Jaeger).Get<JaegerOptions>();

            services.AddOpenTelemetry().WithTracing(
                builder => builder
                    .AddSource(Telemetry.ActivitySourceName)
                    .ConfigureResource(resource =>
                    {
                        resource.AddService(serviceName: openTelemetryServiceName);
                    })
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                    })
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = jaegerOptions.Endpoint;
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    })
            );

            if (jaegerOptions.EnableRuntimeMetrics)
            {
                services.AddOpenTelemetry().WithMetrics(
                    builder => builder
                        .ConfigureResource(resource =>
                        {
                            resource.AddService(serviceName: openTelemetryServiceName);
                        })
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = jaegerOptions.Endpoint;
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        })
                        .AddConsoleExporter()
                );
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Primitives.Logic.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }

            app.UseHealthChecks("/health");
            app.UseOpenApi();
            app.UseSwaggerUi(settings =>
            {
                settings.Path = "/swagger";
                settings.DocumentPath = "/api/specification.json";
            });

            app.UseElmah();

            // Allow Swagger to see the specification file
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseSession();

            // Open for CORS when developing locally
            app.UseCors(builder => builder
                .WithOrigins("http://localhost:3000", "http://localhost:4000", "http://localhost:4001")
                // Allow '*' methods (PUT, POST, GET, etc)
                .AllowAnyMethod()
                // Allow all headers to be send with the request
                .AllowAnyHeader()
                // Allows the client to send credentials in either a cookie or HTTP authentication scheme
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            // custom logging middlewares must be after authentication
            app.UseMiddleware<UserDataLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (Primitives.Logic.Environment.IsDevelopment())
                {
                    endpoints.MapGet("/debug-config", ctx =>
                    {
                        var config = (Configuration as IConfigurationRoot).GetDebugView();
                        return ctx.Response.WriteAsync(config);
                    });
                }
            });
        }
    }
}