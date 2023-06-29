using System;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InEditor.Editor.Class.HandledMember
{
    public class HandledMemberTarget
    {
        /// <summary>
        /// Handles MemberInfo whether its a FieldInfo or PropertyInfo.
        /// </summary>
        private class HandledMemberInfo
        {
            /// <summary>
            /// Stored reflection info.
            /// </summary>
            private readonly MemberInfo member;

            /// <summary>
            /// Name of the MemberInfo.
            /// </summary>
            public string Name
            {
                get { return member.Name; }
            }

            /// <summary>
            /// Creates a handled MemberInfo for later reflection operations.
            /// </summary>
            /// <param name="member">stored info</param>
            public HandledMemberInfo(System.Reflection.MemberInfo member)
            {
                this.member = member;
            }

            /// <summary>
            /// The type of Field or Property.
            /// </summary>
            /// <exception cref="InvalidOperationException">It's not a FieldInfo or PropertyInfo.</exception>
            public Type MemberType
            {
                get
                {
                    switch (member)
                    {
                        case FieldInfo fieldInfo:
                            return fieldInfo.FieldType;
                        case PropertyInfo propertyInfo:
                            return propertyInfo.PropertyType;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            /// <summary>
            /// If this is Field or Property of UnityEngine.Object.
            /// </summary>
            public bool TypeIsObject
            {
                get
                {
                    return typeof(UnityEngine.Object).IsAssignableFrom(
                        MemberType);
                }
            }

            /// <summary>
            /// If this member can be <see cref="SerializedProperty"/> of the target.
            /// </summary>
            /// <param name="target">target to check</param>
            /// <returns></returns>
            public bool IsSerializedProperty(object target)
            {
                switch (target)
                {
                    case SerializedObject serializedObject:
                        return !(serializedObject.FindProperty(Name) is null);
                    case SerializedProperty serializedProperty:
                        return !(serializedProperty.FindPropertyRelative(Name)
                            is
                            null);
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Gets value from a target instance, if static pass null.
            /// </summary>
            /// <param name="target">target to get</param>
            /// <returns>generic value</returns>
            /// <exception cref="InvalidOperationException">It's not a field or property</exception>
            public object GetValue(object target)
            {
                switch (member)
                {
                    case FieldInfo fieldInfo:
                        return fieldInfo.GetValue(target);
                    case PropertyInfo propertyInfo:
                        return propertyInfo.GetValue(target);
                    default:
                        throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Sets value from a target instance, if static pass null.
            /// </summary>
            /// <param name="target">target to get</param>
            /// <param name="value">value to give to target</param>
            /// <exception cref="InvalidOperationException">It's not a field or property</exception>
            public void SetValue(object target, object value)
            {
                switch (member)
                {
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(target, value);
                        break;
                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(target, value);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private object rawTarget;
        private readonly HandledMemberInfo handledMember;
        public readonly bool IsMemberSerializedProperty;

        public Type MemberType
        {
            get { return handledMember.MemberType; }
        }

        public string Name
        {
            get { return handledMember.Name; }
        }

        public bool IsUnityObject
        {
            get { return handledMember.TypeIsObject; }
        }

        public HandledMemberTarget(object rawTarget,
            System.Reflection.MemberInfo member)
        {
            this.rawTarget = rawTarget;
            handledMember = new HandledMemberInfo(member);
            IsMemberSerializedProperty =
                handledMember.IsSerializedProperty(this.rawTarget);
        }

        public void Retarget(object target)
        {
            rawTarget = target;
        }

        public SerializedProperty FindProperty()
        {
            if (!IsMemberSerializedProperty)
                throw new InvalidOperationException();
            switch (rawTarget)
            {
                case SerializedObject serializedObject:
                    return serializedObject.FindProperty(handledMember.Name);
                case SerializedProperty property:
                    return property.FindPropertyRelative(handledMember.Name);
                default:
                    throw new InvalidOperationException();
            }
        }

        public object GetValue()
        {
            return IsMemberSerializedProperty
                ? FindProperty().boxedValue
                : handledMember.GetValue(rawTarget);
        }

        public void SetValue(object value)
        {
            if (IsMemberSerializedProperty)
                SetBoxedValue(FindProperty(), value);
            else
                handledMember.SetValue(rawTarget, value);
        }

        public object PassDown()
        {
            if (IsMemberSerializedProperty)
                return FindProperty();
            object target;
            switch (rawTarget)
            {
                case SerializedObject serializedObject:
                    target = serializedObject.targetObject;
                    break;
                case SerializedProperty serializedProperty:
                    target = serializedProperty.boxedValue;
                    break;
                default:
                    target = rawTarget;
                    break;
            }

            return handledMember.GetValue(target);
        }

        public void NullCheck()
        {
            if (IsMemberSerializedProperty)
                return;
            object target;
            switch (rawTarget)
            {
                case SerializedObject serializedObject:
                    target = serializedObject.targetObject;
                    break;
                case SerializedProperty serializedProperty:
                    target = serializedProperty.boxedValue;
                    break;
                default:
                    target = rawTarget;
                    break;
            }

            if (!(handledMember.GetValue(target) is null) || IsUnityObject)
                return;
            //GetUninitializedObject() is to deal with classes that don't have default ctor. Ex: string
            handledMember.SetValue(target,
                FormatterServices.GetUninitializedObject(MemberType));
        }

        private static PropertyInfo gradientValue
            = typeof(SerializedProperty).GetProperty("gradientValue",
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);
        
        /// <summary>
        /// Sets value of <see cref="SerializedProperty"/>; for boxedValue doesn't exist under version 2022...
        /// </summary>
        /// <param name="prop">the target property</param>
        /// <param name="value">the value to give</param>
        /// <exception cref="ArgumentOutOfRangeException">when it comes to array...</exception>
        private static void SetBoxedValue(SerializedProperty prop, object value)
        {
#if UNITY_2022_1_OR_NEWER
            prop.boxedValue = value;
#else
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = value as string;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = value as Object;
                    break;
                case SerializedPropertyType.LayerMask:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = (AnimationCurve)value;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Gradient:
                    gradientValue.SetValue(prop, value);
                    break;
                case SerializedPropertyType.ExposedReference:
                    prop.exposedReferenceValue = (Object)value;
                    break;
                case SerializedPropertyType.Vector2Int:
                    prop.vector2IntValue = (Vector2Int)value;
                    break;
                case SerializedPropertyType.Vector3Int:
                    prop.vector3IntValue = (Vector3Int)value;
                    break;
                case SerializedPropertyType.RectInt:
                    prop.rectIntValue = (RectInt)value;
                    break;
                case SerializedPropertyType.BoundsInt:
                    prop.boundsIntValue = (BoundsInt)value;
                    break;
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Quaternion:
                    prop.quaternionValue = (Quaternion)value;
                    break;
                case SerializedPropertyType.ManagedReference:
                    prop.managedReferenceValue = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif
        }
    }
}