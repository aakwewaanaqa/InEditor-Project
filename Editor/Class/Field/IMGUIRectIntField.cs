using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(RectInt))]
    public class IMGUIRectIntField : IMGUIField<RectInt>
    {
        protected override RectInt Layout(RectInt input)
        {
            return EditorGUILayout.RectIntField(Label, input);
        }
    }
}