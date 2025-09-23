using AutoMapper;
using BallerupKommune.Api.Configuration;
using BallerupKommune.Api.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;

namespace BallerupKommune.Api
{
    public static class DependencyInjection
    {
        // Shamelessly taken from this issue: https://github.com/codecutout/JsonApiSerializer/issues/115
        // AddJsonApi will insert a new output formatter for NewtonsoftJson
        // This have the effect that the Api layer will be able to send JsonApi from a simple POCO DTO
        public static IMvcBuilder AddJsonApi(this IMvcBuilder builder, Action<MvcNewtonsoftJsonOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, ConfigureMvcOptionsForJsonApi>());
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptionsForJsonApi>();
            builder.Services.Configure(setupAction ?? (options => { }));

            return builder;
        }

        public static IServiceCollection AddApiDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}