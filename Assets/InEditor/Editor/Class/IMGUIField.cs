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
        #region Abstract
        /// <summary>
        /// When target changed, to retarget to the System.Object.
        /// <br>
        /// Non-SerializedField will have to use this to deal reference differing...
        /// </br>
        /// </summary>
        /// <param name="target"> passed new target </param>
        public abstract void Retarget(object target);
        /// <summary>
        /// Used in BaseInEditor.OnInspectorGUI()
        /// to draw EditorGUILayout of this IMGUIField,
        /// we must implemented this method or their won't be any visual element.
        /// </summary>
        public abstract void Layout();
        /// <summary>
        /// Used in InEditorElement.Ctor.Reflect()
        /// to get relatives' target, we must get from the parent's target.
        /// </summary>
        /// <returns></returns>
        public abstract object PassTarget();
        #endregion

        #region Instance Fields
        /// <summary>
        /// The type that this IMGUIField is tracking and dealing.
        /// </summary>
        public Type TargetType;
        /// <summary>
        /// The drawn label to display in IMGUI field.
        /// </summary>
        public GUIContent Label;
        #endregion

        #region public static MakeField()
        /// <summary>
        /// Gets the type of the MemberInfo
        /// </summary>
        /// <param name="member"> passed MemberInfo </param>
        /// <returns> Matching type of IMGUIField </returns>
        private static Type GetType(MemberInfo member)
        {
            return member switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new NotImplementedException()
            };
        }
        /// <summary>
        /// Creates type of IMGUIField[T], T is the passing type.
        /// </summary>
        /// <param name="type">target type</param>
        /// <param name="target">IMGUIField's target</param>
        /// <param name="member">passed MemberInfo</param>
        /// <param name="inEditor">passed InEditorAttribute</param>
        /// <returns></returns>
        private static IMGUIField CreateIMGUI(Type type, object target, MemberInfo member, InEditorAttribute inEditor)
        {
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
                
            var imgui = (IMGUIField)Activator.CreateInstance(imguiType, new object[2] { target, member });
            imgui.TargetType = type;
            
            var name = inEditor.DisplayName;
            var content = string.IsNullOrEmpty(name) ? member.Name : name;
            content = inEditor.NicifyName ? ObjectNames.NicifyVariableName(content) : content;
            imgui.Label = new GUIContent(content);
            
            return imgui;
        }
        /// <summary>
        /// Creates IMGUIField[T] matching MemberInfo's type and tracing target's value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="member"></param>
        /// <param name="inEditor"></param>
        /// <returns></returns>
        public static IMGUIField MakeField(object target, MemberInfo member, InEditorAttribute inEditor)
        {
            var type = GetType(member);
            var imgui = CreateIMGUI(type, target, member, inEditor);
            return imgui;
        }
        
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
        #endregion
    }
}