using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Vector2))]
    public class IMGUIVector2Field : IMGUIField<Vector2>
    {
        protected override Vector2 Layout(Vector2 input)
        {
            return EditorGUILayout.Vector2Field(Label, input);
        }
    }
}