using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InEditor
{
    /// <summary>
    ///     It's a super class to deal fields and properties of inspector.
    /// </summary>
    public partial class InEditorElement : IComparable<InEditorElement>
    {
        /// <summary>
        ///     Deals elements hierarchy.
        /// </summary>
        private readonly ElementHierarchy hierarchy;

        /// <summary>
        ///     Deals imgui drawing.
        /// </summary>
        private readonly IMGUIField imgui;

        /// <summary>
        ///     From the MemberInfo.
        /// </summary>
        private readonly InEditorAttribute inEditor;

        /// <summary>
        ///     Used by [InEditorElement.Reflect]...
        /// </summary>
        private InEditorElement(object target, MemberInfo member, InEditorElement parent)
        {
            member.TryGetAttribute(out inEditor);

            imgui = IMGUIField.MakeField(target, member, inEditor);

            hierarchy = member.IsParentInEditorElement()
                ? new ElementHierarchy(parent, Reflect(imgui.PassTarget(), imgui.TargetType, this))
                : new ElementHierarchy(parent, null);
        }

        /// <summary>
        ///     IComparable: Used in Sorting or Ordering in list.
        /// </summary>
        /// <param name="other"> the compared </param>
        /// <returns> sorting clue </returns>
        public int CompareTo(InEditorElement other)
        {
            return inEditor.DisplayOrder.CompareTo(other.inEditor.DisplayOrder);
        }

        /// <summary>
        ///     Creates a type's elements quickly...
        /// </summary>
        /// <param name="target"> instance of data </param>
        /// <param name="type"> desired type </param>
        /// <param name="parent"> pass null if it's the first one </param>
        /// <returns> editor elements </returns>
        public static IEnumerable<InEditorElement> Reflect(object target, Type type, InEditorElement parent)
        {
            // this [GetMembers()] gets every MemberInfo in the [type]
            var infos = type
                .GetMembers(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(i => i.IsInEditorElement());

            var members = new List<InEditorElement>();
            foreach (var info in infos)
                switch (info)
                {
                    // We only want field and property.....
                    // field is like <code> private string str </code>
                    // property is like <code> private string str { get; set; } </code>
                    case FieldInfo:
                    case PropertyInfo:
                        members.Add(new InEditorElement(target, info, parent));
                        break;
                }

            members.Sort();
            return members;
        }

        /// <summary>
        ///     Draws inspectors IMGUI-like
        /// </summary>
        public void OnInspectorGUI()
        {
            imgui.Layout();
            if (imgui.IsExpended && hierarchy.HasRelatives)
                foreach (var relative in hierarchy)
                    relative.imgui.Layout();
        }
    }
}