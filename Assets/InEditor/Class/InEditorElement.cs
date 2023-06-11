using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System.Text;
using System.Collections;

namespace InEditor
{
    public class InEditorElement : IComparable<InEditorElement>
    {
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
        /// Attribute that holds the desired way to draw this.
        /// </summary>
        private readonly InEditorAttribute inEditor;
        /// <summary>
        /// FieldInfo or PropertyInfo? It tells.
        /// </summary>
        private readonly ReflectiveInfo reflect;
        /// <summary>
        /// Instance holding this InEditorMember member.
        /// </summary>
        private ReflectiveTarget target;
        /// <summary>
        /// Tells the elemets construction.
        /// </summary>
        private readonly ElementHierarchy hierarchy;

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
        public string DisplayName
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
        /// Creates a member to deal Editor drawing.
        /// </summary>
        /// <param name="object"> the target which holds value </param>
        /// <param name="memberInfo"> to bind value </param>
        /// <param name="index"> to sort display </param>
        private InEditorElement(object @object, MemberInfo memberInfo, int index, InEditorElement parent)
        {
            memberInfo.TryGetAttribute(out inEditor);

            this.target = new ReflectiveTarget(@object);
            this.reflect = new ReflectiveInfo(memberInfo);
            this.index = index;

            if (reflect.CanBeInEditorElementParent)
                hierarchy = new ElementHierarchy(parent, Reflect(reflect.GetValue(target), reflect.FieldOrPropertyType, this));
            else
                hierarchy = new ElementHierarchy(parent, null);
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
                .Where(i => i.CanBeInEditorElement());
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
        /// Used to bind value changed...
        /// <br>
        /// Designed to be reflectivily invoked...
        /// </br>
        /// </summary>
        /// <typeparam name="T"> element's value type </typeparam>
        /// <param name="element"> the drawing element </param>
        private void RegisterChangeEvent<T>(VisualElement element)
        {
            {
                if (element is BindableElement bindable)
                {
                    StringBuilder path = new StringBuilder();
                    InEditorElement parent = hierarchy.Parent;
                    while (parent is object)
                    {
                        path.Insert(0, $"{parent.reflect.Name}.");
                        parent = parent.hierarchy.Parent;
                    }
                    path.Append(reflect.Name);
                    bindable.bindingPath = path.ToString();
                }
            }

            if (!reflect.CanBeSerializedInUnity)
            {
                // Dealing value grabbing
                if (element is BaseField<T> b && reflect.CanRead)
                {
                    b.value = (T)reflect.GetValue(target);
                }
                // Dealing value setting
                if (reflect.CanWrite)
                {
                    EventCallback<ChangeEvent<T>> d = (change) =>
                    {
                        reflect.SetValue(target, change.newValue);
                    };
                    element.RegisterCallback(d);
                }
            }
        }
        /// <summary>
        /// Used to assign property if exists, easy for design.....
        /// </summary>
        /// <param name="element"> designing element </param>
        /// <param name="name"> wanted property name </param>
        private bool TrySetProperty(VisualElement element, string name, object value)
        {
            var property = element.GetType().GetProperty(name);
            if (property is object)
                property.SetValue(element, value);
            return property is object;
        }
        /// <summary>
        /// Gives you a binded visual element.....
        /// <br>
        /// It was designed not to create VisualElement in ctor by first place,
        /// 'cause we want to choose from drawing Editor-IMGUI or VisualElement.
        /// </br>
        /// </summary>
        /// <returns> the binded element </returns>
        public VisualElement CreateElement()
        {
            if (target.IsNull)
                target = new ReflectiveTarget(hierarchy.Parent.reflect.CreateDefault());

            VisualElement element = default;

            if (reflect.FieldOrPropertyType == typeof(string))
            {
                element = new TextField();
            }
            else if (reflect.FieldOrPropertyType == typeof(int))
            {
                element = new IntegerField();
            }
            else if (reflect.FieldOrPropertyType == typeof(float))
            {
                element = new FloatField();
            }
            else if (reflect.FieldOrPropertyType == typeof(Vector3))
            {
                element = new Vector3Field();
            }
            else if (reflect.IsIList)
            {
                element = new ListView((IList)reflect.GetValue(target))
                {
                    showBorder = true,
                    showFoldoutHeader = true,
                    headerTitle = DisplayName,
                    showAddRemoveFooter = true,
                    reorderMode = ListViewReorderMode.Animated, 
                };
            }
            else if (reflect.CanBeInEditorElementParent)
            {
                element = new Foldout() { text = DisplayName };
                foreach (var relative in hierarchy.Relatives)
                    element.Add(relative.CreateElement());
            }

            TrySetProperty(element, "label", DisplayName);
            element.SetEnabled(reflect.CanWrite);

            // magic goes here... we want to invoke [RegisterChangeEvent<T>()] but passing [reflect.MemberType] to be the [<T>]
            // so by using [MakeGenericMethod()] we succeed
            typeof(InEditorElement)
                .GetMethod(nameof(RegisterChangeEvent), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(reflect.FieldOrPropertyType)
                .Invoke(this, new object[1] { element });

            return element;
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