using System.Collections;
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
        private string str;
    }
}