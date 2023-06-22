using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        // [SerializeField]
        // [InEditor(DisplayOrder = 1)]
        // private bool boolean;
        //
        // [SerializeField]
        // [InEditor(DisplayOrder = 2)]
        // private SerializableTested serializableTested;

        [InEditor(DisplayOrder = 3)]
        private NotserializableTested notserializableTested;

        [Serializable]
        private class SerializableTested
        {
            [InEditor(DisplayOrder = 1)] [SerializeField]
            private bool boolean;
        }

        private class NotserializableTested
        {
            [InEditor(DisplayOrder = 1)]
            private bool boolean;
        }
    }
}
