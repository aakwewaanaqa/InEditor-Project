using System;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.HandledMember
{
    /// <summary>
    /// 
    /// </summary>
    public class HandledMemberTarget
    {
        /// <summary>
        /// Handles <see cref="MemberInfo"/> whether its a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/>.
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

        /// <summary>
        /// The not nicify name of <see cref="MemberInfo"/>
        /// </summary>
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

        /// <summary>
        /// Gets value by the <see cref="MemberInfo"/> to <see cref="rawTarget"/> passed in.
        /// </summary>
        /// <returns>returned value</returns>
        public object GetValue()
        {
            return IsMemberSerializedProperty
                ? GetBoxedValue(FindProperty())
                : handledMember.GetValue(rawTarget);
        }

        /// <summary>
        /// Sets value by the <see cref="MemberInfo"/> to <see cref="rawTarget"/> passed in.
        /// <param name="value">value to set</param>
        /// </summary>
        public void SetValue(object value)
        {
            if (IsMemberSerializedProperty)
                SetBoxedValue(FindProperty(), value);
            else
                handledMember.SetValue(rawTarget, value);
        }

        /// <summary>
        /// This transform <see cref="SerializedObject"/> or <see cref="SerializedProperty"/> to real object
        /// </summary>
        /// <returns></returns>
        public object GetRealTarget()
        {
            switch (rawTarget)
            {
                case SerializedObject serializedObject:
                    return serializedObject.targetObject;
                case SerializedProperty serializedProperty:
                    return GetBoxedValue(serializedProperty);
                default:
                    return rawTarget;
            }
        }

        /// <summary>
        /// Gets the <see cref="rawTarget"/> for the next deeper <see cref="Element.InEditorElement"/> to ctor with.
        /// </summary>
        /// <returns></returns>
        public object PassDown()
        {
            return IsMemberSerializedProperty 
                ? FindProperty() 
                : handledMember.GetValue(GetRealTarget());
        }

        /// <summary>
        /// Checks if the true value of the target is null.
        /// Avoid getting <see cref="NullReferenceException"/> when creating <see cref="Element.InEditorElement.ElementHierarchy.relatives"/>
        /// </summary>
        public void NullCheck()
        {
            // Serialized property will auto-null-check itself.
            if (IsMemberSerializedProperty)
                return;

            var target = GetRealTarget();

            if (!(handledMember.GetValue(target) is null) || IsUnityObject)
                return;

            var newValue = MemberType == typeof(Gradient)
                //Found that gradient without ctor will crash the Unity Editor
                ? new Gradient()
                //GetUninitializedObject() is to deal with classes that don't have default ctor. Ex: string
                : FormatterServices.GetUninitializedObject(MemberType);
            handledMember.SetValue(target, newValue);
        }

        /// <summary>
        /// Gets gradient by property in <see cref="SerializedProperty"/>
        /// </summary>
        private static PropertyInfo gradientValue
            = typeof(SerializedProperty).GetProperty("gradientValue",
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);

        /// <summary>
        /// Sets value of <see cref="SerializedProperty"/>; for boxedValue doesn't exist under version 2022...
        /// </summary>
        /// <param name="prop">the target property</param>
        /// <param name="value">the value to give</param>
        /// <exception cref="ArgumentOutOfRangeException">when it comes to not supported type</exception>
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
                case SerializedPropertyType.Quaternion:
                    prop.quaternionValue = (Quaternion)value;
                    break;
                case SerializedPropertyType.ManagedReference:
                    prop.managedReferenceValue = value;
                    break;

                default:
                case SerializedPropertyType.Generic:
                    throw new NotImplementedException(
                        "Until UNITY_2022_1_OR_NEWER");
            }
#endif
        }

        /// <summary>
        /// Gets value of <see cref="SerializedProperty"/>; for boxedValue doesn't exist under version 2022...
        /// </summary>
        /// <param name="prop">the target property</param>
        /// <exception cref="ArgumentOutOfRangeException">when it comes to not supported type</exception>
        private static object GetBoxedValue(SerializedProperty prop)
        {
#if UNITY_2022_1_OR_NEWER
            return prop.boxedValue;
#else
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return prop.intValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                case SerializedPropertyType.ArraySize:
                    return prop.intValue;
                case SerializedPropertyType.Character:
                    return prop.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                case SerializedPropertyType.Gradient:
                    return gradientValue.GetValue(prop);
                case SerializedPropertyType.ExposedReference:
                    return prop.exposedReferenceValue;
                case SerializedPropertyType.Vector2Int:
                    return prop.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return prop.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return prop.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return prop.boundsIntValue;
                case SerializedPropertyType.Quaternion:
                    return prop.quaternionValue;

                default:
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.ManagedReference:
                    throw new NotImplementedException(
                        "Until UNITY_2022_1_OR_NEWER");
            }
#endif
        }
    }
}
