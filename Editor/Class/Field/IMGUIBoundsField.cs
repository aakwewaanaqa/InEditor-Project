using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Bounds))]
    public class IMGUIBoundsField : IMGUIField<Bounds>
    {
        protected override Bounds Layout(Bounds input)
        {
            return EditorGUILayout.BoundsField(Label, input);
        }
    }
}