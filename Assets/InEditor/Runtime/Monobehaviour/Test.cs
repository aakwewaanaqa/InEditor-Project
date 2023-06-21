using System;
using System.Collections.Generic;
using UnityEngine;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        [InEditor(DisplayOrder = 1)]
        [SerializeField]
        private bool Boolean;
    }
}