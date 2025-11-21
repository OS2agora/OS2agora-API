using System;

namespace Agora.Entities.Attributes
{
    /// <summary>
    /// Attribute for allowing request include of an entity. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowRequestIncludeAttribute : Attribute
    {
        /// <summary>
        /// The maximum length of the request include navigation path beyond the targeted property including the
        /// targeted property.
        /// </summary>
        public readonly int MaxNavigationPathLength;

        /// <param name="maxNavigationPathLength">
        /// The maximum length of the request include navigation path beyond the targeted property including the
        /// targeted property. The default value is <c>1</c>.
        /// </param>
        public AllowRequestIncludeAttribute(int maxNavigationPathLength = 1)
        {
            MaxNavigationPathLength = maxNavigationPathLength;
        }
    }
}