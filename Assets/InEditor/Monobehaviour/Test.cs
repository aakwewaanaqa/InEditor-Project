using System;
using UnityEngine;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        [InEditor(DisplayOrder = 1)]
        public float Float;
        [InEditor(DisplayOrder = 2)]
        public TestClass Class;

        public class TestClass
        {
            [InEditor(DisplayOrder = 1)]
            public int Int;
        }

        [ContextMenu("Test It")]
        public void TestIt()
        {
        }
    }
}