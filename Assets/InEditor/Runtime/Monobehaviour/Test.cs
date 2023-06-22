using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        [InEditor(DisplayOrder = 1)]
        [SerializeField]
        private bool boolean;

        [InEditor(DisplayOrder = 2)] [SerializeField]
        private Tested tested;
        
        [Serializable]
        private class Tested
        {
            [InEditor(DisplayOrder = 1)]
            [SerializeField]
            private bool boolean;
        }
    }
}