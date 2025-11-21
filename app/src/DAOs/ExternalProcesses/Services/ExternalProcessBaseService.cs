using Agora.DAOs.ExternalProcesses.Interfaces;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Telemetry;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.DAOs.ExternalProcesses.Services;

public abstract class ExternalProcessBaseService<T>
{
    protected readonly ILogger<T> _logger;
    protected readonly IFileService _fileService;

    private readonly string _executablePath;
    private readonly string _argumentsFormat;

    protected string TempFolder;
    protected string ConfigurationPath;

    protected ExternalProcessBaseService(ILogger<T> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        // We have to execute the external processes differently, depending on the OS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _executablePath = Path.Combine(baseDir, "Agora.ExternalProcessHandler.exe");
            _argumentsFormat = "\"{0}\"";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var handlerPath = Path.Combine(baseDir, "Agora.ExternalProcessHandler.dll");
            _executablePath = "dotnet";
            _argumentsFormat = $"\"{handlerPath}\" \"{{0}}\"";
        }
        else
        {
            throw new PlatformNotSupportedException("Operating System not supported for external process execution.");
        }
    }

    protected async Task ExecuteExternalProcess(string tempFolder, IExternalProcessConfiguration configuration)
    {
        TempFolder = tempFolder;
        if (!Directory.Exists(TempFolder))
        {
            Directory.CreateDirectory(TempFolder);
        }

        ConfigurationPath = Path.Combine(TempFolder, $"{Guid.NewGuid()}.json");
        var configurationJson = JsonConvert.SerializeObject(configuration);

        await File.WriteAllTextAsync(ConfigurationPath, configurationJson);

        var arguments = string.Format(_argumentsFormat, ConfigurationPath);

        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException($"Failed to start External process: '{configuration.Job}'");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var errorOutput = await process.StandardOutput.ReadToEndAsync();
            _logger.LogError("External process failed with code {Code}. Output: {Output}", process.ExitCode, errorOutput);
            throw new InvalidOperationException($"External process {configuration.Job} failed (Exit Code: {process.ExitCode}).");
        }
    }

    protected void CleanUp()
    {
        if (File.Exists(ConfigurationPath)) File.Delete(ConfigurationPath);
        if (Directory.Exists(TempFolder)) Directory.Delete(TempFolder);
    }

    protected static Activity StartServiceActivity(string methodName, Dictionary<string, string> tags = null)
    {
        var activity = Instrumentation.Source.StartActivity($"External process: {typeof(T).Name}.{methodName}");

        if (tags == null)
        {
            return activity;
        }

        foreach (var tag in tags)
        {
            activity.SetTag(tag.Key, tag.Value);
        }

        return activity;
    }
}