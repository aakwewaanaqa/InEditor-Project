using System;
using System.Reflection;
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

            public FieldTarget(object target)
            {
                rawTarget = target;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <param name="value"></param>
            public void SetValue(IMGUIFieldInfo path, T value)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <returns></returns>
            public T GetValue(IMGUIFieldInfo path)
            {
                throw new NotImplementedException();
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

            public FieldInfo Field
            {
                get 
                {
                    if (member is FieldInfo field)
                        return field;
                    return null;
                }
            }
            public PropertyInfo Property
            {
                get
                {
                    if (member is PropertyInfo property)
                        return property;
                    return null;
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

        public virtual void Layout()
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