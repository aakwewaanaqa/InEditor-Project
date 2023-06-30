using System;
using System.Collections.Generic;
using System.Linq;
using InEditor.Editor.Class.Attribute;
using InEditor.Editor.Class.Extensions;
using InEditor.Editor.Class.HandledMember;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    /// <summary>
    /// The abstract base class for all fields of IMGUIField to implemented by,
    /// 'cause the abstract using of everywhere.
    /// </summary>
    public abstract class IMGUIField
    {
        /// <summary>
        /// Gets and sets value whether target is SerializedObject, SerializedProperty or raw object...
        /// </summary>
        protected HandledMemberTarget Target { get; private set; }

        /// <summary>
        /// The drawn label to display in IMGUI field.
        /// </summary>
        protected GUIContent Label { get; private set; }

        /// <summary>
        /// Stored types that have IMGUIFieldAttribute with dictionary to reduce search time.
        /// </summary>
        private static readonly Dictionary<Type, Type> IMGUITables;

        /// <summary>
        /// Creates static field first.
        /// </summary>
        static IMGUIField()
        {
            IMGUITables = new Dictionary<Type, Type>();
            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .ToList()
                .ForEach(type =>
                {
                    if (!type.TryGetAttribute(out IMGUIFieldAttribute att))
                        return;
                    foreach (var target in att.Targets)
                        IMGUITables.Add(target, type);
                });
        }

        /// <summary>
        /// Finds suitable target type of an IMGUIField type.
        /// </summary>
        /// <param name="target">target type</param>
        /// <returns>IMGUIField[T] type</returns>
        private static Type FindIMGUIType(Type target)
        {
            try
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(target))
                    return typeof(IMGUIObjectField);
                return IMGUITables[target];
            }
            catch (Exception e)
            {
                Debug.LogError($"{target} : {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates type of IMGUIField[T].
        /// </summary>
        /// <param name="target">target to tracked</param>
        /// <param name="label">label to draw with IMGUIField</param>
        /// <param name="hint">hints for draw style change</param>
        /// <returns></returns>
        public static IMGUIField CreateIMGUI(HandledMemberTarget target,
            GUIContent label, IMGUIDrawHintEnum hint)
        {
            var type = target.MemberType;

            IMGUIField imgui = null;

            switch (hint)
            {
                default:
                case IMGUIDrawHintEnum.Auto:
                    var imguiType = type.IsParentInEditorElement()
                        ? typeof(IMGUIFold)
                        : FindIMGUIType(type);
                    
                    // Can't figure out what to draw with
                    if (imguiType is null)
                        return null;
                    
                    imgui = target.IsMemberSerializedProperty
                        ? new IMGUIPropertyField()
                        : (IMGUIField)Activator.CreateInstance(imguiType);
                    break;
                
                case IMGUIDrawHintEnum.ToStringLabel:
                    imgui = new IMGUILabel();
                    break;
                
                case IMGUIDrawHintEnum.None:
                    return null;
            }

            imgui.Target = target;
            imgui.Label = label;
            return imgui;
        }

        /// <summary>
        /// Used in BaseInEditor.OnInspectorGUI()
        /// to draw EditorGUILayout of this IMGUIField.
        /// </summary>
        public abstract void Layout();

        /// <summary>
        /// Used in InEditorElement.OnInspectorGUI()
        /// to get if the property is expanded or not.
        /// </summary>
        public abstract bool IsExpended { get; set; }
    }
}