using InEditor.Editor.Class.Attribute;
using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(double))]
    public class IMGUIDoubleField : IMGUIField<double>
    {
        protected override double Layout(double input)
        {
            return EditorGUILayout.DoubleField(Label, input);
        }
    }
}