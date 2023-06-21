using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace InEditor
{
    /// <summary>
    /// Attribute to mark field, property, method of a Type to create VisualElement at ease...
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class InEditorAttribute : PropertyAttribute
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
        public IMGUIDrawFieldEnum DesignatedIMGUI = IMGUIDrawFieldEnum.Auto;

        /// <summary>
        /// If [DisplayName] is not empty or null.
        /// </summary>
        public bool IsRenamed
        {
            get => !string.IsNullOrEmpty(DisplayName);
        }

    }
}