using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using BallerupKommune.DTOs.Common;
using BallerupKommune.Models.Common;

namespace BallerupKommune.DTOs.Mappings
{
    /// <summary>
    /// Extension point to provide mapping to a relationship field in the <typeparamref name="TDto"/>.
    /// <br/>
    /// There are three cases for how the mapping is done:
    /// <list type="bullet">
    /// <item><description>
    /// If both the ID property and the object property in the model is <c>null</c>, then the destination property will
    /// be <c>null</c>.
    /// </description></item>
    /// <item><description>
    /// If the ID property is not null and the object property in the model is <c>null</c>, then the destination
    /// property will be of type <see cref="BaseDto{T}"/> with the ID property populated.
    /// </description></item>
    /// <item><description>
    /// If the ID property is not null and the object property in the model is not <c>null</c>, then the destination
    /// property will be of a derived type of <see cref="BaseDto{T}"/> with the ID property and the attributes
    /// populated.
    /// </description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TModel">The source model type.</typeparam>
    /// <typeparam name="TDto">The destination dto type.</typeparam>
    /// <remarks>
    /// The reason for this implementation, is that the
    /// <see href="https://www.nuget.org/packages/JsonApiSerializer">JsonApiSerializer</see> serializes any object
    /// property with an "Id" property as a relationship, and if the object type has any other property than "Id" and
    /// "Type", then it is included in the top-level dto as an include. In some cases, we want to have a relationship on
    /// the data object without it being included with <c>null</c>/default values. To do this we map to the
    /// <see cref="BaseDto{T}"/> instead of any of its derivatives when we only have the ID.
    /// <br/>
    /// <see href="https://github.com/codecutout/JsonApiSerializer/issues/124">See GitHub issue for more details.</see>
    /// </remarks>
    public class RelationshipResolver<TModel, TDto> : IMemberValueResolver<TModel, TDto, object, object>
        where TModel : BaseModel where TDto : BaseDto<TDto>
    {
        private readonly string _propertyName;

        /// <summary>
        /// Constructs <see cref="RelationshipResolver{TModel,TDto}"/>.
        /// </summary>
        /// <param name="propertyName">The name of the relationship property in the <typeparamref name="TDto"/>.</param>
        public RelationshipResolver(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <inheritdoc/>
        /// <seealso cref="RelationshipResolver{TModel,TDto}">See for details on how it is resolved.</seealso>
        public object Resolve(TModel source, TDto destination, object sourceMember, object destMember, ResolutionContext context)
        {
            PropertyInfo sourcePropertyInfo = typeof(TModel).GetProperty(_propertyName)!;
            PropertyInfo destinationPropertyInfo = typeof(TDto).GetProperty(_propertyName)!;
            object sourceRelationshipValue = sourcePropertyInfo.GetValue(source);

            if (sourceRelationshipValue != null)
            {
                Type destinationType = destinationPropertyInfo.PropertyType.GenericTypeArguments.Single();
                return context.Mapper.Map(sourceRelationshipValue, sourcePropertyInfo.PropertyType, destinationType);
            }

            var id = (int?) typeof(TModel).GetProperty(_propertyName + "Id")!.GetValue(source);

            if (id == null)
            {
                return null;
            }
                
            object baseDto = Activator.CreateInstance(destinationPropertyInfo.PropertyType);
            destinationPropertyInfo.PropertyType.GetProperty("Id")!.SetValue(baseDto, id);
            return baseDto;
        }
    }
}