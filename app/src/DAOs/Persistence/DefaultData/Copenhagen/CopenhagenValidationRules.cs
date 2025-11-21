using System;
using System.Collections.Generic;
using Agora.Entities.Entities;

namespace Agora.DAOs.Persistence.DefaultData.Copenhagen;

public static class CopenhagenValidationRules
{
    public static List<ValidationRuleEntity> GetEntities()
    {
        return new List<ValidationRuleEntity>
        {
            // Titel
            new ValidationRuleEntity
            {
                CanBeEmpty = false,
                MaxLength = 60,
                MinLength = 5,
                FieldType = Entities.Enums.FieldType.TITLE,
            },
            // Billede
            new ValidationRuleEntity
            {
                AllowedFileTypes = new[]
                {
                    "image/jpeg",
                    "image/png",
                    "image/svg"
                },
                MaxFileSize = 3 * (int)Math.Pow(10,6), // 3 MB
                CanBeEmpty = true,
                FieldType = Entities.Enums.FieldType.IMAGE,
            },
        };
    }
}