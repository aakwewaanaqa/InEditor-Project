﻿using System;
using System.Collections.Generic;
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

        public abstract Type FieldType { get; }
        public abstract object RawValue { get; }
        public abstract void Retarget(object target);

        public static IMGUIField MakeField(object target, MemberInfo member)
        {
            throw new NotImplementedException();
        }
    }
}