using Agora.Entities.Entities;
using System.Collections.Generic;

namespace Agora.DAOs.Persistence.DefaultData.Ballerup;

public class BallerupValidationRules
{
    public static List<ValidationRuleEntity> GetEntities()
    {
        return new List<ValidationRuleEntity>
        {
            // Conclusion
            new ValidationRuleEntity
            {
                MaxLength = 1000,
                CanBeEmpty = false,
                FieldType = Entities.Enums.FieldType.CONCLUSION,
            }
        };
    }
}