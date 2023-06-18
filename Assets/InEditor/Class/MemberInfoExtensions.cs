using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace InEditor
{
    public static class TypeExtensions
    {
        public static readonly Type[] UnitySerializedTypes = new Type[]
        {
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
        public static VisualElement ToVisualElementField(this Type type)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return new ObjectField() { objectType = type };
            else if (type.IsEnum)
                return new EnumField((Enum)Activator.CreateInstance(type));
            else if (type == typeof(int))
                return new IntegerField();
            else if (type == typeof(long))
                return new LongField();
            else if (type == typeof(float))
                return new FloatField();
            else if (type == typeof(double))
                return new DoubleField();
            else if (type == typeof(char))
                return new TextField();
            else if (type == typeof(string))
                return new TextField();
            else if (type == typeof(Hash128))
                return new Hash128Field();
            else if (type == typeof(Vector2))
                return new Vector2Field();
            else if (type == typeof(Vector2Int))
                return new Vector2IntField();
            else if (type == typeof(Vector3))
                return new Vector3Field();
            else if (type == typeof(Vector3Int))
                return new Vector3IntField();
            else if (type == typeof(Vector4) || type == typeof(Quaternion))
                return new Vector4Field();
            else if (type == typeof(Rect))
                return new RectField();
            else if (type == typeof(RectInt))
                return new RectIntField();
            else if (type == typeof(Bounds))
                return new BoundsField();
            else if (type == typeof(BoundsInt))
                return new BoundsIntField();
            else if (type == typeof(Color) || type == typeof(Color32))
                return new ColorField();
            else if (type == typeof(LayerMask))
                return new LayerMaskField();
            else if (type == typeof(Gradient))
                return new GradientField();
            else if (type == typeof(AnimationCurve))
                return new CurveField();
            return new VisualElement();
        }
        public static bool CanBeSerializedInUnity(this Type type)
        {
            if (type.TryGetAttribute(out SerializableAttribute _))
                return true;
            else
                return type.IsUnitySerializableType();
        }
        public static bool IsUnitySerializableType(this Type type)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                return true;
            else if (type.IsEnum)
                return true;
            else
                return UnitySerializedTypes.Contains(type);
        }
        public static bool CanBeParentInEditorElement(this Type type)
        {
            if (typeof(IList).IsAssignableFrom(type))
                return false;
            else if (UnitySerializedTypes.Contains(type))
                return false;
            else if (type.IsClass)
                return true;
            else if (type.IsValueType)
                return true;
            return false;
        }
    }
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