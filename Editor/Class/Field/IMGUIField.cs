using System;
using System.Collections.Generic;
using System.Linq;
using InEditor.Editor.Class.Attribute;
using InEditor.Editor.Class.Extensions;
using InEditor.Editor.Class.HandledMember;
using PlasticGui;
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
        /// Stored types that have IMGUIFieldAttribute
        /// </summary>
        private static readonly IEnumerable<Type> IMGUITypes =
            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.TryGetAttribute(out IMGUIFieldAttribute _));

        /// <summary>
        /// Finds suitable target type of an IMGUIField type.
        /// </summary>
        /// <param name="target">target type</param>
        /// <returns>IMGUIField[T] type</returns>
        private static Type FindIMGUIType(Type target)
        {
            try
            {
                return IMGUITypes.First(t =>
                    t.TryGetAttribute(out IMGUIFieldAttribute att) &&
                    att.Match(target));
            }
            catch (Exception e)
            {
                Debug.LogError($"{target} : {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates type of IMGUIField[T].
        /// </summary>
        /// <param name="target">target to tracked</param>
        /// <param name="label">label to draw with IMGUIField</param>
        /// <returns></returns>
        public static IMGUIField CreateIMGUI(HandledMemberTarget target, GUIContent label)
        {
            var type = target.MemberType;
            var imguiType = type.IsParentInEditorElement() ? typeof(IMGUIFold) : FindIMGUIType(type);
            var imgui = (IMGUIField)Activator.CreateInstance(imguiType);
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
        public abstract bool IsExpended { get; }
    }
}
