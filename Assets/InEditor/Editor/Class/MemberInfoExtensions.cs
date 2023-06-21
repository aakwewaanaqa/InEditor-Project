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
            return result is object;
        }
        /// <summary>
        /// Makes sure the member has InEditorAttribute
        /// </summary>
        /// <param name="info"> the memeber to be checked </param>
        /// <returns> has or hasn't </returns>
        public static bool IsInEditorElement(this MemberInfo info)
        {
            return info.TryGetAttribute(out InEditorAttribute _);
        }
        public static bool IsParentInEditorElement(this MemberInfo info)
        {
            if (info is FieldInfo field)
                return field.FieldType.CanBeParentInEditorElement();
            else if (info is PropertyInfo property)
                return property.PropertyType.CanBeParentInEditorElement();
            else if (info is Type type)
                return type.CanBeParentInEditorElement();
            throw new NotImplementedException();
        }
    }
}