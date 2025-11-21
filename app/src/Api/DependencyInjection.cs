using Agora.Api.Configuration;
using Agora.Api.Mappings;
using Agora.Operations.ApplicationOptions;
using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Agora.Api
{
    public static class DependencyInjection
    {
        // Shamelessly taken from this issue: https://github.com/codecutout/JsonApiSerializer/issues/115
        // AddJsonApi will insert a new output formatter for NewtonsoftJson
        // This have the effect that the Api layer will be able to send JsonApi from a simple POCO DTO
        public static IMvcBuilder AddJsonApi(this IMvcBuilder builder,
            Action<MvcNewtonsoftJsonOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            builder.Services.TryAddEnumerable(ServiceDescriptor
                .Transient<IConfigureOptions<MvcOptions>, ConfigureMvcOptionsForJsonApi>());
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptionsForJsonApi>();
            builder.Services.Configure(setupAction ?? (options => { }));

            return builder;
        }

        public static IServiceCollection AddApiDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }

        private static TService BindConfig<TService>(this IServiceCollection services, IConfiguration configuration,
            string key, Func<IServiceProvider, TService, TService> implementationFactory = null)
            where TService : class, new()
        {
            var settings = new TService();
            configuration.Bind(key, settings);

            if (implementationFactory == null)
            {
                services.AddSingleton(settings);
            }
            else
            {
                services.AddSingleton((serviceProvider) =>
                {
                    return implementationFactory(serviceProvider, settings);
                });
            }

            return settings;
        }

        public static IServiceCollection AddNemLogin3SamlConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Validate that the configuration should be created
            if (!configuration.GetSection("Saml2").GetValue<bool>("Enable"))
            {
                return services;
            }

            services.BindConfig<Saml2Configuration>(configuration, "Saml2", (serviceProvider, saml2Configuration) =>
            {
                saml2Configuration.SignAuthnRequest = true;

                var azureOptions = serviceProvider.GetService<IOptions<AzureOptions>>();

                if (azureOptions.Value.IsRunningInAzure)
                {
                    // Certificate is fetched from KeyVault when running in Azure.
                    var certificateKeyVaultRef = configuration["Saml2:SigningCertificateKeyVaultReference"];
                    if (string.IsNullOrEmpty(certificateKeyVaultRef))
                    {
                        throw new ArgumentException("Missing configuration Saml2:SigningCertificateVaultReference");
                    }

                    var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        ManagedIdentityClientId = azureOptions.Value.ManagedIdentityClientId
                    });

                    var client = new CertificateClient(new Uri(azureOptions.Value.KeyVaultUrl), azureCredentials);
                    var certificate = client.DownloadCertificate(certificateKeyVaultRef);

                    saml2Configuration.SigningCertificate = certificate.Value;
                }
                else
                {
                    // File is referenced with a path and should be loaded accordingly
                    saml2Configuration.SigningCertificate = CertificateUtil.Load(configuration["Saml2:SigningCertificateFile"], configuration["Saml2:SigningCertificatePassword"], X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                }

                
                saml2Configuration.DecryptionCertificates.Add(saml2Configuration.SigningCertificate);
                saml2Configuration.EncryptionCertificate = saml2Configuration.SigningCertificate;

                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                // Load Identity Provider Metadata
                var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrlAsync(httpClientFactory, new Uri(configuration["Saml2:IdPMetadata"])).GetAwaiter().GetResult();

                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.AllowedIssuer = entityDescriptor.EntityId;
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                    if (entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.HasValue)
                    {
                        saml2Configuration.SignAuthnRequest = entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.Value;
                    }
                }
                else
                {
                    throw new Exception("IdPSsoDescriptor not loaded from metadata.");
                }

                return saml2Configuration;
            });

            return services;
        }
    }
}