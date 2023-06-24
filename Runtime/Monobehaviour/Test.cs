using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace InEditor
{
    [AddComponentMenu("InEditor/Test")]
    public class Test : MonoBehaviour
    {
        [SerializeField]
        [InEditor(DisplayOrder = 1)]
        private bool boolean;

        [SerializeField]
        [InEditor(DisplayOrder = 2)]
        private float floating;
        
        [SerializeField]
        [InEditor(DisplayOrder = 3)]
        private new GameObject gameObject;
        
        [InEditor(DisplayOrder = 4)]
        private static string text;
        
        [SerializeField]
        [InEditor(DisplayOrder = 100)]
        private SerializableTested serializableTested;

        [InEditor(DisplayOrder = 101)]
        private NotserializableTested notserializableTested;

        [SerializeField]
        [InEditor(DisplayOrder = 102)]
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
