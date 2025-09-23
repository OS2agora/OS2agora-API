using BallerupKommune.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BallerupKommune.DAOs.Utility
{
    /// <summary>
    /// Contains logic to prune model objects based on a set of includes. 
    /// </summary>
    public static class ModelPruneUtility
    {
        public static T PruneIncludes<T>(T model, List<string> includes) where T : BaseModel
        {
            var includesTree = GenerateIncludesTree(includes);
            var allowedNavigationProperties = GetAllowedNavigationProperties(model.GetType(), includesTree);
            
            PruneModel(model, includesTree, allowedNavigationProperties);
            return model;
        }

        public static T PruneIncludes<T>(T model, IncludeProperties includes) where T : BaseModel
        {
            return PruneIncludes(model, includes?.AllIncludes.ToList());
        }

        private static bool InheritsFromBaseModel(this PropertyInfo propInfo) =>
            propInfo.PropertyType.IsSubclassOf(typeof(BaseModel));

        private static bool IsCollectionProperty(this PropertyInfo propInfo) =>
            propInfo.PropertyType.IsConstructedGenericType &&
            propInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>);

        /// <summary>
        /// Builds a dictionary containing object types as keys, and allowed navigationsproperties as values for that key
        /// </summary>
        /// <param name="modelType">Type of the model from which the allowed navigation properties should be deduced</param>
        /// <param name="includesTrees">IncludesTrees for the given model type</param>
        /// <param name="allowedNavigationProperties"> Recursive parameter for building the resulting dictionary</param>
        private static Dictionary<Type, List<string>> GetAllowedNavigationProperties(Type modelType, List<IncludesTree> includesTrees, Dictionary<Type, List<string>> allowedNavigationProperties = null)
        {
            // Initialize list 
            allowedNavigationProperties ??= new Dictionary<Type, List<string>>();

            var allowedProperties = includesTrees.Select(includesTree => includesTree.PropertyName).ToList();

            // Add allowed properties to a new key or union them to an existing key in the dict
            if (!allowedNavigationProperties.ContainsKey(modelType))
            {
                if (allowedProperties.Count != 0)
                {
                    allowedNavigationProperties.Add(modelType, allowedProperties.ToList());
                }
            }
            else
            {
                allowedNavigationProperties[modelType] = allowedNavigationProperties[modelType].Union(allowedProperties).ToList();
            }

            // Recursively iterate the includesTree properties
            foreach (var includesTree in includesTrees)
            {
                var propertyInfo = modelType.GetProperty(includesTree.PropertyName);

                if (propertyInfo.IsCollectionProperty())
                {
                    var nextType = propertyInfo.PropertyType.GetGenericArguments().First();
                    GetAllowedNavigationProperties(nextType, includesTree.IncludesTrees, allowedNavigationProperties);
                }
                else if (propertyInfo.InheritsFromBaseModel())
                {
                    var nextType = propertyInfo.PropertyType;
                    GetAllowedNavigationProperties(nextType, includesTree.IncludesTrees, allowedNavigationProperties);
                }
            }

            return allowedNavigationProperties;
        }

        /// <summary>
        /// Recursively removes all navigation properties from model and sub-models that are not present in includeTree
        /// </summary>
        /// <param name="model">The model to remove properties from</param>
        /// <param name="includesTreeList">Tree of properties that are allowed on the model</param>
        /// <param name="allowedNavigationProperties">A collection of all includes types and what navigation properties are allowed from these types</param>
        private static void PruneModel(object model, List<IncludesTree> includesTreeList, Dictionary<Type, List<string>> allowedNavigationProperties)
        {
            // Find properties to delete
            var navigationProperties = model.GetType().GetProperties()
                     .Where(prop => prop.InheritsFromBaseModel() || prop.IsCollectionProperty());

            var modelHasPruningMap = allowedNavigationProperties.TryGetValue(model.GetType(), out var allowedProperties);

            var propertiesToDelete = navigationProperties
                .Where(prop => !modelHasPruningMap || allowedProperties.All(propertyName => propertyName != prop.Name));

            // Delete properties
            foreach (var propertyInfo in propertiesToDelete)
            {
                if (propertyInfo.IsCollectionProperty())
                {
                    var emptyListOfType =
                        typeof(List<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments()
                            .First());
                    var emptyListInstance = Activator.CreateInstance(emptyListOfType);
                    propertyInfo.SetValue(model, emptyListInstance);
                    continue;
                }
                propertyInfo.SetValue(model, null);
            }


            // Recursively iterate through includesTree
            foreach (var includesTree in includesTreeList)
            {
                var propertyToPrune = model.GetType().GetProperty(includesTree.PropertyName);
                if (propertyToPrune.IsCollectionProperty())
                {
                    var list = propertyToPrune.GetValue(model) as ICollection;
                    foreach (var newModel in list)
                    {
                        PruneModel(newModel, includesTree.IncludesTrees, allowedNavigationProperties);
                    }
                }
                else
                {
                    var newModel = propertyToPrune.GetValue(model);
                    if (newModel != null)
                    {
                        PruneModel(newModel, includesTree.IncludesTrees, allowedNavigationProperties);
                    }
                }
            }
        }

        /// <summary>
        /// Recursively generate IncludesTree from a list of includes 
        /// </summary>
        /// <param name="includes">The includes to apply to the tree</param>
        /// <param name="result">The recursive parameter</param>
        /// <returns></returns>
        private static List<IncludesTree> GenerateIncludesTree(List<string> includes, List<IncludesTree> result = null)
        {
            // Initialize lists
            result ??= new List<IncludesTree>();
            includes ??= new List<string>();

            foreach (var include in includes)
            {
                var navigationProps = include.Split('.').ToList();
                var currentProp = navigationProps.First();
                var isLeaf = navigationProps.Count == 1;

                var newNavigationProps = navigationProps.Where((x, index) => index > 0).ToList();
                var newInclude = string.Join('.', newNavigationProps);

                // Only add to tree if it does not already exist
                if (result.All(x => x.PropertyName != currentProp))
                {
                    var resultToAdd = new IncludesTree
                    {
                        PropertyName = currentProp
                    };

                    if (!isLeaf)
                    {
                        resultToAdd.IncludesTrees = GenerateIncludesTree(new List<string> { newInclude });
                    }
                    result.Add(resultToAdd);
                }
                // Update current tree if it already exists
                else
                {
                    if (isLeaf)
                    {
                        continue;
                    }
                    var currentTree = result.Single(x => x.PropertyName == currentProp);
                    currentTree.IncludesTrees = GenerateIncludesTree(new List<string> { newInclude }, currentTree.IncludesTrees);
                }
            }

            return result;
        }

        private class IncludesTree
        {
            public string PropertyName { get; set; }
            public List<IncludesTree> IncludesTrees { get; set; } = new List<IncludesTree>();
        }
    }
}
