using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace InEditor
{
    public class InEditorElement : IComparable<InEditorElement>
    {
        private struct ReflectiveTarget
        {
            public object RawTarget
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

            public void Retarget(object target)
            {
                rawTarget = target;
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
        private readonly ReflectiveTarget target;
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
        /// Is it serialized as SerializedProperty?
        /// </summary>
        public bool IsSerialized
        {
            get => reflect.IsSerialized;
        }
        /// <summary>
        /// Creates a member to deal Editor drawing.
        /// </summary>
        /// <param name="object"> the target which holds value </param>
        /// <param name="memberInfo"> to bind value </param>
        /// <param name="index"> to sort display </param>
        private InEditorElement(object @object, MemberInfo memberInfo, int index, InEditorElement parent)
        {
            this.target = new ReflectiveTarget(@object);
            this.reflect = new ReflectiveInfo(memberInfo);
            this.index = index;

            memberInfo.TryGetAttribute(out inEditor);

            if (reflect.IsClass || reflect.IsStruct)
            {
                if (target.IsNull) // This is really dangerous to create default value
                {
                    target.Retarget(Activator.CreateInstance(reflect.FieldOrPropertyType));
                    reflect.SetValue(@object, target.RawTarget);
                }
                hierarchy = new ElementHierarchy(parent, Reflect(target.RawTarget, reflect.FieldOrPropertyType));
            }
            else
            {

            }
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
        public static IEnumerable<InEditorElement> Reflect(object target, Type type, InEditorElement parent = null)
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
                    bindable.bindingPath = reflect.Name;
            }

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
        /// Gives you a binded visual element.....
        /// <br>
        /// It was designed not to create VisualElement in ctor by first place,
        /// 'cause we want to choose from drawing Editor-IMGUI or VisualElement.
        /// </br>
        /// </summary>
        /// <returns> the binded element </returns>
        public VisualElement CreateElement()
        {
            VisualElement element = default;
            if (reflect.FieldOrPropertyType == typeof(string))
            {
                element = new TextField(DisplayName);
            }
            else if (reflect.FieldOrPropertyType == typeof(int))
            {
                element = new IntegerField(DisplayName);
            }
            else if (reflect.FieldOrPropertyType == typeof(float))
            {
                element = new FloatField(DisplayName);
            }
            else if (reflect.FieldOrPropertyType == typeof(Vector3))
            {
                element = new Vector3Field(DisplayName);
            }
            else if (reflect.IsClass || reflect.IsStruct)
            {
                element = new VisualElement();
                foreach (var relative in hierarchy.Relatives)
                    element.Add(relative.CreateElement());
            }

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