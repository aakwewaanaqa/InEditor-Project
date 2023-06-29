using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InEditor.Editor.Class.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Type[] UnitySerializedTypes = {
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(string),
            typeof(Hash128),
            typeof(Vector2),
            typeof(Vector2Int),
            typeof(Vector3),
            typeof(Vector3Int),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Rect),
            typeof(RectInt),
            typeof(Bounds),
            typeof(BoundsInt),
            typeof(Color),
            typeof(Color32),
            typeof(LayerMask),
            typeof(Gradient),
            typeof(AnimationCurve),
            typeof(UnityEngine.Object),
        };
        public static bool CanBeParentInEditorElement(this Type type)
        {
            if (typeof(IList).IsAssignableFrom(type))
                return false;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return false;
            if (type.IsPrimitive)
                return false;
            if (UnitySerializedTypes.Contains(type))
                return false;
            return type.IsClass || type.IsValueType;
        }
        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>() where TAttribute : System.Attribute
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.TryGetAttribute(out TAttribute _));
        }
    }
}
