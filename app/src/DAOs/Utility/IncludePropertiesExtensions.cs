using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agora.Entities.Attributes;
using Agora.Entities.Common;
using Agora.Models.Common;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.DAOs.Utility
{
    public static class IncludePropertiesExtensions
    {
        /// <summary>
        /// Throws an <see cref="Operations.Common.Exceptions.InvalidOperationException"/> if there are any disallowed
        /// request includes on the supplied include properties.
        /// </summary>
        /// <param name="includes">The include properties to validate.</param>
        /// <typeparam name="TEntity">The entity to validate request includes against.</typeparam>
        /// <exception cref="Operations.Common.Exceptions.InvalidOperationException">
        /// If any request include is invalid.
        /// </exception>
        /// <seealso cref="IsDisallowedRequestInclude{TEntity}">
        /// When a request include is considered disallowed.
        /// </seealso>
        public static void ValidateRequestIncludes<TEntity>(this IncludeProperties includes)
            where TEntity : BaseEntity 
        {
            List<string> invalidIncludes = includes.GetDisallowedRequestIncludes<TEntity>();

            if (invalidIncludes.Any())
            {
                throw new InvalidOperationException(
                    $"Invalid request includes for '{typeof(TEntity).Name}': {string.Join(", ", invalidIncludes.Select(include => $"'{include}'"))}.");
            }
        }

        /// <summary>
        /// Identifies and gets disallowed request includes from supplied <paramref name="includes"/>.
        /// </summary>
        /// <param name="includes">The include properties for which to validate the request includes.</param>
        /// <typeparam name="TEntity">The entity to validate request includes against.</typeparam>
        /// <returns>Disallowed request includes.</returns>
        /// <seealso cref="IsDisallowedRequestInclude{TEntity}">
        /// When a request include is considered disallowed.
        /// </seealso>
        private static List<string> GetDisallowedRequestIncludes<TEntity>(this IncludeProperties includes)
            where TEntity : BaseEntity
        {
            return includes.RequestIncludes.Where(IsDisallowedRequestInclude<TEntity>).ToList();
        }

        /// <summary>
        /// Identifies whether the supplied include is disallowed as a request include.
        /// <br/><br/>
        /// A request include is considered disallowed if
        /// <list type="bullet">
        /// <item><description>The navigation path does not exist on the entities.</description></item>
        /// <item><description>
        /// Any property in the navigation path does not have a <see cref="AllowRequestIncludeAttribute"/> applied.
        /// </description></item>
        /// <item><description>
        /// The length of the navigation path exceeds the max length allowed from any property in the navigation path.
        /// <see cref="AllowRequestIncludeAttribute.MaxNavigationPathLength">See AllowRequestIncludeAttribute.MaxNavigationPathLength</see>
        /// for more details.
        /// </description></item>
        /// </list>
        /// </summary>
        /// <param name="include">The request include to validate.</param>
        /// <typeparam name="TEntity">The entity to validate the request include against.</typeparam>
        /// <returns><see cref="bool"/> representing whether the request include is disallowed.</returns>
        private static bool IsDisallowedRequestInclude<TEntity>(this string include) where TEntity : BaseEntity
        {
            Type currentType = typeof(TEntity);
            var pathDepth = 1;
            var maxNavigationPathLength = int.MaxValue;
            
            foreach (string includePart in include.Split('.'))
            {
                if (pathDepth > maxNavigationPathLength)
                {
                    return true;
                }

                if (!TryGetIncludeProperty(currentType, includePart, out PropertyInfo property))
                {
                    return true;
                }

                if (!TryGetAllowRequestIncludeAttribute(property, out AllowRequestIncludeAttribute attribute))
                {
                    return true;
                }
                
                maxNavigationPathLength = Math.Min(maxNavigationPathLength, attribute.MaxNavigationPathLength + pathDepth - 1);
                pathDepth++;
                currentType = GetEntityType(property);
            }

            return false;
        }

        private static bool TryGetIncludeProperty(Type currentType, string includePart, out PropertyInfo property)
        {
            property = currentType.GetProperty(includePart);
            return property != null;
        }
        
        private static bool TryGetAllowRequestIncludeAttribute(PropertyInfo property,
            out AllowRequestIncludeAttribute allowRequestIncludeAttribute)
        {
            allowRequestIncludeAttribute =
                (AllowRequestIncludeAttribute) Attribute.GetCustomAttribute(property,
                    typeof(AllowRequestIncludeAttribute));
            return allowRequestIncludeAttribute != null;
        }

        private static Type GetEntityType(PropertyInfo property)
        {
            return property.PropertyType.IsGenericType
                ? property.PropertyType.GenericTypeArguments.Single()
                : property.PropertyType;
        }
    }
}
