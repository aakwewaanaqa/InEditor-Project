using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InEditor
{
    public sealed class IMGUIFold : IMGUIField<bool>
    {
        /// <summary>
        /// When the target is not serialized, store temporary data here.
        /// </summary>
        private bool fold;

        protected override bool GetValue()
        {
            return Target.IsMemberSerializedProperty ? Target.FindProperty().isExpanded : fold;
        }

        protected override void SetValue(bool value)
        {
            if (Target.IsMemberSerializedProperty)
                Target.FindProperty().isExpanded = value;
            else
                fold = value;
        }

        protected override bool Layout(bool value)
        {
            return EditorGUILayout.Foldout(value, Label);
        }

        public override bool IsExpended
        {
            get => GetValue();
        }
    }
}
