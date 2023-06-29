using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Vector2Int))]
    public class IMGUIVector2IntField : IMGUIField<Vector2Int>
    {
        protected override Vector2Int Layout(Vector2Int input)
        {
            return EditorGUILayout.Vector2IntField(Label, input);
        }
    }
}
