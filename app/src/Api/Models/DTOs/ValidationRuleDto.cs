using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class ValidationRuleDto : BaseDto<ValidationRuleDto.ValidationRuleAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("field")
        };

        public class ValidationRuleAttributeDto : BaseAttributeDto 
        {
            private bool _canBeEmpty;
            private int _maxLength;
            private int _minLength;
            private int _maxFileSize;
            private int _maxFileCount;
            private string[] _allowedFileTypes;
            private Enums.FieldType _fieldType;

            public bool CanBeEmpty {
                get => _canBeEmpty;
                set { _canBeEmpty = value; PropertyUpdated(); }
            }

            public int MaxLength
            {
                get => _maxLength;
                set { _maxLength = value; PropertyUpdated(); }
            }

            public int MinLength
            {
                get => _minLength;
                set { _minLength = value; PropertyUpdated(); }
            }

            public int MaxFileSize
            {
                get => _maxFileSize;
                set { _maxFileSize = value; PropertyUpdated(); }
            }

            public int MaxFileCount
            {
                get => _maxFileCount;
                set { _maxFileCount = value; PropertyUpdated(); }
            }

            public string[] AllowedFileTypes
            {
                get => _allowedFileTypes;
                set { _allowedFileTypes = value; PropertyUpdated(); }
            }

            public Enums.FieldType FieldType
            {
                get => _fieldType;
                set { _fieldType = value; PropertyUpdated(); }
            }
        }
    }
}