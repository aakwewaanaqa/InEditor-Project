using System;
using System.Reflection;
using UnityEngine;

namespace InEditor
{
    public abstract class IMGUIField<T> : IMGUIField
    {
        /// <summary>
        /// TODO: Set value whether target is SerializedObject, SerializedProperty or raw object...
        /// </summary>
        protected class FieldTarget
        {
            public object rawTarget;

            public FieldTarget(object target)
            {
                rawTarget = target;
            }

            public void SetValue(string path, T value)
            {
                throw new NotImplementedException();
            }
            public T GetValue(string path)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Sets value through target.
        /// </summary>
        protected class FieldMember
        {
            public MemberInfo member;

            public FieldMember(MemberInfo member)
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

            public T GetValue(FieldTarget getSet)
            {
                return getSet.GetValue(member.Name);
            }
            public void SetValue(FieldTarget getSet, T value)
            {
                getSet.SetValue(member.Name, value);
            }
        }

        private readonly FieldTarget target;
        private readonly FieldMember member;

        public GUIContent Label = new GUIContent(string.Empty);

        public virtual void Layout()
        {
            SetValue(Layout(GetValue()));
        }
        
        protected void SetValue(T value)
        {
            member.SetValue(target, value);
        }
        protected T GetValue()
        {
            return member.GetValue(target);
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

        public IMGUIField(object target, MemberInfo member)
        {
            this.target = new FieldTarget(target);
            this.member = new FieldMember(member);
        }
    }
}