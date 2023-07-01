using System;

namespace InEditor
{
    /// <summary>
    /// Attribute to mark field, property, method of a Type to create VisualElement at ease...
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class, AllowMultiple = false)]
    public class InEditorAttribute : Attribute
    {
        /// <summary>
        /// Marks displaying order by this.
        /// </summary>
        public int DisplayOrder = 0;

        /// <summary>
        /// Marks custom name by this.
        /// </summary>
        public string DisplayName = string.Empty;

        /// <summary>
        /// Marks weather making display name looks nicer by this.
        /// </summary>
        public bool NicifyName = true;

        /// <summary>
        /// Marks displaying of the IMGUI to be a certain type.
        /// </summary>
        public IMGUIDrawHintEnum Hint = IMGUIDrawHintEnum.Auto;

        /// <summary>
        /// Marks displaying of the value as read-only
        /// </summary>
        public bool Disabled = false;

        /// <summary>
        /// Marks displaying of the field to use a pre-define method in Editor or Monobehaviour.
        /// </summary>
        public string Method = string.Empty;

        /// <summary>
        /// Checks displaying with custom method or not.
        /// This will disable auto drawing and data handling be careful with this.
        /// </summary>
        public bool HasCustomMethod
        {
            get { return !string.IsNullOrEmpty(Method); }
        }


        /// <summary>
        /// Mark it! Marks a field or property to draw in BaseInEditor Editor class.
        /// Usually with an order is the best to sort drawing.
        /// </summary>
        /// <param name="order">display order, smaller first.</param>
        /// <param name="canEdit">whether the field will shown as editable field</param>
        public InEditorAttribute(int order = 0, bool canEdit = true)
        {
            DisplayOrder = order;
            Disabled = !canEdit;
        }
    }
}
