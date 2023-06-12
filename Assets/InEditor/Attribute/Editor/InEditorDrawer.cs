using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace InEditor
{
    [InEditor]
    [CustomPropertyDrawer(typeof(InEditorAttribute))]
    public class InEditorDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }
    }
}