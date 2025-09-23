using AutoMapper;
using BallerupKommune.DTOs.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BallerupKommune.DTOs
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