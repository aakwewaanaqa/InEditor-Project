using System;
using System.Reflection;

namespace InEditor.Editor.Class.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static bool TryGetAttribute<T>(this MemberInfo info, out T result) where T : System.Attribute
        {
            result = info.GetCustomAttribute<T>();
            return !(result is null);
        }

        public static bool IsParentInEditorElement(this MemberInfo info)
        {
            switch (info)
            {
                case FieldInfo field:
                    return field.FieldType.CanBeParentInEditorElement();
                case PropertyInfo property:
                    return property.PropertyType.CanBeParentInEditorElement();
                case Type type:
                    return type.CanBeParentInEditorElement();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
