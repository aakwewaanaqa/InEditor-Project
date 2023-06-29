using System;
using InEditor.Runtime.Interface;
using JetBrains.Annotations;
using UnityEditor;

namespace InEditor
{
    public class InEditorRepaintAttribute : Attribute, IOnInspectorGUIEditor
    {
        /// <summary>
        /// Marks a class that repaint constantly when EditorApplication.isPlaying
        /// </summary>
        public bool PlayingRepaint;

#if UNITY_EDITOR
        public void OnInspectorGUI(object editor)
        {
            if (PlayingRepaint && EditorApplication.isPlaying)
                (editor as Editor)?.Repaint();
        }
#endif
    }
}
