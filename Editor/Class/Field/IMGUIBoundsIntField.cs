using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(BoundsInt))]
    public class IMGUIBoundsIntField : IMGUIField<BoundsInt>
    {
        protected override BoundsInt Layout(BoundsInt input)
        {
            return EditorGUILayout.BoundsIntField(Label, input);
        }
    }
}