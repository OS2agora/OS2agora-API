using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Agora.DAOs.ExternalProcesses.Interfaces;

namespace Agora.ExternalProcessHandler.Interfaces
{
    public interface IExternalJob
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration, IExternalProcessConfiguration jobConfiguration);
        int Handle(IExternalProcessConfiguration jobConfiguration, IServiceProvider serviceProvider);
    }
}

