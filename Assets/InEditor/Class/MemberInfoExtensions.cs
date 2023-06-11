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
        private static bool TryGetAttribute<T>(this Type type, out T result) where T : Attribute
        {
            result = type.GetCustomAttribute<T>();
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
            if (info is FieldInfo field)
            {
                if (!field.IsStatic && field.IsPublic && field.FieldType.CanBeSerializedInUnity())
                    return true;
            }
            return false;
        }
        private static bool CanBeSerializedInUnity(this Type type)
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
    }
}