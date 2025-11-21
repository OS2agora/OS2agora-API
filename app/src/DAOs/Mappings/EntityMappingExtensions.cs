using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agora.Entities.Common;
using Agora.Models.Common;
using TypeExtensions = Agora.Models.Extensions.TypeExtensions;

namespace Agora.DAOs.Mappings
{
    public static class EntityMappingExtensions
    {
        /// <summary>
        /// Copy updated properties and relationship IDs from a new entity to an existing entity. Default-valued ID
        /// properties are not copied.
        /// </summary>
        /// <param name="existingEntity">The existing entity to copy properties to. This object is mutated.</param>
        /// <param name="newEntity">The new entity to copy from.</param>
        /// <param name="updatedProperties">The updated properties on the new entity.</param>
        /// <remarks>
        /// This ensures half-DTO's from the API layer won't change anything besides exactly what is set from the API
        /// layer.
        /// </remarks>
        public static void CopyUpdatedPropertiesAndRelationships<TEntity>(this TEntity existingEntity,
            TEntity newEntity, IEnumerable<string> updatedProperties) where TEntity : BaseEntity
        {
            IEnumerable<PropertyInfo> updatedPropertyInfos =
                updatedProperties?.Select(name => typeof(TEntity).GetProperty(name)).Where(info => info != null) ??
                Enumerable.Empty<PropertyInfo>();
            
            foreach (PropertyInfo propertyInfo in updatedPropertyInfos)
            {
                object valueFromNewEntity = propertyInfo.GetValue(newEntity);
                propertyInfo.SetValue(existingEntity, valueFromNewEntity);
            }
            
            IEnumerable<PropertyInfo> idPropertyInfos = TypeExtensions.GetRelationships<TEntity>()
                .Select(relationship => relationship.IdPropertyInfo);
            
            foreach (PropertyInfo propertyInfo in idPropertyInfos)
            {
                object valueFromNewEntity = propertyInfo.GetValue(newEntity);
                if (valueFromNewEntity != default)
                {
                    propertyInfo.SetValue(existingEntity, valueFromNewEntity);
                }
            }
        }

        /// <summary>
        /// When updating entities in EF Core we want to set the Id of the relationship, but in the Models layer we are working with the property itself
        /// Automapper maps the Id correctly from Model -> Entity, but EF Core throws up if the property itself is set (because Automapper new's it)
        /// This method finds the Id-relationships and clears out the property.
        /// </summary>
        public static void FixRelationships<TEntity>(this TEntity existingEntity) where TEntity : BaseEntity
        {
            List<RelationshipInfo<TEntity>> relationships = TypeExtensions.GetRelationships<TEntity>();
            foreach (RelationshipInfo<TEntity> relationship in relationships)
            {
                object associatedEntity = relationship.ObjectPropertyInfo.GetValue(existingEntity);
                if (associatedEntity == null)
                {
                    continue;
                }
                
                if (!relationship.HasTheSameIdsOnProperties(existingEntity))
                {
                    throw new ArgumentException(
                        $"Cannot fix relationship of entity of type '{typeof(TEntity).Name}'. Id of '{relationship.ObjectPropertyInfo.Name}' and '{relationship.IdPropertyInfo.Name}' are not the same.",
                        nameof(existingEntity));
                }

                relationship.ObjectPropertyInfo.SetValue(existingEntity, null);
            }
        }
    }
}
