using InEditor.Editor.Class.Attribute;
using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    [IMGUIField(typeof(AnimationCurve))]
    public sealed class IMGUIAnimationCurveField : IMGUIField<AnimationCurve>
    {
        protected override AnimationCurve Layout(AnimationCurve value)
        {
            return EditorGUILayout.CurveField(Label, value);
        }
    }
}