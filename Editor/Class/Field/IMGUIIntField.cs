using InEditor.Editor.Class.Attribute;
using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(int))]
    public class IMGUIIntField : IMGUIField<int>
    {
        protected override int Layout(int input)
        {
            return EditorGUILayout.IntField(Label, input);
        }
    }
}
