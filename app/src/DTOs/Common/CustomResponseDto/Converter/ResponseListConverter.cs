using Agora.Models.Common.CustomResponse;
using AutoMapper;

namespace Agora.DTOs.Common.CustomResponseDto.Converter
{
    public class ResponseListConverter<TModel, TDto> : ITypeConverter<ResponseList<TModel>, ResponseListDto<TDto>>
    {
        public ResponseListDto<TDto> Convert(ResponseList<TModel> source, ResponseListDto<TDto> destination,
            ResolutionContext context)
        {
            destination = new ResponseListDto<TDto>
            {
                Meta = source?.Meta
            };

            foreach (var item in source)
            {
                destination.Add(context.Mapper.Map<TModel, TDto>(item));
            }

            return destination;
        }
        
    }
}
