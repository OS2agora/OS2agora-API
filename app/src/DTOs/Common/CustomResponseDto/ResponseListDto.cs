using System.Collections.Generic;

namespace Agora.DTOs.Common.CustomResponseDto
{
    public class ResponseListDto<T> : List<T>
    {
        public Dictionary<string, object> Meta { get; set; }
    }
}
