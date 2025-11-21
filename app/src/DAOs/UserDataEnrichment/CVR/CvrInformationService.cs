using Agora.DAOs.UserDataEnrichment.CVR.DTOs;
using Agora.Models.Models.Cpr;
using Agora.Operations.Common.Interfaces.Cvr;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Agora.DAOs.UserDataEnrichment.CVR
{
    public class CvrInformationService : ICvrInformationService
    {
        private readonly CvrInformationClient _client;
        private readonly ILogger<CvrInformationService> _logger;

        public CvrInformationService(ILogger<CvrInformationService> logger, CvrInformationClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<AddressInformation> GetAddressInformation(string cvr)
        {
            var dto = await _client.GetCvrData(cvr);

            if (dto == null)
            {
                return null;
            }

            return new AddressInformation
            {
                PostalCode = dto.Beliggenhedsadresse.CvrAdressePostnummer.ToString(),
                Address = GenerateAddressString(dto),
                Municipality = dto.Beliggenhedsadresse.CvrAdresseKommunekode.ToString(),
                Country = dto.Beliggenhedsadresse.CvrAdresseLandekode,
                City = dto.Beliggenhedsadresse.CvrAdressePostdistrikt,
                StreetName = dto.Beliggenhedsadresse.CvrAdresseVejnavn
            };
        }

        public async Task<AddressInformation> GetMailAddressInformation(string cvr)
        {
            var dto = await _client.GetCvrData(cvr);

            if (dto == null)
            {
                return null;
            }

            return new AddressInformation
            {
                Name = dto.VirksomhedsNavn.Navn,
                PostalCode = GetMailAddressValue(dto.Postadresse?.CvrAdressePostnummer, dto.Beliggenhedsadresse.CvrAdressePostnummer),
                MailDistrict = GetMailAddressValue(dto.Postadresse?.CvrAdressePostdistrikt, dto.Beliggenhedsadresse.CvrAdressePostdistrikt),
                StreetName = GetMailAddressValue(dto.Postadresse?.CvrAdresseVejnavn, dto.Beliggenhedsadresse.CvrAdresseVejnavn),
                StreetBuilding = GetMailAddressValue(dto.Postadresse?.CvrAdresseHusnummerFra, dto.Beliggenhedsadresse.CvrAdresseHusnummerFra),
                Floor = GetMailAddressValue(dto.Postadresse?.CvrAdresseEtagebetegnelse, dto.Beliggenhedsadresse.CvrAdresseEtagebetegnelse),
                Suite = GetMailAddressValue(dto.Postadresse?.CvrAdresseDoerbetegnelse, dto.Beliggenhedsadresse.CvrAdresseDoerbetegnelse)
            };
        }

        private static string GetMailAddressValue(long? postAddressValue, long locationAddressValue)
        {
            return postAddressValue is null or 0 ? locationAddressValue.ToString() : postAddressValue.ToString();
        }

        private static string GetMailAddressValue(string postAddressValue, string locationAddressValue)
        {
            return string.IsNullOrEmpty(postAddressValue) ? locationAddressValue : postAddressValue;
        }

        private static string GenerateAddressString(HentCvrDataDto dto)
        {
            var streetName = dto.Beliggenhedsadresse.CvrAdresseVejnavn;
            var streetNumber = dto.Beliggenhedsadresse.CvrAdresseHusnummerFra.ToString();

            if (string.IsNullOrEmpty(streetName))
            {
                return null;
            }

            var result = streetName;

            if (!string.IsNullOrEmpty(streetNumber))
            {
                result += $", {streetNumber}";
            }

            return result;
        }
    }
}