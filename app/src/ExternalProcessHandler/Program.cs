using Agora.DAOs.ExternalProcesses.Enums;
using Agora.DAOs.ExternalProcesses.Interfaces;
using Agora.DAOs.ExternalProcesses.Services.ExternalPdfService;
using Agora.ExternalProcessHandler.Interfaces;
using Agora.ExternalProcessHandler.Jobs;
using Agora.ExternalProcessHandler.Models;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agora.ExternalProcessHandler
{
    public class Program
    {
        private const string DockerSecretsDir = "/run/secrets";
        private const string UserSecretsAssemblyName = "Agora.Api";

        private static readonly Dictionary<ExternalProcessJobs, ExternalProcess> ExternalProcessMap = new()
        {
            { ExternalProcessJobs.GENERATE_PDF, 
                new ExternalProcess
                {
                    Job = typeof(GeneratePdfJob),
                    Config = typeof(GeneratePdfConfiguration)
                }
            }
        };

        public static int Main(string[] args)
        {
            var services = InitializeServiceCollection();
            using var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            // If program is run directly, /no-monitor should be given as argument, so we do not listen to parent process, as it does not exist
            var noMonitorMode = args.Any(a => a.Equals("/no-monitor", StringComparison.OrdinalIgnoreCase));

            if (!noMonitorMode)
            {
                Task.Run(() => MonitorParentProcess(logger));
            }

            var requiredArgs = args.Where(a => !a.Equals("/no-monitor", StringComparison.OrdinalIgnoreCase)).ToArray();

            if (requiredArgs.Length == 0)
            {
                logger.LogError("No input arguments provided. ConfigurationPath must be provided");
                return 1;
            }

            var configurationPath = requiredArgs[0];

            try
            {
                if (!File.Exists(configurationPath))
                {
                    throw new FileNotFoundException($"Configuration file not found at path: {configurationPath}");
                }

                var jsonConfig = File.ReadAllText(configurationPath);
                var processConfig = JsonSerializer.Deserialize<ExternalProcessConfiguration>(jsonConfig);

                if (!ExternalProcessMap.TryGetValue(processConfig.Job, out var externalProcess))
                {
                    throw new NotSupportedException($"Job type {processConfig.Job} is not supported");
                }

                var configuration = BuildConfigurations();

                var job = (IExternalJob)Activator.CreateInstance(externalProcess.Job);
                var jobConfig = (IExternalProcessConfiguration)JsonSerializer.Deserialize(jsonConfig, externalProcess.Config);

                job.ConfigureServices(services, configuration, jobConfig);

                var serviceProvider = services.BuildServiceProvider();

                return job.Handle(jobConfig, serviceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "External Process Handler failed executing job.");
                return 2;
            }
        }

        private static IServiceCollection InitializeServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());

            return services;
        }

        private static IConfiguration BuildConfigurations()
        {
            DotEnv.Load(options: new DotEnvOptions(
                envFilePaths: new[] { ".env.local", "../.env.local", "../../.env.local" },
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
                .AddUserSecrets(UserSecretsAssemblyName, true)
                .AddEnvironmentVariables()
                .AddKeyPerFile(DockerSecretsDir, true);

            return configurationBuilder.Build();
        }

        private static void MonitorParentProcess(ILogger logger)
        {
            try
            {
                // Receives "input-stream" from parent process
                Console.In.Peek();
                // This blocks the thread by "waiting" for input from parent process
                Console.In.ReadLine();

                // If parent process is closed, the "input-stream" will be closed, and the thread is no longer "blocked"
                logger.LogWarning("Parent process Standard Input stream closed. Terminating external process.");
            }
            catch (IOException)
            {
                logger.LogWarning("IO Exception while monitoring parent process. Terminating.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in parent process monitor. Terminating.");
            }

            Environment.Exit(3);
        }
    }
}