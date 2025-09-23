using BallerupKommune.Models.Models;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvalidOperationException = BallerupKommune.Operations.Common.Exceptions.InvalidOperationException;

namespace BallerupKommune.Operations.Plugins.Plugins
{
    public class SbsysKleMappingPlugin : PluginBase
    {
        private readonly IOptions<SbsipOptions> _sbsipOptions;
        private readonly IKleHierarchyDao _kleHierarchyDao;

        public SbsysKleMappingPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(
            serviceProvider, pluginConfiguration)
        {
            _sbsipOptions = serviceProvider.GetService<IOptions<SbsipOptions>>();
            _kleHierarchyDao = serviceProvider.GetService<IKleHierarchyDao>();
        }

        public override async Task CheckSbsysKleMappings(List<KleMapping> kleMappings)
        {
            var kleNumbersLackingSbsysMappings = new List<KleHierarchy>();
            var mappedKleNumbers = _sbsipOptions.Value.SbsysTemplateIds;

            var kleHierarchies = await _kleHierarchyDao.GetAllAsync();

            foreach (var kleMapping in kleMappings)
            {
                var kleHierarchy = kleHierarchies.FirstOrDefault((k => k.Id == kleMapping.KleHierarchyId));
                if (kleHierarchy == null)
                {
                    throw new InvalidOperationException($"KLE number with Id {kleMapping.KleHierarchyId} does not exist.");
                }

                var kleMappingKey = kleHierarchy.Number.Substring(0, 2);
                if (!mappedKleNumbers.ContainsKey(kleMappingKey))
                {
                    kleNumbersLackingSbsysMappings.Add(kleHierarchy);
                }
            }

            if (kleNumbersLackingSbsysMappings.Any())
            {
                throw new KleMappingException(kleNumbersLackingSbsysMappings);
            }
        }
    }
}