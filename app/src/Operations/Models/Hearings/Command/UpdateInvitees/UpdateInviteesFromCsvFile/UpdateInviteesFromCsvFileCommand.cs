using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Files;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Common.Interfaces.Services;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromCsvFile
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateInviteesFromCsvFileCommand : UpdateInviteesBaseCommand
    {
        public File File { get; set; }

        public class UpdateInviteesFromCsvFileCommandHandler : UpdateInviteesBaseCommandHandler<UpdateInviteesFromCsvFileCommand>
        {
            private readonly IInvitationSourceDao _invitationSourceDao;

            private readonly ICsvService _csvService;
            private readonly IInvitationService _invitationService;
            private readonly IPluginService _pluginService;

            public UpdateInviteesFromCsvFileCommandHandler(IHearingDao hearingDao, IInvitationSourceDao invitationSourceDao,
                ICsvService csvService, IInvitationHandler invitationHandler, IInvitationService invitationService,
                IPluginService pluginService) : base(hearingDao, invitationHandler)
            {
                _invitationSourceDao = invitationSourceDao;

                _csvService = csvService;
                _invitationService = invitationService;
                _pluginService = pluginService;
            }

            protected override async Task<(List<InviteeIdentifiers> identifiers, InvitationSource invitationSource, string sourceName)>
                GetInvitationData(UpdateInviteesFromCsvFileCommand request)
            {
                var invitationSource = await _invitationSourceDao.GetAsync(request.InvitationSourceId);
                if (invitationSource.InvitationSourceType != InvitationSourceType.CSV)
                {
                    throw new InvalidOperationException("Wrong InvitationSourceType. InvitationSourceType must be of type CSV when uploading .csv file");
                }

                var sourceName = invitationSource.CanDeleteIndividuals
                    ? $"{invitationSource.Name}: {request.File.Name}" : invitationSource.Name;

                request.File = await _pluginService.BeforeFileUpload(request.File);
                if (request.File.MarkedByScanner)
                {
                    throw new FileUploadException(request.File.Name);
                }

                var columnMappings = GetCsvMappings(invitationSource);
                var uploadedInviteeIdentifiers = await _csvService.ParseCsv<InviteeIdentifiers>(request.File, columnMappings);

                return (uploadedInviteeIdentifiers, invitationSource, sourceName);
            }

            protected override void NormalizeAndValidateInviteeIdentifiers(List<InviteeIdentifiers> inviteeIdentifiers)
            {
                var (isValid, errors) = _invitationService.NormalizeAndValidateInviteeIdentifiers(inviteeIdentifiers);

                if (!isValid)
                {
                    var invalidTypes = new List<string>();

                    if (errors.Any(e => e is InvalidCprException))
                    {
                        invalidTypes.Add("Cpr numbers");
                    }
                    if (errors.Any(e => e is InvalidCvrException))
                    {
                        invalidTypes.Add("Cvr numbers");
                    }
                    if (errors.Any(e => e is InvalidEmailException))
                    {
                        invalidTypes.Add("Emails");
                    }

                    var message = $"File contains invalid {string.Join(" and ", invalidTypes)}.";
                    throw new InvalidFileContentException(message);
                }
            }

            private Dictionary<string, string> GetCsvMappings(InvitationSource invitationSource)
            {
                var mappings = new Dictionary<string, string>
                {
                    ["Cpr"] = !string.IsNullOrEmpty(invitationSource.CprColumnHeader)
                    ? invitationSource.CprColumnHeader
                    : "CPR",

                    ["Email"] = !string.IsNullOrEmpty(invitationSource.EmailColumnHeader)
                    ? invitationSource.EmailColumnHeader
                    : "Email",

                    ["Cvr"] = !string.IsNullOrEmpty(invitationSource.CvrColumnHeader)
                    ? invitationSource.CvrColumnHeader
                    : "CVR"
                };

                return mappings;
            }
        }
    }
}