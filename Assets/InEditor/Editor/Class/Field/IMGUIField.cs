using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InEditor
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
        /// Pairing types with IMGUIField[T]
        /// <br>
        /// Planed to use attribute instead listing in Dictionary...
        /// </br>
        /// </summary>
        private static readonly Dictionary<Type, Type> IMGUIPairs
            = new Dictionary<Type, Type>()
            {
                { typeof(bool), typeof(IMGUIToggleField) }

                //{ typeof(int), IMGUIDrawFieldEnum.Int },
                //{ typeof(long), IMGUIDrawFieldEnum.Int },

                //{ typeof(float), IMGUIDrawFieldEnum.Float },
                //{ typeof(double), IMGUIDrawFieldEnum.Float },
                //{ typeof(decimal), IMGUIDrawFieldEnum.Float },

                //{ typeof(string), IMGUIDrawFieldEnum.Text },

                //{ typeof(Rect), IMGUIDrawFieldEnum.Rect },
                //{ typeof(RectInt), IMGUIDrawFieldEnum.RectInt },

                //{ typeof(Bounds), IMGUIDrawFieldEnum.Bounds },
                //{ typeof(BoundsInt), IMGUIDrawFieldEnum.BoundsInt },

                //{ typeof(Color), IMGUIDrawFieldEnum.Color },
                //{ typeof(Color32), IMGUIDrawFieldEnum.Color },

                //{ typeof(Vector2), IMGUIDrawFieldEnum.Vector2 },
                //{ typeof(Vector2Int), IMGUIDrawFieldEnum.Vector2Int },

                //{ typeof(Vector3), IMGUIDrawFieldEnum.Vector3 },
                //{ typeof(Vector3Int), IMGUIDrawFieldEnum.Vector3Int },

                //{ typeof(Vector4), IMGUIDrawFieldEnum.Vector4 },
                //{ typeof(Quaternion), IMGUIDrawFieldEnum.Vector4 },

                //{ typeof(Gradient), IMGUIDrawFieldEnum.Gradient },

                //{ typeof(UnityEngine.Object), IMGUIDrawFieldEnum.Object },
            };

        /// <summary>
        /// Creates type of IMGUIField[T].
        /// </summary>
        /// <param name="target">target to tracked</param>
        /// <param name="label">label to draw with IMGUIField</param>
        /// <returns></returns>
        public static IMGUIField CreateIMGUI(HandledMemberTarget target, GUIContent label)
        {
            var type = target.MemberType;
            
            Type imguiType;
            if (type.IsParentInEditorElement())
            {
                imguiType = typeof(IMGUIFold);
            }
            else
            {
                var key = IMGUIPairs.Keys.First(k => k.IsAssignableFrom(type));
                imguiType = IMGUIPairs[key];
            }
                
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
