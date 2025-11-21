using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agora.Models.Common;

namespace Agora.Models.Extensions
{
    public static class ModelExtension
    {
        /// <summary>
        /// Gets all the properties in a BaseModel that are references to other BaseModel types
        /// </summary>
        /// <typeparam name="TModel">Type of BaseModel</typeparam>
        /// <returns>All properties referencing other BaseModel types</returns>
        public static List<PropertyInfo> GetPropertyInfoForBaseModelFields<TModel>(this TModel model) where TModel : BaseModel
        {
            return model.GetType().GetProperties().Where(pi => pi.IsBaseModelReference()).ToList();
        }

        public static List<PropertyInfo> GetPropertyInfoForBaseModelFields<TModel>() where TModel : BaseModel
        {
            return typeof(TModel).GetProperties().Where(pi => pi.IsBaseModelReference()).ToList();
        }

        public static List<PropertyInfo> GetPropertyInfoForSingleBaseModelFields<TModel>() where TModel : BaseModel
        {
            return typeof(TModel).GetProperties().Where(pi => pi.IsSingleBaseModelReference()).ToList();
        }
    }
}
