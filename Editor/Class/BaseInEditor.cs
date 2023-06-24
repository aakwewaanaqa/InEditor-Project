using System;
using System.Collections.Generic;
using System.Linq;
using InEditor.Editor.Class.Element;
using InEditor.Runtime.Interface;

namespace InEditor.Editor.Class
{
    /// <summary>
    /// The base editor to be inherited if a Monobehaviour we want it the draw with InEditorAttribute
    /// </summary>
    public class BaseInEditor : UnityEditor.Editor
    {
        /// <summary>
        /// The inspected target type
        /// </summary>
        private Type type;

        /// <summary>
        /// Stores attributes of 
        /// </summary>
        private IEnumerable<IOnInspectorGUIEditor> onInspectorGUIEditors;

        /// <summary>
        /// Stores all the reflected members.
        /// </summary>
        protected IEnumerable<InEditorElement> Members;

        protected virtual void OnEnable()
        {
            // Gets the target type.
            type = target.GetType();
            // Gets attributes that is IOnInspectorGUI.
            onInspectorGUIEditors = type.GetCustomAttributes(false).OfType<IOnInspectorGUIEditor>();
            // Reflects all the fields and properties in the target's type.
            Members = InEditorElement.Reflect(serializedObject, type, null);
        }
        public override void OnInspectorGUI()
        {
            // Updates all SerializedProperty.
            serializedObject.Update();
            // Draws all the member of InEditorElement.
            foreach (var member in Members)
                member.OnInspectorGUI();
            // Executes all the attributes that bound to this target class.
            foreach (var attribute in onInspectorGUIEditors)
                attribute.OnInspectorGUI(this);
            // Apply changes that happens on any SerializedProperty.
            serializedObject.ApplyModifiedProperties();
        }
    }
}
