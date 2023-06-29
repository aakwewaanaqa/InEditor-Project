using UnityEditor;

namespace InEditor.Editor.Class.Field
{
    public class IMGUIPropertyField : IMGUIField
    {
        public override void Layout()
        {
            EditorGUILayout.PropertyField(Target.FindProperty(), Label);
        }

        public override bool IsExpended
        {
            get
            {
                return Target.FindProperty().isExpanded;
            }
            set
            {
                Target.FindProperty().isExpanded = value;
            }
        }
    }
}
