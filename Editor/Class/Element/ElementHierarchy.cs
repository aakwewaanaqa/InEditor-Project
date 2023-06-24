using System.Collections;
using System.Collections.Generic;

namespace InEditor.Editor.Class.Element
{
    public partial class InEditorElement
    {
        /// <summary>
        /// Used to store parent and relatives into struct.
        /// </summary>
        private class ElementHierarchy : IEnumerable<InEditorElement>
        {
            private readonly InEditorElement parent;
            private readonly IEnumerable<InEditorElement> relatives;

            public ElementHierarchy(InEditorElement parent, IEnumerable<InEditorElement> relatives)
            {
                this.parent = parent;
                this.relatives = relatives;
            }

            public bool HasRelatives
            {
                get { return relatives is not null; }
            }

            public IEnumerator<InEditorElement> GetEnumerator()
            {
                return relatives.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)relatives).GetEnumerator();
            }
        }
    }
}