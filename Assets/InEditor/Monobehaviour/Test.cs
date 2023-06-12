using System;
using System.Collections.Generic;
using UnityEngine;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        [InEditor(DisplayOrder = 1)]
        public float Float;
        [InEditor(DisplayOrder = 2)]
        public TestClass TestedClass;
        [InEditor(DisplayOrder = 3)]
        public List<int> Ints;
        [InEditor(DisplayOrder = 4)]
        public string Str { get; }

        public class TestClass
        {
            [InEditor(DisplayOrder = 1)]
            public int Int;
        }

        [ContextMenu("Test It")]
        public void TestIt()
        {
            Debug.Log(typeof(List<int>).IsArray);
        }
    }
}