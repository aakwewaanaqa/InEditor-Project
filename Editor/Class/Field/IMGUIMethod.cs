using System;
using System.Reflection;

namespace InEditor.Editor.Class.Field
{
    public class IMGUIMethod : IMGUIField
    {
        private readonly MethodInfo method;
        private readonly object realTarget;
        
        public override void Layout()
        {
            method.Invoke(realTarget, Array.Empty<object>());
        }

        public IMGUIMethod(MethodInfo method, object realTarget)
        {
            this.method = method;
            this.realTarget = realTarget;
        }
        
        public override bool IsExpended { get; set; }
    }
}
