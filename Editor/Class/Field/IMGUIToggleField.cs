using InEditor.Editor.Class.Attribute;
using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(bool))]
    public sealed class IMGUIToggleField : IMGUIField<bool>
    {
        protected override bool Layout(bool value)
        {
            return EditorGUILayout.Toggle(Label, value);
        }
    }
}
