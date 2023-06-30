using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class SerializedTest : MonoBehaviour
    {
        [InEditor(0)]
        public bool b;

        [InEditor(1)]
        public int i;

        [InEditor(2)]
        public float f;

        [InEditor(3, false)]
        public string str;

        [InEditor(4)]
        public Rect r;

        [InEditor(5)]
        public RectInt ri;

        [InEditor(6)]
        public Color c;

        [InEditor(7)]
        public Color32 c32;

        [InEditor(8)]
        public Vector2 v2;

        [InEditor(9)]
        public Vector2Int v2I;

        [InEditor(10)]
        public Vector3 v3;

        [InEditor(11)]
        public Vector3Int v3I;

        [InEditor(12)]
        public Vector4 v4;

        [InEditor(13)]
        public LayerMask layerMask;

        [InEditor(14)]
        public Gradient grad;

        [InEditor(15)]
        public AnimationCurve curve;

        [InEditor(16)]
        public GameObject obj;

        [InEditor(17)]
        public ManagedClass managed;

        [Serializable]
        public class ManagedClass
        {
            public bool b;
        }
    }
}