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

        [SerializeField]
        [InEditor(DisplayOrder = 2)]
        private SerializableTested serializableTested;

        [InEditor(DisplayOrder = 3)]
        private NotserializableTested notserializableTested;

        [SerializeField]
        [InEditor(DisplayOrder = 4)]
        private A a;

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
        
        [Serializable]
        private class A
        {
            [Serializable]
            private class B
            {
                [Serializable]
                private class C
                {
                    [SerializeField]
                    [InEditor(DisplayOrder = 1)]
                    private bool Boolean;
                }
                
                [SerializeField]
                [InEditor(DisplayOrder = 1)]
                private C c;
            }
            
            [SerializeField]
            [InEditor(DisplayOrder = 1)]
            private B b;
        }
    }
}
