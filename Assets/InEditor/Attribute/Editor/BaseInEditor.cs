using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InEditor
{
    public class BaseInEditor : Editor
    {
        protected Type type;
        protected IEnumerable<InEditorMember> members;
        protected virtual void OnEnable()
        {
            type = target.GetType();
            members = InEditorMember.Reflect(type);
        }
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            foreach (var member in members)
                root.Add(member.CreateElement(target));
            return root;
        }
    }
}