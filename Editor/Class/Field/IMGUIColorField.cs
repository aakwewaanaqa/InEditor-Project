using System;
using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(Color))]
    public sealed class IMGUIColorField : IMGUIField<Color>
    {
        protected override Color Layout(Color value)
        {
            return EditorGUILayout.ColorField(Label, value);
        }
    }
}
