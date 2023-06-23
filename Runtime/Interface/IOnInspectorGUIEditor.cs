using UnityEditor;

namespace InEditor.Runtime.Interface
{
    /// <summary>
    /// To implemented an attribute of a class that when its Editor can pass itself to it for Repaint();
    /// </summary>
    public interface IOnInspectorGUIEditor
    {
        /// <summary>
        /// To implement the OnInspectorGUI function for its Editor
        /// </summary>
        /// <param name="editor">passed Editor</param>
        void OnInspectorGUI(Editor editor);
    }
}