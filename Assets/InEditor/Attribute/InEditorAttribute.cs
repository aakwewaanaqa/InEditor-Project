using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq.Expressions;

namespace InEditor
{
    public static class MemberInfoExtensions
    {
        public static bool TryGetAttribute<T>(this MemberInfo info, out T result) where T : Attribute
        {
            result = info.GetCustomAttribute<T>();
            return result is object;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class InEditorAttribute : PropertyAttribute
    {
        public int DisplayOrder = 0;

        public string DisplayName = string.Empty;
        public bool NicifyName = true;

        public InEditorHint DisplayHint = InEditorHint.Auto;

        public bool DisplayDisabled = false;
        public GUIStyle DisplayStyle = GUIStyle.none;

        public string Method = string.Empty;
        public string[] Parameters = new string[0];

        public bool HasName
        {
            get => !string.IsNullOrEmpty(DisplayName);
        }
        public bool HasStyle
        {
            get => !GUIStyle.none.Equals(DisplayStyle);
        }
        public bool HasMethod
        {
            get => !string.IsNullOrEmpty(Method);
        }
        public bool HasParameter
        {
            get => Parameters.Length > 0;
        }
    }

    public class InEditorMember : IComparable<InEditorMember>
    {
        private static readonly BindingFlags flags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private readonly InEditorAttribute inEditor;
        private readonly ReflectiveInfo reflect;
        private readonly int index;

        public InEditorAttribute InEditor
        {
            get => inEditor;
        }
        public string DisplayName
        {
            get
            {
                if (inEditor is object)
                {
                    var displayName = inEditor.HasName ? inEditor.DisplayName : reflect.Name;
                    return inEditor.NicifyName ? ObjectNames.NicifyVariableName(displayName) : displayName;
                }
                else
                {
                    return ObjectNames.NicifyVariableName(reflect.Name);
                }
            }
        }
        public bool IsSerialized
        {
            get => reflect.IsSerialized;
        }
        public bool IsInEditor
        {
            get => IsSerialized || inEditor is object;
        }

        private InEditorMember(MemberInfo memberInfo, int index)
        {
            this.reflect = new ReflectiveInfo(memberInfo);
            this.index = index;
            memberInfo.TryGetAttribute(out inEditor);
        }

        public static IEnumerable<InEditorMember> Reflect(Type type)
        {
            var infos = type.GetMembers(flags);
            var members = new List<InEditorMember>();
            foreach (var info in infos)
            {
                switch (info)
                {
                    case FieldInfo:
                    case PropertyInfo:
                        {
                            var member = new InEditorMember(info, members.Count);
                            if (member.IsInEditor)
                                members.Add(member);
                            break;
                        }
                    default:
                        break;
                }
            }
            members.Sort();
            return members;
        }

        private void ChangeValue<T>(object target, ChangeEvent<T> change)
        {
            reflect.SetValue(target, change.newValue);
        }
        private void RegisterChangeEvent<T>(object target, VisualElement element)
        {
            if (element is BaseField<T> b)
                b.value = (T)reflect.GetValue(target);
            var d = new EventCallback<ChangeEvent<T>>((change) => { reflect.SetValue(target, change.newValue); });
            element.RegisterCallback(d);
        }
        public VisualElement CreateElement(object target)
        {
            VisualElement element = default;
            if (reflect.MemberType == typeof(string))
            {
                element = new TextField(DisplayName);
                RegisterChangeEvent<string>(target, element);
            }
            else if (reflect.MemberType == typeof(float))
            {
                element = new FloatField(DisplayName);
                RegisterChangeEvent<float>(target, element);
            }
            else if (reflect.MemberType == typeof(Vector3))
            {
                element = new Vector3Field(DisplayName);
                RegisterChangeEvent<Vector3>(target, element);
            }
            return element;
        }

        public int CompareTo(InEditorMember other)
        {
            int a = inEditor is object ? inEditor.DisplayOrder : index;
            int b = other.inEditor is object ? other.inEditor.DisplayOrder : other.index;
            return a.CompareTo(b);
        }
    }
    public class ReflectiveInfo : ITargetGetSet
    {
        private readonly SerializeField serializeField;
        public readonly MemberInfo MemberInfo;
        public readonly Type MemberType;

        public bool IsSerialized
        {
            get
            {
                if (IsField)
                {
                    if (Field.IsPublic && !Field.IsStatic && Field.FieldType.IsSerializable)
                        return true;
                    else if (serializeField is object && !Field.IsStatic && Field.FieldType.IsSerializable)
                        return true;
                }
                return false;
            }
        }
        public string Name
        {
            get => MemberInfo.Name;
        }
        public FieldInfo Field
        {
            get => MemberInfo as FieldInfo;
        }
        public PropertyInfo Property
        {
            get => MemberInfo as PropertyInfo;
        }

        public ReflectiveInfo(MemberInfo info)
        {
            MemberInfo = info;
            if (info is FieldInfo field)
            {
                MemberInfo = field;
                MemberType = field.FieldType;
            }
            else if (info is PropertyInfo prop)
            {
                MemberInfo = prop;
                MemberType = prop.PropertyType;
            }
            MemberInfo.TryGetAttribute(out serializeField);
        }

        public bool IsField
        {
            get => MemberInfo is FieldInfo;
        }
        public bool IsProperty
        {
            get => MemberInfo is PropertyInfo;
        }

        public object GetValue(object target)
        {
            if (IsField)
            {
                return Field.GetValue(target);
            }
            else if (IsProperty)
            {
                if (Property.CanRead)
                    return Property.GetValue(target);
                else
                    throw new InvalidOperationException();
            }
            throw new InvalidOperationException();
        }
        public void SetValue(object target, object value)
        {
            if (IsField)
            {
                Field.SetValue(target, value);
            }
            else if (IsProperty)
            {
                if (Property.CanWrite)
                    Property.SetValue(target, value);
                else
                    throw new InvalidOperationException();
            }
        }
    }
    public interface ITargetGetSet
    {
        object GetValue(object target);
        void SetValue(object target, object value);
    }

    public enum InEditorHint
    {
        Auto,
        Label,
        Slider,
        TextArea,
        Tag,
        Object,
    }
}