using Agora.Models.Models.Cpr;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.Cpr;
using DAO.ServicePlatformen.SF1520;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Agora.DAOs.UserDataEnrichment.CPR
{
    public class CprInformationService : ICprInformationService
    {
        private readonly IOptions<CprInformationOptions> _cprInformationOptions;
        private readonly ICertificateService _certificateService;
        private readonly ILogger<CprInformationService> _logger;

        private readonly PersonBaseDataExtendedPortTypeClient _client;

        public CprInformationService(IOptions<AzureOptions> azureOptions, IOptions<CprInformationOptions> cprInformationOptions, ICertificateService certificateService, ILogger<CprInformationService> logger)
        {
            _cprInformationOptions = cprInformationOptions;
            _certificateService = certificateService;
            _logger = logger;
            _client = InitializeClient(azureOptions, cprInformationOptions);
        }

        public async Task<AddressInformation> GetAddressInformation(string cpr)
        {
            var request = GetRequest(cpr);
            try
            {
                var response = await _client.PersonLookupAsync(request);
                return MapToAddressInformation(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while retrieving information from the CPR registry: {msg}", e.Message);
                return null;
            }
        }

        public async Task<AddressInformation> GetMailAddressInformation(string cpr)
        {
            var request = GetRequest(cpr);
            try
            {
                var response = await _client.PersonLookupAsync(request);
                return MapToMailAddressInformation(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while retrieving information from the CPR registry: {msg}", e.Message);
                return null;
            }
        }

        private AddressInformation MapToMailAddressInformation(PersonLookupResponse response)
        {
            return new AddressInformation
            {
                Name = GetPersonName(response.PersonLookupResponse1.persondata.navn),
                PostalCode = response.PersonLookupResponse1.adresse.aktuelAdresse.postnummer,
                MailDistrict = response.PersonLookupResponse1.adresse.aktuelAdresse.postdistrikt,
                StreetName = response.PersonLookupResponse1.adresse.aktuelAdresse.vejnavn,
                StreetBuilding = response.PersonLookupResponse1.adresse.aktuelAdresse.husnummer,
                Floor = response.PersonLookupResponse1.adresse.aktuelAdresse.etage,
                Suite = response.PersonLookupResponse1.adresse.aktuelAdresse.sidedoer
            };
        }

        private static string GetPersonName(navnType nameDto)
        {
            if (!string.IsNullOrEmpty(nameDto.personadresseringsnavn))
            {
                return nameDto.personadresseringsnavn;
            }

            var name = nameDto.fornavn;
            if (!string.IsNullOrEmpty(nameDto.mellemnavn))
            {
                name += $" {nameDto.mellemnavn}";
            }

            if (!string.IsNullOrEmpty(nameDto.efternavn))
            {
                name += $" {nameDto.efternavn}";
            }

            return name;
        }

        private static AddressInformation MapToAddressInformation(PersonLookupResponse response)
        {
            var alternateCity = response.PersonLookupResponse1.adresse.aktuelAdresse.bynavn;
            var city = response.PersonLookupResponse1.adresse.aktuelAdresse.postdistrikt;

            if (!string.IsNullOrEmpty(alternateCity))
            {
                if (string.IsNullOrEmpty(city))
                {
                    city = alternateCity;
                }
                else
                {
                    city = $"{city}, {alternateCity}";
                }
            }

            return new AddressInformation
            {
                PostalCode = response.PersonLookupResponse1.adresse.aktuelAdresse.postnummer,
                Address = response.PersonLookupResponse1.adresse.aktuelAdresse.standardadresse,
                City = city,
                Municipality = response.PersonLookupResponse1.adresse.aktuelAdresse.kommunekode,
                StreetName = response.PersonLookupResponse1.adresse.aktuelAdresse.vejnavn
            };
        }

        private PersonLookupRequestType GetRequest(string cpr)
        {
            return new PersonLookupRequestType
            {
                AuthorityContext = new AuthorityContextType
                {
                    MunicipalityCVR = _cprInformationOptions.Value.MunicipalityCvr,
                },
                PNR = cpr,
            };
        }

        private PersonBaseDataExtendedPortTypeClient InitializeClient(IOptions<AzureOptions> azureOptions, IOptions<CprInformationOptions> cprInformationOptions)
        {
            X509Certificate2 certificate;
            if (azureOptions.Value.IsRunningInAzure)
            {
                var keyVaultRef = cprInformationOptions.Value.CertificateKeyVaultReference;
                // get certificate from azure keyvault
                if (string.IsNullOrEmpty(keyVaultRef))
                {
                    throw new ArgumentException("Missing configuration CprInformation:CertificateVaultReference");
                }

                certificate = _certificateService.GetPrivateCertificateFromKeyVault(keyVaultRef);
            }
            else
            {
                // get certificate from file path
                var filePath = cprInformationOptions.Value.CertificatePath;
                var password = cprInformationOptions.Value.CertificatePassword;

                certificate = _certificateService.GetPrivateCertificateFromDisc(filePath, password);
            }

            var binding = GetHttpBinding();
            var endpointAddress = new EndpointAddress(cprInformationOptions.Value.Endpoint);

            var client = new PersonBaseDataExtendedPortTypeClient(binding, endpointAddress)
            {
                ClientCredentials =
                {
                    ClientCertificate =
                    {
                        Certificate = certificate,
                    }
                }
            };

            // Disable revocation checking
            client.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            return client;
        }

        private static BasicHttpsBinding GetHttpBinding()
        {
            var timeoutTimeSpan = new TimeSpan(0, 0, 30);
            var binding = new BasicHttpsBinding
            {
                Security =
                {
                    Mode = BasicHttpsSecurityMode.Transport,
                    Transport =
                    {
                        ClientCredentialType = HttpClientCredentialType.Certificate
                    }
                },
                MaxReceivedMessageSize = int.MaxValue,
                OpenTimeout = timeoutTimeSpan,
                CloseTimeout = timeoutTimeSpan,
                ReceiveTimeout = timeoutTimeSpan,
                SendTimeout = timeoutTimeSpan
            };

            return binding;
        }
    }
}