using UnityEditor;
using UnityEngine;

namespace InEditor.Editor.Class.Field
{
    public class IMGUILabel : IMGUIField
    {
        public override void Layout()
        {
            var value = Target.GetValue();
            var content = new GUIContent(value.ToString());
            if (value is UnityEngine.Object obj)
            {
                var type = !(obj.Equals(null))
                    ? obj.GetType()
                    : typeof(UnityEngine.Object);
                content = EditorGUIUtility.ObjectContent(obj, type);
            }
            EditorGUILayout.LabelField(Label, content);
        }

        public override bool IsExpended { get; set; }
    }
}