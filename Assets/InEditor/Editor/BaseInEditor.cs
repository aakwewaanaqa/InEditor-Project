using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InEditor
{
    /// <summary>
    /// The base editor to be inherited if a Monobehaviour we want it the draw with InEditorAttribute
    /// </summary>
    public class BaseInEditor : Editor
    {
        /// <summary>
        /// the inspected target type
        /// </summary>
        protected Type Type;
        /// <summary>
        /// Stores all the reflected members.
        /// </summary>
        protected IEnumerable<InEditorElement> Members;
        protected virtual void OnEnable()
        {
            Type = target.GetType();
            Members = InEditorElement.Reflect(serializedObject, Type, null);
        }
        protected virtual void OnDisable()
        {
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            foreach (var member in Members)
                member.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}