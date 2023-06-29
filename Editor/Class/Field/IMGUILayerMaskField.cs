using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(LayerMask))]
    public class IMGUILayerMaskField : IMGUIField<LayerMask>
    {
        protected override LayerMask Layout(LayerMask input)
        {
            return EditorGUILayout.LayerField(Label, input);
        }
    }
}