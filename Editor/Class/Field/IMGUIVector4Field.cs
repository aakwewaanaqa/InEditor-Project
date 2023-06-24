using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Vector4))]
    public class IMGUIVector4Field : IMGUIField<Vector4>
    {
        protected override Vector4 Layout(Vector4 input)
        {
            return EditorGUILayout.Vector4Field(Label, input);
        }
    }
}