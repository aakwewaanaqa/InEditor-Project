using InEditor.Editor.Class.Attribute;
using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(long))]
    public class IMGUILongField : IMGUIField<long>
    {
        protected override long Layout(long input)
        {
            return EditorGUILayout.LongField(Label, input);
        }
    }
}