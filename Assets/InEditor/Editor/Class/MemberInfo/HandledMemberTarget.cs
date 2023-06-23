using System;
using System.Reflection;
using UnityEditor;

namespace InEditor
{
    public class HandledMemberTarget
    {
        private class HandledMemberInfo
        {
            private readonly MemberInfo member;

            public string Name
            {
                get => member.Name;
            }

            public HandledMemberInfo(MemberInfo member)
            {
                this.member = member;
            }

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

            public bool TypeIsObject
            {
                get => typeof(UnityEngine.Object).IsAssignableFrom(MemberType);
            }

            public bool IsSerializedProperty(object target)
            {
                return target switch
                {
                    SerializedObject serializedObject => serializedObject.FindProperty(Name) is object,
                    SerializedProperty serializedProperty => serializedProperty.FindPropertyRelative(Name) is object,
                    _ => false,
                };
            }

            public object GetValue(object target)
            {
                return member switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(target),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(target),
                    _ => throw new InvalidOperationException(),
                };
            }

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
            get => handledMember.MemberType;
        }
        public string Name
        {
            get
            {
                return handledMember.Name;
            }
        }
        public bool IsUnityObject
        {
            get => handledMember.TypeIsObject;
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
