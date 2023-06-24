using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    /// <summary>
    /// IMGUI Drawing base class of type T.
    /// </summary>
    /// <typeparam name="T">the field's dealing type T</typeparam>
    public abstract class IMGUIField<T> : IMGUIField
    {
        /// <summary>
        /// Used in Editor.OnInspectorGUI()
        /// to draw EditorGUILayout
        /// </summary>
        public override void Layout()
        {
            using var scope = new EditorGUI.ChangeCheckScope();

            var value = Layout(GetValue());
            if (scope.changed)
                SetValue(value);
        }

        public override bool IsExpended
        {
            get => false;
        }

        /// <summary>
        /// Used in Layout()
        /// to set value with IMGUIFieldInfo via FieldTarget.
        /// </summary>
        /// <param name="value">passed value</param>
        protected virtual void SetValue(T value)
        {
            Target.SetValue(value);
        }

        /// <summary>
        /// Used in Layout()
        /// to get value with IMGUIFieldInfo via FieldTarget.
        /// </summary>
        protected virtual T GetValue()
        {
            return (T)Target.GetValue();
        }

        /// <summary>
        /// To implement, override it with EditorGUILayout of the field
        /// </summary>
        /// <param name="input"> value input </param>
        /// <returns> value output </returns>
        protected abstract T Layout(T input);
    }
}
