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
        protected Type type;
        /// <summary>
        /// Stores all the reflected members.
        /// </summary>
        protected IEnumerable<InEditorElement> members;
        protected virtual void OnEnable()
        {
            type = target.GetType();
            members = InEditorElement.Reflect(target, type);
        }
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            foreach (var member in members)
                root.Add(member.CreateElement());
            return root;
        }
    }
}