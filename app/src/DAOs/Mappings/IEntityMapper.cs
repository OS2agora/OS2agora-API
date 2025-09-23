using AutoMapper;
using BallerupKommune.Entities.Common;
using BallerupKommune.Models.Common;

namespace BallerupKommune.DAOs.Mappings
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
