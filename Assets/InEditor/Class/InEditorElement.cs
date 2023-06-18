using System.Collections.Generic;
using UnityEngine;
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
        /// Used to bind value changed...
        /// <br>
        /// Designed to be reflectivily invoked...
        /// </br>
        /// </summary>
        /// <typeparam name="T"> element's value type </typeparam>
        /// <param name="element"> the drawing element </param>
        private void RegisterChangeEvent<T>(VisualElement element, bool serialized)
        {
            if (element is BindableElement bindable)
            {
                bindable.bindingPath = Path;
            }
            // Any member that can or cannot be serialized must rely on their declaring type.
            // It was not fixed yet.....
            if (!serialized)
            {
                // Dealing value grabbing
                if (element is BaseField<T> b && reflect.CanRead)
                {
                    b.value = (T)reflect.GetValue(target);
                    EventCallback<MouseMoveEvent> d = (e) =>
                    {
                        Debug.Log("Recieve");
                        b.value = (T)reflect.GetValue(target);
                    };
                    element.RegisterCallback(d);
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
        /// Gives you a binded visual element.....
        /// <br>
        /// It was designed not to create VisualElement in ctor by first place,
        /// 'cause we want to choose from drawing Editor-IMGUI or VisualElement.
        /// </br>
        /// </summary>
        /// <returns> the binded element </returns>
        public VisualElement CreatePropertyGUI(SerializedProperty prop)
        {
            if (target.IsNull)
                target = new ReflectiveTarget(hierarchy.Parent.reflect.CreateDefault());

            VisualElement element = default;
            if (reflect.CanBeInEditorElementParent)
            {
                element = new Foldout() { text = NicifiedName };
                foreach (var relative in hierarchy.Relatives)
                {
                    prop = prop?.FindPropertyRelative(reflect.Name);
                    element.Add(relative.CreatePropertyGUI(prop));
                }
            }
            else
            {
                if (reflect.IsIList)
                {
                    element = new ListView((IList)reflect.GetValue(target))
                    {
                        showBorder = true,
                        showFoldoutHeader = true,
                        headerTitle = NicifiedName,
                        showAddRemoveFooter = true,
                        reorderMode = ListViewReorderMode.Animated,
                    };
                }
                else
                {
                    element = reflect.FieldOrPropertyType.ToVisualElementField();
                }
            }

            TrySetProperty(element, "label", NicifiedName);
            element.SetEnabled(reflect.CanWrite);

            // magic goes here... we want to invoke [RegisterChangeEvent<T>()] but passing [reflect.MemberType] to be the [<T>]
            // so by using [MakeGenericMethod()] we succeed
            typeof(InEditorElement)
                .GetMethod(nameof(RegisterChangeEvent), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(reflect.FieldOrPropertyType)
                .Invoke(this, new object[2] { element, prop is object });

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