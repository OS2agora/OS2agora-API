using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Models.Models.Esdh;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Newtonsoft.Json;

namespace Agora.DAOs.Esdh.Stub
{
    /// <summary>
    /// This service is a stub service that mimics the behaviour of the real EsdhService integrations.
    /// </summary>
    public class EsdhStubService : IEsdhService
    {

        private readonly IHearingDao _hearingDao;
        private readonly IUserDao _userDao;
        private readonly IKleHierarchyDao _kleHierarchyDao;
        private readonly IMapper _mapper;

        public EsdhStubService(IUserDao userDao, IKleHierarchyDao kleHierarchyDao, IMapper mapper, IHearingDao hearingDao)
        {
            _userDao = userDao;
            _kleHierarchyDao = kleHierarchyDao;
            _mapper = mapper;
            _hearingDao = hearingDao;
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
                throw new Exception($"No hearing with id: {hearingId} - Failed to create case in Esdh Stub");
            }

            if (string.IsNullOrEmpty(esdhTitle))
            {
                throw new Exception($"No ESDH-Title found on hearing: {hearingId} - Failed to create case in Esdh Stub");
            }

            if (kleHierarchyId == 0)
            {
                throw new Exception($"No KleHierarchy Id found on hearing: {hearingId} - Failed to create case in Esdh Stub");
            }

            var kleHierarchy = await _kleHierarchyDao.GetAsync(kleHierarchyId);

            if (kleHierarchy == null || string.IsNullOrEmpty(kleHierarchy.Number))
            {
                throw new KleMappingException($"Could not find Kle-number of KleHiearchy with id: {kleHierarchy} - Failed to generate Esdh Stub Case");
            }

            var guid = Guid.NewGuid();
            var esdhCase = new Case
            {
                EsdhTitle = $"{kleHierarchy.Number}-{guid.ToString()}",
                Id = 0,
                Guid = guid
            };

            return esdhCase;
        }

        public Task<Case> ChangeHearingOwner(int hearingId, int userId)
        {
            return Task.FromResult(new Case());
        }

        public Task JournalizeHearingAnswer(int hearingId, byte[] file, string fileName, string fileDescription,
            string fileContentType)
        {
            return Task.CompletedTask;
        }

        public Task JournalizeHearingText(int hearingId, byte[] file, string fileName, string fileDescription, string fileContentType)
        {
            return Task.CompletedTask;
        }

        public Task JournalizeHearingConclusion(int hearingId, byte[] file, string fileName, string fileDescription,
            string fileContentType)
        {
            return Task.CompletedTask;
        }

        public Task<Case> CloseCase(int hearingId, string comment = null)
        {
            return Task.FromResult(new Case());
        }
    }
}