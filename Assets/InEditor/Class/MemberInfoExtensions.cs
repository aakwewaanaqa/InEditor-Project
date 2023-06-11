using System;
using System.Reflection;
using UnityEngine;

namespace InEditor
{
    public static class MemberInfoExtensions
    {
        public static bool TryGetAttribute<T>(this MemberInfo info, out T result) where T : Attribute
        {
            result = info.GetCustomAttribute<T>();
            return result is object;
        }
        public static bool CanBeInEditorElement(this MemberInfo info)
        {
            if (info.TryGetAttribute(out InEditorAttribute _))
                return true;
            if (info.TryGetAttribute(out SerializeField _))
                return true;
            if (info.TryGetAttribute(out SerializeReference _))
                return true;
            if (info is Type type)
                return type.CanBeSerializedInUnity();
            if (info is FieldInfo field)
            {
                if (!field.IsStatic && field.IsPublic && field.FieldType.CanBeSerializedInUnity())
                    return true;
            }
            return false;
        }
        public static bool CanBeSerializedInUnity(this Type type)
        {
            if (type.TryGetAttribute(out SerializableAttribute _))
                return true;
            if (type.IsPrimitive)
                return true;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type) ||
                typeof(UnityEngine.Vector2) == (type) ||
                typeof(UnityEngine.Vector2Int) == (type) ||
                typeof(UnityEngine.Vector3) == (type) ||
                typeof(UnityEngine.Vector3Int) == (type) ||
                typeof(UnityEngine.Rect) == (type) ||
                typeof(UnityEngine.RectInt) == (type) ||
                typeof(UnityEngine.Bounds) == (type) ||
                typeof(UnityEngine.BoundsInt) == (type) ||
                typeof(UnityEngine.Quaternion) == (type) ||
                typeof(UnityEngine.Color) == (type) ||
                typeof(UnityEngine.Color32) == (type))
                return true;
            return false;
        }
        public static bool CanBeParentInEditorElement(this MemberInfo info)
        {
            if (info is FieldInfo field)
                return field.FieldType.CanBeParentInEditorElement();
            else if (info is PropertyInfo property)
                return property.PropertyType.CanBeParentInEditorElement();
            else if (info is Type type)
                return type.CanBeParentInEditorElement();
            throw new NotImplementedException();
        }
        private static bool CanBeParentInEditorElement(this Type type)
        {
            if (type.IsClass)
                return true;
            if (type.IsValueType && !type.IsEnum && !type.IsPrimitive)
                return true;
            return false;
        }
    }
}