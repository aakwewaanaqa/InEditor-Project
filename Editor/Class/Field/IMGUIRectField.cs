using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Rect))]
    public class IMGUIRectField : IMGUIField<Rect>
    {
        protected override Rect Layout(Rect input)
        {
            return EditorGUILayout.RectField(Label, input);
        }
    }
}