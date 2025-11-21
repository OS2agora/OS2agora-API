using Agora.Entities.Entities;
using Agora.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.DAOs.Persistence.DefaultData.Ballerup
{
    public static class BallerupFields
    {
        public static async Task<List<FieldEntity>> GetEntities(ApplicationDbContext context)
        {
            var conclusionFieldType =
                await context.FieldTypes.FirstOrDefaultAsync(x => x.Type == FieldType.CONCLUSION);
            var standardTemplate = await context.HearingTemplates.FirstOrDefaultAsync();
            var conclusionValidationRule = await context.ValidationRules.FirstOrDefaultAsync(x => x.FieldType == FieldType.CONCLUSION);

            return new List<FieldEntity>
            {
                new FieldEntity
                {
                    Name = "Konklusion",
                    AllowTemplates = true,
                    DisplayOrder = 500,
                    FieldType = conclusionFieldType,
                    HearingTemplate = standardTemplate,
                    ValidationRule = conclusionValidationRule,
                },
            };
        }
    }
}
