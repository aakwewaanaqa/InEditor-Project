using InEditor.Editor.Class.Attribute;
using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(string))]
    public class IMGUITextField : IMGUIField<string>
    {
        protected override string Layout(string input)
        {
            return EditorGUILayout.TextField(Label, input);
        }
    }
}
