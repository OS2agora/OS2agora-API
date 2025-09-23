using BallerupKommune.Api.Filters;
using BallerupKommune.Api.Services;
using BallerupKommune.DAOs;
using BallerupKommune.DAOs.Persistence;
using BallerupKommune.DTOs;
using BallerupKommune.Operations;
using BallerupKommune.Operations.Common.Interfaces;
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
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Linq;
using System.Reflection;
using BallerupKommune.Api.Configuration;
using BallerupKommune.Api.Models.Elmah;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Constants;
using ElmahCore.Mvc;
using OpenTelemetry.Exporter;

namespace BallerupKommune.Api
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

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<ICookieService, CookieService>();
            services.AddSingleton<IAuthenticationClientService, AuthenticationClientService>();

            // Allow use of HttpContext
            services.AddHttpContextAccessor();

            // Allow use of HttpContext.Session
            services.AddDistributedMemoryCache();
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
                .WithOrigins("http://localhost:4000", "http://localhost:3000")
                // Allow '*' methods (PUT, POST, GET, etc)
                .AllowAnyMethod()
                // Allow all headers to be send with the request
                .AllowAnyHeader()
                // Allows the client to send credentials in either a cookie or HTTP authentication scheme
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

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