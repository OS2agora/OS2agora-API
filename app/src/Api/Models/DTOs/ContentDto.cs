using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class ContentDto : BaseDto<ContentDto.ContentAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("comment"),
            new DtoRelationship("field"),
            new DtoRelationship("hearing")
        };

        public class ContentAttributeDto : BaseAttributeDto
        {
            private string _content;
            private string _fileName;
            private string _filePath;
            private string _fileContentType;
            private Enums.ContentType _contentType;

            public string Content
            {
                get => _content;
                set { _content = value; PropertyUpdated(); }
            }

            public string FileName
            {
                get => _fileName;
                set { _fileName = value; PropertyUpdated(); }
            }

            public string FilePath
            {
                get => _filePath;
                set { _filePath = value; PropertyUpdated(); }
            }

            public string FileContentType
            {
                get => _fileContentType;
                set { _fileContentType = value; PropertyUpdated(); }
            }

            public Enums.ContentType ContentType
            {
                get => _contentType;
                set { _contentType = value; PropertyUpdated(); }
            }
        }
    }
}