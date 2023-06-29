using InEditor.Editor.Class.Attribute;
using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(float))]
    public sealed class IMGUIFloatField : IMGUIField<float>
    {
        protected override float Layout(float input)
        {
            return EditorGUILayout.FloatField(Label, input);
        }
    }
}
