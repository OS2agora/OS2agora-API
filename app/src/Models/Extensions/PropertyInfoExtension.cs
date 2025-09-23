using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Extensions;

namespace Models.Extension
{
    internal static class PropertyInfoExtension
    {
        internal static bool IsListBaseModelReference(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsGenericType &&
                   propertyInfo.PropertyType.IsCollection() &&
                propertyInfo.PropertyType.GenericTypeArguments[0].BaseType.IsSubclassOf(typeof(BaseModel));
        }

        internal static bool IsSingleBaseModelReference(this PropertyInfo propertyInfo)
        {
            return typeof(BaseModel).IsAssignableFrom(propertyInfo.PropertyType);
        }

        public static bool IsBaseModelReference(this PropertyInfo propertyInfo)
        {
            return propertyInfo.IsListBaseModelReference() || propertyInfo.IsSingleBaseModelReference();
        }
    }
}
