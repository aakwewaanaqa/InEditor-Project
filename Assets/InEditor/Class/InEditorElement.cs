using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEditor.UIElements;

namespace InEditor
{
    /// <summary>
    /// It's a super class to deal fields and properties of inspector.
    /// </summary>
    public partial class InEditorElement : IComparable<InEditorElement>
    {
        /// <summary>
        /// Used to store type informations.
        /// </summary>
        private struct ReflectiveTarget : IByTarget
        {
            public object Target
            {
                get => rawTarget;
            }
            public bool IsNull
            {
                get => rawTarget is null || rawTarget.Equals(null);
            }

            private object rawTarget;

            public ReflectiveTarget(object rawTarget)
            {
                this.rawTarget = rawTarget;
            }
        }
        /// <summary>
        /// Used to store parent and relatives into struct.
        /// </summary>
        private struct ElementHierarchy 
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

        /// <summary>
        /// From the MemberInfo.
        /// </summary>
        private readonly InEditorAttribute inEditor;
        /// <summary>
        /// Deals reflection infos.
        /// </summary>
        private readonly ReflectiveInfo reflect;
        /// <summary>
        /// Deals gets and sets.
        /// </summary>
        private ReflectiveTarget target;
        /// <summary>
        /// Deals elements hierarchy.
        /// </summary>
        private readonly ElementHierarchy hierarchy;
        /// <summary>
        /// Deals imgui drawing.
        /// </summary>
        private readonly IMGUIInfo imgui;

        /// <summary>
        /// Editor used this to sort.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// Attribute that holds the desired way to draw this.
        /// </summary>
        public InEditorAttribute InEditor
        {
            get => inEditor;
        }
        /// <summary>
        /// Nicified display name
        /// </summary>
        public string NicifiedName
        {
            get
            {
                if (inEditor is object)
                {
                    var displayName = inEditor.IsRenamed ? inEditor.DisplayName : reflect.Name;
                    return inEditor.NicifyName ? ObjectNames.NicifyVariableName(displayName) : displayName;
                }
                else
                {
                    return ObjectNames.NicifyVariableName(reflect.Name);
                }
            }
        }
        /// <summary>
        /// Calculated path to elements hierarchy to bind property changes.
        /// </summary>
        public string Path
        {
            get
            {
                StringBuilder build = new StringBuilder();
                build.Append(reflect.Name);
                if (hierarchy.Parent is object)
                    build.Insert(0, $"{hierarchy.Parent.Path}.");
                return build.ToString();
            }
        }


        /// <summary>
        /// Used by [InEditorElement.Reflect]...
        /// </summary>
        private InEditorElement(object @object, MemberInfo memberInfo, int index, InEditorElement parent)
        {
            memberInfo.TryGetAttribute(out inEditor);

            this.target = new ReflectiveTarget(@object);
            this.reflect = new ReflectiveInfo(memberInfo);
            this.imgui = new IMGUIInfo(reflect.FieldOrPropertyType);
            this.index = index;

            if (reflect.CanBeInEditorElementParent)
                this.hierarchy = new ElementHierarchy(parent, Reflect(reflect.GetValue(target), reflect.FieldOrPropertyType, this));
            else
                this.hierarchy = new ElementHierarchy(parent, null);
        }

        /// <summary>
        /// Creates a type's members quickly...
        /// <br>
        /// We want the target to be stored first before it goes into Editor drawing.
        /// </br>
        /// </summary>
        /// <param name="target"> instance of data </param>
        /// <param name="type"> desired type </param>
        /// <returns></returns>
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
            {
                switch (info)
                {
                    // we only want field and property.....
                    // field is like <code> private string str </code>
                    // property is like <code> private string str { get; set; } </code>
                    case FieldInfo:
                    case PropertyInfo:
                        members.Add(new InEditorElement(target, info, members.Count, parent));
                        break;
                    default:
                        break;
                }
            }
            members.Sort();
            return members;
        }

        /// <summary>
        /// Draws inpectors IMGUI-like
        /// </summary>
        /// <param name="prop"> pass the property to deal with prefab system </param>
        public void OnInspectorGUI(SerializedProperty prop)
        {
        }

        /// <summary>
        /// ICompareable: Used in Sorting or Ordering in list.
        /// </summary>
        /// <param name="other"> the compared </param>
        /// <returns> sorting clue </returns>
        public int CompareTo(InEditorElement other)
        {
            int a = inEditor is object ? inEditor.DisplayOrder : index;
            int b = other.inEditor is object ? other.inEditor.DisplayOrder : other.index;
            return a.CompareTo(b);
        }
    }
}