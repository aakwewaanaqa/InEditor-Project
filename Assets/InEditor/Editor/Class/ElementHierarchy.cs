using System.Collections.Generic;

namespace InEditor
{
    public partial class InEditorElement
    {
        /// <summary>
        /// Used to store parent and relatives into struct.
        /// </summary>
        private class ElementHierarchy 
        {
            public InEditorElement Parent 
            { 
                get => parent; 
            }
            public IEnumerable<InEditorElement> Relatives 
            { 
                get => relatives; 
            }

            private readonly InEditorElement parent;
            private readonly IEnumerable<InEditorElement> relatives;

            public ElementHierarchy(InEditorElement parent, IEnumerable<InEditorElement> relatives)
            {
                this.parent = parent;
                this.relatives = relatives;
            }
        }
    }
}