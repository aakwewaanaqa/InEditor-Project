using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InEditor
{
    public sealed class IMGUIToggleField : IMGUIField<bool>
    {
        public IMGUIToggleField(object target, MemberInfo member) : base(target, member) { }
        protected override bool Layout(bool value)
        {
            return EditorGUILayout.Toggle(Label, value);
        }
    }
}