using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Vector3))]
    public class IMGUIVector3Field : IMGUIField<Vector3>
    {
        protected override Vector3 Layout(Vector3 input)
        {
            return EditorGUILayout.Vector3Field(Label, input);
        }
    }
}
