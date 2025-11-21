using AutoMapper;
using Agora.Entities.Common;
using Agora.Models.Common;

namespace Agora.DAOs.Mappings
{
    public interface IEntityMapper<TEntity, TModel>
        where TEntity : BaseEntity
        where TModel : BaseModel
    {
        void Mapping(Profile profile)
        {
            profile.CreateMap<TEntity, TModel>()
                .ForMember(x => x.PropertiesUpdated, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
