using System.Runtime.Serialization;
using UnityEngine;

namespace InEditor
{
    public class NotSerializedTest : MonoBehaviour
    {
        [InEditor(0)]
        public static bool b;
        
        [InEditor(1)]
        public static int i;
        
        [InEditor(2)]
        public static float f;
        
        [InEditor(3)]
        public static string str;
        
        [InEditor(4)]
        public static Rect r;
        
        [InEditor(5)]
        public static RectInt ri;
        
        [InEditor(6)]
        public static Color c;
        
        [InEditor(7)]
        public static Color32 c32;
        
        [InEditor(8)]
        public static Vector2 v2;
        
        [InEditor(9, false)]
        public static Vector2Int v2I;
        
        [InEditor(10)]
        public static Vector3 v3;
        
        [InEditor(11)]
        public static Vector3Int v3I;
        
        [InEditor(12)]
        public static Vector4 v4;
        
        [InEditor(13)]
        public static LayerMask layerMask;
        
        [InEditor(14)]
        public static Gradient grad;
        
        [InEditor(15)]
        public static AnimationCurve curve;
        
        [InEditor(16)]
        public static GameObject obj;

        [InEditor(17)]
        public static ManagedClass managed;
        
        public class ManagedClass
        {
            [InEditor(0)]
            public bool b;
        }
    }
}
