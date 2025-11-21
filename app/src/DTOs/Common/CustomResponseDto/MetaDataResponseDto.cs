namespace Agora.DTOs.Common.CustomResponseDto
{
    public class MetaDataResponseDto<TDto, TMetaDto>
    {
        public TDto ResponseData { get; set; }
        public TMetaDto Meta { get; set; }
    }
}