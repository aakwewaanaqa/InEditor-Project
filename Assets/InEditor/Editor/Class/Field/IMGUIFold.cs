using System.Reflection;
using UnityEditor;

namespace InEditor
{
    public sealed class IMGUIFold : IMGUIField<bool>
    {
        /// <summary>
        /// When the target is not serialized, store temporary data here.
        /// </summary>
        private bool fold;
        
        public IMGUIFold(object target, MemberInfo member) : base(target, member) { }

        protected override bool GetValue()
        {
            return target.IsSerialized ? target.Find(member).isExpanded : fold;
        }
        protected override void SetValue(bool value)
        {
            if (target.IsSerialized)
                target.Find(member).isExpanded = value;
            else
                fold = value;
        }
        protected override bool Layout(bool value)
        {
            return EditorGUILayout.Foldout(value, Label);
        }
    }
}