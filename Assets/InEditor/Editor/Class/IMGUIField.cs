using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InEditor
{
    public abstract class IMGUIField
    {
        public static readonly Dictionary<Type, Type> IMGUIPairs
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
        /// Fields target type.
        /// </summary>
        public abstract Type FieldType { get; }
        /// <summary>
        /// Gets the raw value of this field.
        /// </summary>
        public abstract object RawValue { get; }
        /// <summary>
        /// When target changed, to retarget to the System.Object.
        /// <br>
        /// Non-SerializedField will have to use this to deal reference differing...
        /// </br>
        /// </summary>
        /// <param name="target"> passed new target </param>
        public abstract void Retarget(object target);
        /// <summary>
        /// Draws field with EditorGUILayout.
        /// </summary>
        public abstract void Layout();

        public static IMGUIField MakeField(object target, MemberInfo member)
        {
            Type type = null;
            if (member is FieldInfo field)
                type = field.FieldType;
            else if (member is PropertyInfo property)
                type = property.PropertyType;
            else
                throw new NotImplementedException();

            Type imguiType = null;
            var key = IMGUIPairs.Keys.First(k => k.IsAssignableFrom(type));
            imguiType = IMGUIPairs[key];

            var imgui = Activator.CreateInstance(imguiType, new object[2] { target, member });
            return (IMGUIField)imgui;
        }
    }
}