using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Gradient))]
    public sealed class IMGUIGradientField : IMGUIField<Gradient>
    {
        protected override Gradient Layout(Gradient value)
        {
            return EditorGUILayout.GradientField(Label, value);
        }
    }
}
