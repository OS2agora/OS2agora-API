using Agora.DAOs.Identity;
using Agora.DAOs.Persistence;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces;
using AutoMapper;
using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Environment = System.Environment;

namespace Agora.Api
{
    public class Program
    {
        private const string DockerSecretsDir = "/run/secrets";

        public static async Task Main(string[] args)
        {
            // Create configuration
            DotEnv.Load(options: new DotEnvOptions(
                envFilePaths: new []{ ".env.local", "../.env.local", "../../.env.local" }, 
                overwriteExistingVars: false));
            
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            string appSettingsProfile = Environment.GetEnvironmentVariable(Primitives.Constants.EnvironmentVariables.AppSettingsProfile);
            if (appSettingsProfile != null)
            {
                // is optional because it can be injected through environment variables instead of appsettings file
                configurationBuilder.AddJsonFile($"appsettings.{appSettingsProfile}.json", optional: true);
            }
            
            configurationBuilder
                .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                .AddEnvironmentVariables()
                .AddKeyPerFile(DockerSecretsDir, true)
                .AddCommandLine(args);
            IConfigurationRoot configuration = configurationBuilder.Build();

            // create logger
            var consoleOutputTemplate = configuration.GetValue<string>("App:Logging:ConsoleOutputTemplate");
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console(outputTemplate: consoleOutputTemplate);

            var disableLogEnrichment = configuration.GetValue<bool>("App:Logging:DisableLogEnrichment");

            if (!disableLogEnrichment)
            {
                loggerConfig.Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithThreadId();
            }

            Log.Logger = loggerConfig.CreateLogger();

            try
            {
                Log.Information("Starting application");
                var host = CreateHostBuilderWithConfig(args, configuration).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        var appOptions = services.GetRequiredService<IOptions<AppOptions>>();
                        var shouldUseDevelopmentDatabaseSeed = appOptions.Value.UseDevelopmentDatabase;
                        var ensureRequiredDataExist = appOptions.Value.EnsureRequiredDataExist;

                        if (context.Database.IsMySql())
                        {
                            await context.Database.MigrateAsync();
                        }

                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var kleService = services.GetRequiredService<IKleService>();
                        var mapper = services.GetRequiredService<IMapper>();

                        await ApplicationDbContextSeed.SeedDefaultRolesAsync(roleManager);
                        await ApplicationDbContextSeed.SeedDefaultDataAsync(context, kleService, mapper);

                        if (shouldUseDevelopmentDatabaseSeed)
                        {
                            await ApplicationDbContextSeed.SeedDefaultUsers(userManager);
                            await ApplicationDbContextSeed.SeedSampleDataAsync(context, userManager, kleService, mapper);
                        }

                        if (ensureRequiredDataExist)
                        {
                            await ApplicationDbContextEnsureRequiredData.SeedRequiredData(context);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while migrating or seeding the database.");

                        throw;
                    }
                }

                await host.RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // This overload exists because NSwag wants has demands in order to create the swagger spec. 
        public static IHostBuilder CreateHostBuilder(string[] args) => CreateHostBuilderWithConfig(args, null);

        // dotnet entity framework can't create migration if multiple overloads have the same name
        // https://github.com/dotnet/aspnetcore/issues/21658
        public static IHostBuilder CreateHostBuilderWithConfig(string[] args, IConfigurationRoot configuration) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(builder =>
                {
                    if (configuration != null)
                    {
                        builder.AddConfiguration(configuration);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                            if (configuration != null)
                            {
                                options.Limits.MaxRequestBodySize = long.Parse(configuration["Kestrel:Limits:MaxRequestBodySize"]);
                            }
                        });
                });
    }
}