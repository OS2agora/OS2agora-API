using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Files.Excel;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Common.Interfaces.Services;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromExcelFile
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateInviteesFromExcelFileCommand : UpdateInviteesBaseCommand
    {
        public File File { get; set; }

        public class UpdateInviteesFromExcelFileCommandHandler : UpdateInviteesBaseCommandHandler<UpdateInviteesFromExcelFileCommand>
        {
            private readonly IInvitationSourceDao _invitationSourceDao;
            private readonly IExcelService _excelService;
            private readonly IInvitationService _invitationService;
            private readonly IPluginService _pluginService;

            public UpdateInviteesFromExcelFileCommandHandler(IHearingDao hearingDao, IInvitationSourceDao invitationSourceDao,
                IExcelService excelService, IInvitationHandler invitationHandler, IInvitationService invitationService,
                IPluginService pluginService) : base(hearingDao, invitationHandler)
            {
                _invitationSourceDao = invitationSourceDao;
                _excelService = excelService;
                _invitationService = invitationService;
                _pluginService = pluginService;
            }

            protected override async Task<(List<InviteeIdentifiers> identifiers, InvitationSource invitationSource, string sourceName)>
                GetInvitationData(UpdateInviteesFromExcelFileCommand request)
            {
                var invitationSource = await _invitationSourceDao.GetAsync(request.InvitationSourceId);
                if (invitationSource.InvitationSourceType != InvitationSourceType.EXCEL)
                {
                    throw new InvalidOperationException("Wrong InvitationSourceType. InvitationSourceType must be of type Excel when uploading excel file");
                }

                var sourceName = invitationSource.CanDeleteIndividuals
                    ? $"{invitationSource.Name}: {request.File.Name}" : invitationSource.Name;

                request.File = await _pluginService.BeforeFileUpload(request.File);
                if (request.File.MarkedByScanner)
                {
                    throw new FileUploadException(request.File.Name);
                }

                var headers = GetHeaders(invitationSource);
                var sourceData = _excelService.ParseExcel(request.File, headers);

                var uploadedInviteeIdentifiers = MapSourceDataToInviteeIdentifiersList(sourceData, invitationSource);

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

            private List<InviteeIdentifiers> MapSourceDataToInviteeIdentifiersList(
                Dictionary<string, List<string>> sourceData, InvitationSource invitationSource)
            {
                var inviteeIdentifiers = new List<InviteeIdentifiers>();

                if (!string.IsNullOrEmpty(invitationSource.CprColumnHeader) && sourceData.TryGetValue(invitationSource.CprColumnHeader, out var cprData))
                {
                    inviteeIdentifiers.AddRange(cprData.Distinct().Select(cpr => new InviteeIdentifiers { Cpr = cpr }).ToList());
                }

                if (!string.IsNullOrEmpty(invitationSource.EmailColumnHeader) && sourceData.TryGetValue(invitationSource.EmailColumnHeader, out var emailData))
                {
                    inviteeIdentifiers.AddRange(emailData.Distinct(StringComparer.InvariantCultureIgnoreCase).Select(email => new InviteeIdentifiers { Email = email }).ToList());
                }

                if (!string.IsNullOrEmpty(invitationSource.CvrColumnHeader) && sourceData.TryGetValue(invitationSource.CvrColumnHeader, out var cvrData))
                {
                    inviteeIdentifiers.AddRange(cvrData.Distinct().Select(cvr => new InviteeIdentifiers { Cvr = cvr }).ToList());
                }

                return inviteeIdentifiers;
            }


            private List<string> GetHeaders(InvitationSource source)
            {
                var headers = new List<string>();
                if (!string.IsNullOrEmpty(source.CprColumnHeader))
                {
                    headers.Add(source.CprColumnHeader);
                }
                if (!string.IsNullOrEmpty(source.EmailColumnHeader))
                {
                    headers.Add(source.EmailColumnHeader);
                }
                if (!string.IsNullOrEmpty(source.CvrColumnHeader))
                {
                    headers.Add(source.CvrColumnHeader);
                }
                return headers;
            }
        }
    }
}