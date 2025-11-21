using Agora.DAOs.Persistence;
using Agora.DAOs.Statistics;
using Agora.Entities.Entities;
using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.DAOs.Models
{
    public class InvitationGroupDao : BaseDao<InvitationGroupEntity, InvitationGroup>, IInvitationGroupDao
    {
        public InvitationGroupDao(IApplicationDbContext db, ILogger<BaseDao<InvitationGroupEntity, InvitationGroup>> logger, IMapper mapper, ICommandCountStatistics commandCountStatistics) : 
            base(db, logger, mapper, commandCountStatistics)
        {
        }

        public new async Task<InvitationGroup> GetAsync(int id, IncludeProperties includes = null)
        {
            var invitationGroupEntity = await base.GetAsync(id, includes);
            return MapAndPrune(invitationGroupEntity, includes);
        }

        public async Task<List<InvitationGroup>> GetAllAsync(IncludeProperties includes = null)
        {
            var invitationGroupEntities = await base.GetAllAsync(includes);
            return invitationGroupEntities.Select(invitationGroupEntity => MapAndPrune(invitationGroupEntity, includes)).ToList();
        }

        public new async Task<InvitationGroup> CreateAsync(InvitationGroup model, IncludeProperties includes = null)
        {
            var invitationGroupEntity = await base.CreateAsync(model, includes);
            return MapAndPrune(invitationGroupEntity, includes);
        }

        public async Task<InvitationGroup> UpdateAsync(InvitationGroup model, IncludeProperties includes = null)
        {
            var invitationGroupEntity = await UpdateAsync(model, model.PropertiesUpdated, includes);
            return MapAndPrune(invitationGroupEntity, includes);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }
    }
}