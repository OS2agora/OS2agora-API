using AutoMapper;
using Agora.DAOs.Esdh.Sbsip.V12.Interface;
using Agora.Models.Models;
using Agora.Models.Models.Esdh;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Models.Common;
using HearingRole = Agora.Models.Enums.HearingRole;

namespace Agora.DAOs.Esdh.Sbsip
{
    public class SbsipService : IEsdhService
    {

        private readonly ICaseServiceV12 _caseServiceV12;
        private readonly IUserServiceV12 _userServiceV12;
        private readonly IDocumentServiceV12 _documentServiceV12;
        
        private readonly IOptions<SbsipOptions> _sbsipOptions;
        private readonly IHearingDao _hearingDao;
        private readonly IUserDao _userDao;
        private readonly IKleHierarchyDao _kleHierarchyDao;
        private readonly IMapper _mapper;

        public SbsipService(ICaseServiceV12 caseServiceV12, IMapper mapper, IUserServiceV12 userServiceV12, IHearingDao hearingDao, IDocumentServiceV12 documentServiceV12, IUserDao userDao, IOptions<SbsipOptions> sbsipOptions, IKleHierarchyDao kleHierarchyDao)
        {
            _caseServiceV12 = caseServiceV12;
            _mapper = mapper;
            _userServiceV12 = userServiceV12;
            _hearingDao = hearingDao;
            _documentServiceV12 = documentServiceV12;
            _userDao = userDao;
            _sbsipOptions = sbsipOptions;
            _kleHierarchyDao = kleHierarchyDao;
        }
        
        public Case DeserializeMetaDataFromHearing(Hearing hearing)
        {
            try
            {
                return JsonConvert.DeserializeObject<Case>(hearing.EsdhMetaData);
            }
            catch
            {
                return new Case();
            }
        }

        public async Task<Case> CreateCase(int hearingId, string esdhTitle, int kleHierarchyId)
        {
            var includes = IncludeProperties.Create<Hearing>(null, new List<string>
            {
                nameof(Hearing.KleHierarchy), nameof(Hearing.UserHearingRoles),
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}"
            });

            var hearing = await _hearingDao.GetAsync(hearingId, includes);

            if (hearing == null)
            {
                throw new Exception($"No hearing with id: {hearingId} - Failed to create case in SBSYS");
            }

            User hearingOwner = hearing.UserHearingRoles?.SingleOrDefault(x => x.HearingRole?.Role == HearingRole.HEARING_OWNER)?.User;
            var hearingOwnerDisplayName = hearingOwner?.EmployeeDisplayName;

            if (hearingOwnerDisplayName == null)
            {
                throw new Exception($"No HearingOwner found on hearing: {hearingId} - Failed to create case in SBSYS");
            }
            var userDto = await _userServiceV12.SearchForUser(hearingOwnerDisplayName);

            if (userDto == null)
            {
                throw new Exception($"No User found in SBSYS with LogonID: {hearingOwnerDisplayName} - Failed to create case in SBSYS");
            }

            var userId = userDto.Id;

            if (string.IsNullOrEmpty(esdhTitle))
            {
                throw new Exception($"No ESDH-Title found on hearing: {hearingId} - Failed to create case in SBSYS");
            }

            if (kleHierarchyId == 0)
            {
                throw new Exception($"No KleHierarchy Id found on hearing: {hearingId} - Failed to create case in SBSYS");
            }

            var templateId = await GetCorrectTemplateId(kleHierarchyId);

            var caseResponse = await _caseServiceV12.CreateCase(userId, esdhTitle, templateId);
            var caseModel = _mapper.Map<Case>(caseResponse);

            return caseModel;
        }

        public async Task<Case> ChangeHearingOwner(int hearingId, int userId)
        {
            var includes = IncludeProperties.Create<Hearing>(null, new List<string>
            {
                nameof(Hearing.UserHearingRoles),
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}"
            });

            var hearing = await _hearingDao.GetAsync(hearingId, includes);

            if (hearing == null)
            {
                throw new Exception($"No hearing with id: {hearingId} - Failed to change hearing owner in SBSYS");
            }

            var hearingMetaData = DeserializeMetaDataFromHearing(hearing);

            if (hearingMetaData.Guid == Guid.Empty)
            {
                throw new Exception($"No case guid found on hearing: {hearingId} - Failed to change hearing owner in SBSYS");
            }

            var hearingOwner = await _userDao.GetAsync(userId);
            var hearingOwnerDisplayName = hearingOwner?.EmployeeDisplayName;

            if (hearingOwnerDisplayName == null)
            {
                throw new Exception($"No HearingOwner found on hearing: {hearingId} - Failed to change hearing owner in SBSYS");
            }

            var userDto = await _userServiceV12.SearchForUser(hearingOwnerDisplayName);

            if (userDto == null)
            {
                throw new Exception($"No User found in SBSYS with LogonID: {hearingOwnerDisplayName} - Failed to change hearing owner in SBSYS");
            }

            var caseResponse = await _caseServiceV12.UpdateHearingOwnerOnCase(hearingMetaData.Guid, userDto);
            var caseModel = _mapper.Map<Case>(caseResponse);

            return caseModel;
        }

        public async Task JournalizeHearingAnswer(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            await JournalizeDocumentOnCase(hearingId, "Høringssvar", file, fileName, fileDescription, fileContentType);
        }

        public async Task JournalizeHearingText(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            await JournalizeDocumentOnCase(hearingId, "Høringstekst", file, fileName, fileDescription, fileContentType);
        }

        public async Task JournalizeHearingConclusion(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            await JournalizeDocumentOnCase(hearingId, "Konklusion", file, fileName, fileDescription, fileContentType);
        }

        public async Task JournalizeDocumentOnCase(int hearingId, string documentType, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            var hearing = await _hearingDao.GetAsync(hearingId);

            if (hearing == null)
            {
                throw new Exception($"No hearing with id: {hearingId} - Failed to journalise document in SBSYS");
            }

            var hearingMetaData = DeserializeMetaDataFromHearing(hearing);

            if (hearingMetaData.Id == 0)
            {
                throw new Exception($"No case id found on hearing: {hearingId} - Failed to journalise document in SBSYS");
            }

            var documentTypes = await _documentServiceV12.GetDocumentTypes(hearingMetaData.Id);
            var correctDocumentType = documentTypes.SingleOrDefault(document => document.Titel == documentType);

            if (correctDocumentType == null)
            {
                var expectedDocumentTypes = documentTypes.Select(x => x.Titel).ToList();
                string joinedExpectedTypes = string.Join(", ", expectedDocumentTypes);
                string message = $"No matching document type found on hearing: {hearingId} - Failed to journalise document in SBSYS\nDocument types available: \"{joinedExpectedTypes}\" while looking for \"{documentType}\"";
                throw new Exception(message);
            }

            var savedDocument = await _documentServiceV12.JournaliseDocument(hearingMetaData.Id, file, fileName, fileDescription, fileContentType);

            await _documentServiceV12.ConnectDocumentAndCase(correctDocumentType.ID, savedDocument.Id);
        }

        public async Task<Case> CloseCase(int hearingId, string comment = null)
        {
            var hearing = await _hearingDao.GetAsync(hearingId);

            if (hearing == null)
            {
                throw new Exception($"No hearing with id: {hearingId} - Failed to close case in SBSYS");
            }

            var hearingMetaData = DeserializeMetaDataFromHearing(hearing);

            if (hearingMetaData.Guid == Guid.Empty)
            {
                throw new Exception($"No case guid found on hearing: {hearingId} - Failed to close case in SBSYS");
            }

            var caseStatus = await _caseServiceV12.GetCaseStatus();

            var closedCaseStatus = caseStatus.SingleOrDefault(status => status.Navn == "Afsluttet");

            if (closedCaseStatus == null)
            {
                throw new Exception($"No matching case status found - Failed to close case in SBSYS");
            }

            var caseResponse = await _caseServiceV12.CloseCase(hearingMetaData.Guid, closedCaseStatus.Id, comment);
            var caseModel = _mapper.Map<Case>(caseResponse);

            return caseModel;
        }

        private async Task<int> GetCorrectTemplateId(int kleHiearchyId)
        {
            var kleHierarchy = await _kleHierarchyDao.GetAsync(kleHiearchyId);


            if (kleHierarchy == null || string.IsNullOrEmpty(kleHierarchy.Number))
            {
                throw new KleMappingException($"Could not find Kle-number of KleHiearchy with id: {kleHierarchy} - Failed to find sbsys template id");
            }

            var kleMappingKey = kleHierarchy.Number.Substring(0, 2);
            var kleMappingTable = _sbsipOptions.Value.SbsysTemplateIds;

            if (!kleMappingTable.TryGetValue(kleMappingKey, out var templateId))
            {
                throw new KleMappingException($"No template found in SBSYS corresponding to KLE number: {kleHierarchy.Number} - Failed to get template from SBSYS");
            }

            return templateId;
        }
    }
}