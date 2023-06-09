using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace InEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class InEditorAttribute : PropertyAttribute
    {
        public bool IsSerialized = false;

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
                    var displayName = inEditor.HasName ? inEditor.DisplayName : memberInfo.Name;
                    return inEditor.NicifyName ? ObjectNames.NicifyVariableName(displayName) : displayName;
                }
                else
                {
                    return ObjectNames.NicifyVariableName(memberInfo.Name);
                }
            }
        }
        public bool IsSerialized
        { 
            get
            {
                if (inEditor is object)
                {
                    return inEditor.IsSerialized;
                }
                else
                {
                    if (memberInfo is FieldInfo field)
                    {
                        if (field.IsPublic && !field.IsStatic && field.FieldType.IsSerializable)
                            return true;
                        else if (serializeField is object && !field.IsStatic && field.FieldType.IsSerializable)
                            return true;
                    }
                    return false;
                }
            }
        }
        public bool IsInEditor
        {
            get
            {
                return IsSerialized || inEditor is object;
            }
        }

        private static readonly BindingFlags flags = 
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private readonly SerializeField serializeField;
        private readonly InEditorAttribute inEditor;

        private readonly MemberInfo memberInfo;
        private readonly int index;
        
        private InEditorMember(MemberInfo memberInfo, int index)
        {
            this.memberInfo = memberInfo;
            this.index = index;
            TryGetAttribute(memberInfo, out serializeField);
            TryGetAttribute(memberInfo, out inEditor);
        }

        private static bool TryGetAttribute<T>(MemberInfo info, out T result) where T : Attribute
        {
            result = info.GetCustomAttribute<T>();
            return result is object;
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
                    case MethodInfo:
                        var member = new InEditorMember(info, members.Count);
                        if (member.IsInEditor)
                            members.Add(member);
                        break;
                    default:
                        break;
                }
            }
            members.Sort();
            return members;
        }

        public int CompareTo(InEditorMember other)
        {
            int a = inEditor is object ? inEditor.DisplayOrder : index;
            int b = other.inEditor is object ? other.inEditor.DisplayOrder : other.index;
            return a.CompareTo(b);
        }
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