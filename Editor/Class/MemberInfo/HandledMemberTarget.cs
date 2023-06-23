using System;
using System.Reflection;
using UnityEditor;

namespace InEditor
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
            public HandledMemberInfo(MemberInfo member)
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
                    return member switch
                    {
                        FieldInfo fieldInfo => fieldInfo.FieldType,
                        PropertyInfo propertyInfo => propertyInfo.PropertyType,
                        _ => throw new InvalidOperationException(),
                    };
                }
            }
            /// <summary>
            /// If this is Field or Property of UnityEngine.Object.
            /// </summary>
            public bool TypeIsObject
            {
                get { return typeof(UnityEngine.Object).IsAssignableFrom(MemberType); }
            }
            /// <summary>
            /// If this member can be <see cref="SerializedProperty"/> of the target.
            /// </summary>
            /// <param name="target">target to check</param>
            /// <returns></returns>
            public bool IsSerializedProperty(object target)
            {
                return target switch
                {
                    SerializedObject serializedObject => serializedObject.FindProperty(Name) is not null,
                    SerializedProperty serializedProperty => serializedProperty.FindPropertyRelative(Name) is not null,
                    _ => false,
                };
            }
            /// <summary>
            /// Gets value from a target instance, if static pass null.
            /// </summary>
            /// <param name="target">target to get</param>
            /// <returns>generic value</returns>
            /// <exception cref="InvalidOperationException">It's not a field or property</exception>
            public object GetValue(object target)
            {
                return member switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(target),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(target),
                    _ => throw new InvalidOperationException(),
                };
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

        public HandledMemberTarget(object rawTarget, MemberInfo member)
        {
            this.rawTarget = rawTarget;
            handledMember = new HandledMemberInfo(member);
            IsMemberSerializedProperty = handledMember.IsSerializedProperty(this.rawTarget);
        }

        public void Retarget(object target)
        {
            rawTarget = target;
        }

        public SerializedProperty FindProperty()
        {
            if (!IsMemberSerializedProperty)
                throw new InvalidOperationException();
            return rawTarget switch
            {
                SerializedObject serializedObject => serializedObject.FindProperty(handledMember.Name),
                SerializedProperty property => property.FindPropertyRelative(handledMember.Name),
                _ => throw new InvalidOperationException()
            };
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
                FindProperty().boxedValue = value;
            else
                handledMember.SetValue(rawTarget, value);
        }

        public object PassDown()
        {
            if (IsMemberSerializedProperty)
                return FindProperty();
            var target = rawTarget switch
            {
                SerializedObject serializedObject => serializedObject.targetObject,
                SerializedProperty serializedProperty => serializedProperty.boxedValue,
                _ => rawTarget,
            };
            return handledMember.GetValue(target);
        }

        public void NullCheck()
        {
            if (IsMemberSerializedProperty)
                return;
            var target = rawTarget switch
            {
                SerializedObject serializedObject => serializedObject.targetObject,
                SerializedProperty serializedProperty => serializedProperty.boxedValue,
                _ => rawTarget,
            };
            if (handledMember.GetValue(target) is not null || IsUnityObject)
                return;
            handledMember.SetValue(target, Activator.CreateInstance(MemberType));
        }
    }
}
