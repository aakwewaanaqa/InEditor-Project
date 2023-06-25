using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Color32))]
    public sealed class IMGUIColor32Field : IMGUIField<Color32>
    {
        protected override Color32 GetValue()
        {
            var v = Target.GetValue();
            return v switch
            {
                Color c => new Color32(
                    (byte)Mathf.RoundToInt(c.r * 255f),
                    (byte)Mathf.RoundToInt(c.g * 255f),
                    (byte)Mathf.RoundToInt(c.b * 255f),
                    (byte)Mathf.RoundToInt(c.a * 255f)),
                _ => (Color32)v,
            };
        }

        protected override Color32 Layout(Color32 value)
        {
            return EditorGUILayout.ColorField(Label, value);
        }
    }
}
