using AutoMapper;
using Agora.DTOs.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agora.DTOs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataTransferObjects(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}