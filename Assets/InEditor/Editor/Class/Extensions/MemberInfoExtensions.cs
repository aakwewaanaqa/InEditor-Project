using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace InEditor
{
    internal static class MemberInfoExtensions
    {
        public static bool TryGetAttribute<T>(this MemberInfo info, out T result) where T : Attribute
        {
            result = info.GetCustomAttribute<T>();
            return result is not null;
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
