using System;
using UnityEngine;

namespace InEditor
{
    public sealed class IMGUIToggleField : IMGUIField<bool>
    {
        public override bool Layout(bool value)
        {
            return GUILayout.Toggle(value, Label);
        }
    }
}