using System.Collections.Generic;
using UnityEngine;
using System;

namespace InEditor
{
    public partial class InEditorElement
    {
        /// <summary>
        /// Used to store IMGUI info to draw like.
        /// </summary>
        private struct IMGUIInfo
        {
            private static List<IFieldMatch> IMGUIPairs
                => new List<IFieldMatch>()
            {
                new IMGUIToggleField(),

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

            public IMGUIInfo(Type type)
            {
            }
        }
    }
}