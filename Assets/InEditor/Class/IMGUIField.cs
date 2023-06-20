using System;
using UnityEngine;

namespace InEditor
{
    public interface IFieldMatch
    {
        bool Match(Type type);
    }
    public abstract class IMGUIField<T> : IFieldMatch
    {
        public GUIContent Label = new GUIContent(string.Empty);
        public virtual bool Match(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
        public abstract T Layout(T value);
    }
}