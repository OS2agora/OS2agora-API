using System.Reflection;

namespace Agora.Models.Common
{
    /// <summary>
    /// Property information for properties from a relationship property pair on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type the properties belong to.</typeparam>
    public class RelationshipInfo<T>
    {
        /// <summary>
        /// The property info for the ID property on <typeparamref name="T"/>.
        /// </summary>
        public PropertyInfo IdPropertyInfo { get; set; }
        
        /// <summary>
        /// The property info for the object property on <typeparamref name="T"/>. The object property type is expected
        /// to have an ID property.
        /// </summary>
        public PropertyInfo ObjectPropertyInfo { get; set; }
        
        /// <summary>
        /// Checks whether the <paramref name="instance"/> has the same ID for the relationship properties.
        /// </summary>
        /// <param name="instance">The instance to check.</param>
        /// <returns>
        /// <see cref="bool"/> representing whether the IDs on the relationship properties are the same.
        /// </returns>
        public bool HasTheSameIdsOnProperties(T instance)
        {
            return GetIdFromObjectProperty(instance) == GetIdFromIdProperty(instance);
        }

        private int? GetIdFromIdProperty(T instance)
        {
            return (int?) IdPropertyInfo.GetValue(instance);
        }

        private int? GetIdFromObjectProperty(T instance)
        {
            PropertyInfo associatedIdPropertyInfo = ObjectPropertyInfo.PropertyType.GetProperty("Id")!;
            object associatedObject = ObjectPropertyInfo.GetValue(instance);
            if (associatedObject == null)
            {
                return null;
            }
            return (int) associatedIdPropertyInfo.GetValue(associatedObject);
        }
    }
}