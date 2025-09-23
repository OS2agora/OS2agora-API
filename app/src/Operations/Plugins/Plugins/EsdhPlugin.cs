using BallerupKommune.Models.Models;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Operations.Resolvers;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;

namespace BallerupKommune.Operations.Plugins.Plugins
{
    public class EsdhPlugin : PluginBase
    {
        private readonly IEsdhService _esdhService;
        private readonly ILogger<Hearing> _hearingLogger;
        private readonly IHearingRoleResolver _hearingRoleResolver;

        public EsdhPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(
            serviceProvider, pluginConfiguration)
        {
            _esdhService = serviceProvider.GetService<IEsdhService>();
            _hearingLogger = serviceProvider.GetService<ILogger<Hearing>>();
            _hearingRoleResolver = serviceProvider.GetService<IHearingRoleResolver>();
        }

        public override async Task<Hearing> BeforeHearingStatusUpdate(Hearing model, HearingStatus oldStatus)
        {
            if (oldStatus == HearingStatus.CREATED && model?.HearingStatus.Status == HearingStatus.DRAFT)
            {
                var esdhResult = await _esdhService.CreateCase(model.Id, model.EsdhTitle, model.KleHierarchyId ?? 0);
                model.EsdhNumber = esdhResult.EsdhTitle;
                model.EsdhMetaData = JsonConvert.SerializeObject(esdhResult);
                model.PropertiesUpdated.AddRange(new List<string> { nameof(Hearing.EsdhNumber), nameof(Hearing.EsdhMetaData) });

                _hearingLogger.LogInformation($"Hearing with id {model.Id} was successfully created in esdh-system with id: {model.EsdhNumber}");
            }
            return model;
        }

        public override async Task BeforeHearingDelete(int hearingId, HearingStatus hearingStatus)
        {
            if (hearingStatus == HearingStatus.DRAFT)
            {
                var esdhResult = await _esdhService.CloseCase(hearingId, "Sagen er lukket før afvikling af høringen");
                _hearingLogger.LogInformation($"Hearing with id {hearingId} case was successfully closed prematurely with id: {esdhResult.EsdhTitle}");
            }
        }

        public override async Task<UserHearingRole> BeforeUserHearingRoleCreate(UserHearingRole model)
        {
            int hearingOwnerRoleId = (await _hearingRoleResolver.GetHearingRole(HearingRole.HEARING_OWNER)).Id;
            if (model.HearingRoleId == hearingOwnerRoleId && model.HearingId != 0 && model.UserId != 0)
            {
                await _esdhService.ChangeHearingOwner(model.HearingId, model.UserId);
            }

            return model;
        }
    }
}