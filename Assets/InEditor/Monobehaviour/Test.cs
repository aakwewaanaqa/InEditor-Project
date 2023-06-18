using System;
using System.Collections.Generic;
using UnityEngine;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        [InEditor(DisplayOrder = 3)]
        [NonSerialized]
        private List<int> Ints;

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