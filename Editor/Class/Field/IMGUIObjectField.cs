using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Object), IsAlsoInherited = true)]
    public sealed class IMGUIObjectField : IMGUIField<Object>
    {
        protected override Object Layout(Object value)
        {
            return EditorGUILayout.ObjectField(Label, value, Target.MemberType, true);
        }
    }
}