using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BallerupKommune.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IFormFile, BallerupKommune.Models.Models.Files.File>(MemberList.Destination)
                .ForMember(x => x.Extension, options => options.MapFrom(x => Path.GetExtension(x.FileName)))
                .ForMember(x => x.Name, options => options.MapFrom(x => x.FileName))
                .ForMember(x => x.Content, options => options.MapFrom(x => MapIFormFileToByteArray(x)))
                .ForMember(x => x.MarkedByScanner, options => options.Ignore())
                .ReverseMap();
        }

        private static byte[] MapIFormFileToByteArray(IFormFile file)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}