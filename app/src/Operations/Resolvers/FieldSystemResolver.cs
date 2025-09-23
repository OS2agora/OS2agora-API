using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Resolvers
{
    /// <summary>
    /// This resolver interface is in charge of the FieldSystem in the ER diagram in our SAD(software architetcure document)
    /// It caches the data in its session (scoped)
    /// </summary>
    public interface IFieldSystemResolver
    {
        public Task<List<HearingTemplate>> GetFieldSystem();

        public Task<List<int>> GetFieldsIds(params BallerupKommune.Models.Enums.FieldType[] fieldtypes);
    }

    
    public class FieldSystemResolver : IFieldSystemResolver
    {
        private readonly IHearingTemplateDao _hearingTemplateDao;
        private List<HearingTemplate> _cachedFieldSystem = new List<HearingTemplate>();

        public FieldSystemResolver(IHearingTemplateDao hearingTemplateDao)
        {
            _hearingTemplateDao = hearingTemplateDao;
        }

        /// <summary>
        /// Adds the necessary includes to perform a query that will pull out all data from Fieldsystem
        /// </summary>
        /// <returns> The entire FieldSystem from the er diagram with all the data in it </returns>
        public async Task<List<HearingTemplate>> GetFieldSystem()
        {
            await EnsureLoaded();
            return _cachedFieldSystem;
        }

        /// <summary>
        /// Adds the necessary includes to perform a query that will pull out all data from Fieldsystem
        /// It then filters the users fieldtypes with the data's fieldtypes and keeps the field id 
        /// if it contains any
        /// </summary>
        /// <param name="fieldtypes"> List of different fieldtypes that will be searched for in query</param>
        /// <returns> The fields id's corresponding to the "fieldtypes" that the user has requested</returns>
        public async Task<List<int>> GetFieldsIds(params BallerupKommune.Models.Enums.FieldType[] fieldtypes)
        {
            await EnsureLoaded();
            IEnumerable<Field> result = _cachedFieldSystem
                .SelectMany(hearingtemplate => hearingtemplate.Fields)
                .Where(field => fieldtypes.Contains(field.FieldType.Type));
            return result.Select(field => field.Id).ToList();
        }

        /// <summary>
        /// Checks if data is cached if not then caches it
        /// </summary>
        private async Task EnsureLoaded()
        {
            if (!_cachedFieldSystem.Any())
            {
                await LoadHearingTemplate();
            }
        }

        /// <summary>
        /// Loads the data into the dictionary so it is cached in the scoped session
        /// </summary>
        private async Task LoadHearingTemplate()
        {
            var includes = IncludeProperties.Create<HearingTemplate>(null, new List<string> {
            $"{nameof(HearingTemplate.Fields)}",
            $"{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldTemplates)}",
            $"{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldTemplates)}.{nameof(FieldTemplate.Field)}",
            $"{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldType)}",
            $"{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldType)}.{nameof(FieldType.FieldTypeSpecifications)}",
            $"{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldType)}.{nameof(FieldType.FieldTypeSpecifications)}.{nameof(FieldTypeSpecification.FieldType)}",
            $"{nameof(HearingTemplate.Fields)}.{nameof(Field.HearingTemplate)}",
            });
            List<HearingTemplate> response = _hearingTemplateDao.GetAllAsync(includes: includes).Result;
            _cachedFieldSystem = response;
        }
    }
}
