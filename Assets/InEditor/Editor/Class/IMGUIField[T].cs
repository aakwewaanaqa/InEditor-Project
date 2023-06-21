using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InEditor
{
    public abstract class IMGUIField<T> : IMGUIField
    {
        /// <summary>
        /// TODO: Gets and sets value whether target is SerializedObject, SerializedProperty or raw object...
        /// </summary>
        protected class FieldTarget
        {
            public object rawTarget;

            public bool IsSerialized
            {
                get => rawTarget is SerializedObject || rawTarget is SerializedProperty;
            }

            public FieldTarget(object target)
            {
                rawTarget = target;
            }

            public SerializedProperty Find(IMGUIFieldInfo path)
            {
                if (rawTarget is SerializedObject serializedObject)
                {
                    return serializedObject.FindProperty(path.member.Name);
                }
                else if (rawTarget is SerializedProperty property)
                {
                    return property.FindPropertyRelative(path.member.Name);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Sets through SerializedObject / SerializedProperty / System.Object
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <param name="value"> passed value </param>
            public void SetValue(IMGUIFieldInfo path, T value)
            {
                if (IsSerialized)
                {
                    Find(path).boxedValue = value;
                }
                else
                {
                    if (path.IsField)
                        path.Field.SetValue(rawTarget, value);
                    else if (path.IsProperty)
                        path.Property.SetValue(rawTarget, value);
                }
            }
            /// <summary>
            /// Gets from SerializedObject / SerializedProperty / System.Object
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <returns> returned value </returns>
            public T GetValue(IMGUIFieldInfo path)
            {
                if (IsSerialized)
                {
                    return (T)Find(path).boxedValue;
                }
                else
                {
                    if (path.IsField)
                        return (T)path.Field.GetValue(rawTarget);
                    else if (path.IsProperty)
                        return (T)path.Property.GetValue(rawTarget);
                    else
                        throw new InvalidOperationException();
                }
            }
        }
        /// <summary>
        /// Stores informations.
        /// </summary>
        protected class IMGUIFieldInfo
        {
            public MemberInfo member;

            public IMGUIFieldInfo(MemberInfo member)
            {
                this.member = member;
            }
            public bool IsField
            {
                get => member is FieldInfo;
            }
            public bool IsProperty
            {
                get => member is PropertyInfo;
            }
            public FieldInfo Field
            {
                get 
                {
                    return member as FieldInfo;
                }
            }
            public PropertyInfo Property
            {
                get
                {
                    return member as PropertyInfo;
                }
            }
            public Type MemberType
            {
                get
                {
                    return member.MemberType switch
                    {
                        MemberTypes.Field => Field.FieldType,
                        MemberTypes.Property => Property.PropertyType,
                        _ => throw new InvalidOperationException(),
                    };
                }
            }
        }

        private readonly FieldTarget target;
        private readonly IMGUIFieldInfo member;

        /// <summary>
        /// The drawn label to display in IMGUI field.
        /// </summary>
        public GUIContent Label = new GUIContent(string.Empty);

        public override void Layout()
        {
            SetValue(Layout(GetValue()));
        }
        
        protected void SetValue(T value)
        {
            target.SetValue(member, value);
        }
        protected T GetValue()
        {
            return target.GetValue(member);
        }

        public override Type FieldType => typeof(T);
        public override object RawValue => GetValue();
        public override void Retarget(object target)
        {
            this.target.rawTarget = target;
        }

        /// <summary>
        /// To implement, override it with GUILayout of the field
        /// </summary>
        /// <param name="input"> value input </param>
        /// <returns> value output </returns>
        protected abstract T Layout(T input);

        public IMGUIField(object target, MemberInfo member) : base()
        {
            this.target = new FieldTarget(target);
            this.member = new IMGUIFieldInfo(member);
        }
    }
}