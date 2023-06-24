using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Vector3Int))]
    public class IMGUIVector3IntField : IMGUIField<Vector3Int>
    {
        protected override Vector3Int Layout(Vector3Int input)
        {
            return EditorGUILayout.Vector3IntField(Label, input);
        }
    }
}