using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.Plugins;
using System.Threading.Tasks;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.Operations.Plugins.Service
{
    public partial class PluginService
    {
        public async Task<Hearing> BeforeHearingCreate(Hearing model)
        {
            return await InvokeMethodOnPlugins(model,
                (type, accumulator) => InvokePlugin<Hearing>(type, nameof(IPlugin.BeforeHearingCreate), accumulator));
        }

        public async Task<Hearing> AfterHearingCreate(Hearing model)
        {
            return await InvokeMethodOnPlugins(model,
                (type, accumulator) => InvokePlugin<Hearing>(type, nameof(IPlugin.AfterHearingCreate), accumulator));
        }

        public async Task<Hearing> BeforeHearingUpdate(Hearing model, HearingStatus oldStatus)
        {
            var result = await InvokeMethodOnPlugins(model,
                (type, accumulator) => InvokePlugin<Hearing>(type, nameof(IPlugin.BeforeHearingUpdate), accumulator));

            if (model.HearingStatus != null && model.HearingStatus.Status != oldStatus)
            {
                result = await InvokeMethodOnPlugins(result,
                    (type, accumulator) =>
                        InvokePlugin<Hearing>(type, nameof(IPlugin.BeforeHearingStatusUpdate), accumulator, oldStatus));
            }

            return result;
        }

        public async Task<Hearing> AfterHearingUpdate(Hearing model, HearingStatus oldStatus)
        {
            var result = await InvokeMethodOnPlugins(model,
                (type, accumulator) => InvokePlugin<Hearing>(type, nameof(IPlugin.AfterHearingUpdate), accumulator));

            if (model.HearingStatus != null && model.HearingStatus.Status != oldStatus)
            {
                result = await InvokeMethodOnPlugins(result,
                    (type, accumulator) =>
                        InvokePlugin<Hearing>(type, nameof(IPlugin.AfterHearingStatusUpdate), accumulator, oldStatus));
            }

            return result;
        }

        public async Task BeforeHearingDelete(int hearingId, HearingStatus hearingStatus)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.BeforeHearingDelete), hearingId, hearingStatus));
        }

        public async Task AfterHearingDelete(int hearingId)
        {
            await InvokeMethodOnPlugins(type => InvokePlugin(type, nameof(IPlugin.AfterHearingDelete), hearingId));
        }
    }
}