using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Extensions
{
    public static class TypeExtensions
    {
        public static Type GetTypeByPropertyPath<TModel>(this string path) where TModel : BaseModel
        {
            var type = typeof(TModel);

            var memberInfos = GetMemberPath(type, path);

            return GetCurrentType(GetUnderlyingType(memberInfos.Last()));
        }

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        public const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(TAttribute)));
        }

        public static IEnumerable<Type> BaseClassesAndInterfaces(this Type type)
        {
            var currentType = type;
            while ((currentType = currentType.BaseType) != null)
            {
                yield return currentType;
            }
            foreach (var interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }
        }

        public static MethodInfo GetStaticMethod(this Type type, string name) => type.GetMethod(name, StaticFlags);
        public static MethodInfo GetInstanceMethod(this Type type, string name) =>
            (MethodInfo)type.GetMember(name, MemberTypes.Method, InstanceFlags).FirstOrDefault();

        public static PropertyInfo GetInheritedProperty(this Type type, string name) => type.GetProperty(name, InstanceFlags) ??
            type.BaseClassesAndInterfaces().Select(t => t.GetProperty(name, InstanceFlags)).FirstOrDefault(p => p != null);

        public static FieldInfo GetInheritedField(this Type type, string name) => type.GetField(name, InstanceFlags) ??
            type.BaseClassesAndInterfaces().Select(t => t.GetField(name, InstanceFlags)).FirstOrDefault(f => f != null);

        public static MethodInfo GetInheritedMethod(this Type type, string name) => type.GetInstanceMethod(name) ??
            type.BaseClassesAndInterfaces().Select(t => t.GetInstanceMethod(name)).FirstOrDefault(m => m != null)
            ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find member {name} of type {type}.");

        public static bool IsCollection(this Type type) => type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);

        internal static Type GetCurrentType(Type type) => type.IsGenericType && type.IsCollection() ? type.GenericTypeArguments[0] : type;
        public static MemberInfo[] GetMemberPath(this Type type, string fullMemberName)
        {
            var memberNames = fullMemberName.Split('.');
            var members = new MemberInfo[memberNames.Length];
            Type previousType = type;
            for (int index = 0; index < memberNames.Length; index++)
            {
                var currentType = GetCurrentType(previousType);
                var memberName = memberNames[index];
                var property = currentType.GetInheritedProperty(memberName);
                if (property != null)
                {
                    previousType = property.PropertyType;
                    members[index] = property;
                }
                else if (currentType.GetInheritedField(memberName) is FieldInfo field)
                {
                    previousType = field.FieldType;
                    members[index] = field;
                }
                else
                {
                    var method = currentType.GetInheritedMethod(memberName);
                    previousType = method.ReturnType;
                    members[index] = method;
                }
            }
            return members;
        }

        /// <summary>
        /// Gets the <see cref="RelationshipInfo{T}"/>s of all the relationship property pairs on
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to get relationships from.</typeparam>
        /// <returns>A list of relationship infos.</returns>
        public static List<RelationshipInfo<T>> GetRelationships<T>()
        {
            PropertyInfo[] allPropertyInfos = typeof(T).GetProperties();
            return allPropertyInfos
                .Where(propertyInfo => propertyInfo.Name.EndsWith("Id"))
                .Select(idPropertyInfo => GetRelationShip<T>(idPropertyInfo, allPropertyInfos))
                .Where(relationship => relationship != null)
                .ToList();
        }

        private static RelationshipInfo<T> GetRelationShip<T>(PropertyInfo idPropertyInfo,
            PropertyInfo[] allPropertyInfos)
        {
            if (idPropertyInfo.PropertyType != typeof(int) && idPropertyInfo.PropertyType != typeof(int?))
            {
                return null;
            }

            PropertyInfo objectPropertyInfo = 
                allPropertyInfos.SingleOrDefault(propertyInfo => propertyInfo.Name == idPropertyInfo.Name[..^2]);
            if (objectPropertyInfo == null)
            {
                return null;
            }

            return new RelationshipInfo<T>
            {
                IdPropertyInfo = idPropertyInfo,
                ObjectPropertyInfo = objectPropertyInfo
            };
        }
    }
}
