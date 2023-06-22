using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace InEditor
{
    public static class MemberInfoExtensions
    {
        public static IEnumerable<Type> GetTypesOf<TAttribute>() where TAttribute : Attribute
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.TryGetAttribute(out TAttribute _)) ;
        }
        public static bool TryGetAttribute<T>(this MemberInfo info, out T result) where T : Attribute
        {
            result = info.GetCustomAttribute<T>();
            return result is not null;
        }
        /// <summary>
        /// Makes sure the member has InEditorAttribute
        /// </summary>
        /// <param name="info"> the member to be checked </param>
        /// <returns> has or hasn't </returns>
        public static bool IsInEditorElement(this MemberInfo info)
        {
            return info.TryGetAttribute(out InEditorAttribute _);
        }
        /// <summary>
        /// Gets type of the MemberInfo conveniently.
        /// </summary>
        /// <param name="info">passed MemberInfo</param>
        /// <returns>field or property type</returns>
        public static Type GetFieldOrPropertyType(this MemberInfo info)
        {
            return info switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                Type type => type,
                _ => throw new InvalidOperationException()
            };
        }
        public static bool IsParentInEditorElement(this MemberInfo info)
        {
            return info switch
            {
                FieldInfo field => field.FieldType.CanBeParentInEditorElement(),
                PropertyInfo property => property.PropertyType.CanBeParentInEditorElement(),
                Type type => type.CanBeParentInEditorElement(),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
