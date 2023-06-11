using UnityEngine;
using System;
using System.Reflection;

namespace InEditor
{
    public class ReflectiveInfo : IGetSet
    {
        public readonly MemberInfo MemberInfo;
        public readonly Type FieldOrPropertyType;

        private readonly SerializeField serializeField;

        /// <summary>
        /// Determines whether the member can be serialized through Unity.
        /// This affects the way the Editor's behaviour of dealing value changes.
        /// </summary>
        public bool IsSerialized
        {
            get
            {
                if (IsField)
                {
                    if (Field.IsPublic && !Field.IsStatic && Field.FieldType.IsSerializable)
                        return true;
                    else if (serializeField is object && !Field.IsStatic && Field.FieldType.IsSerializable)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Member's declared name.
        /// </summary>
        public string Name
        {
            get => MemberInfo.Name;
        }
        /// <summary>
        /// If member is a field, this will return it's field as object.
        /// </summary>
        public FieldInfo Field
        {
            get => MemberInfo as FieldInfo;
        }
        /// <summary>
        /// If member is a property, this will return it's property as object.
        /// </summary>
        public PropertyInfo Property
        {
            get => MemberInfo as PropertyInfo;
        }


        /// <summary>
        /// Constructs member into every infos of necesity.
        /// </summary>
        /// <param name="info"></param>
        public ReflectiveInfo(MemberInfo info)
        {
            MemberInfo = info;
            if (info is FieldInfo field)
            {
                MemberInfo = field;
                FieldOrPropertyType = field.FieldType;
            }
            else if (info is PropertyInfo prop)
            {
                MemberInfo = prop;
                FieldOrPropertyType = prop.PropertyType;
            }
            MemberInfo.TryGetAttribute(out serializeField);
        }

        public bool IsField
        {
            get => MemberInfo is FieldInfo;
        }
        public bool IsProperty
        {
            get => MemberInfo is PropertyInfo;
        }
        public bool CanRead
        {
            get => IsField || (IsProperty && Property.CanRead);
        }
        public bool CanWrite
        {
            get => IsField || (IsProperty && Property.CanWrite);
        }
        public bool IsEnum
        {
            get => FieldOrPropertyType.IsEnum;
        }
        public bool IsClass
        {
            get => FieldOrPropertyType.IsClass;
        }
        public bool IsStruct
        {
            get => FieldOrPropertyType.IsValueType && !IsEnum && !FieldOrPropertyType.IsPrimitive;
        }

        public object CreateDefault()
        {
            return Activator.CreateInstance(FieldOrPropertyType);
        }
        public object GetValue(object target)
        {
            if (IsField)
            {
                return Field.GetValue(target);
            }
            else if (IsProperty)
            {
                if (Property.CanRead)
                    return Property.GetValue(target);
                else
                    throw new InvalidOperationException();
            }
            throw new InvalidOperationException();
        }
        public void SetValue(object target, object value)
        {
            if (IsField)
            {
                Field.SetValue(target, value);
            }
            else if (IsProperty)
            {
                if (Property.CanWrite)
                    Property.SetValue(target, value);
                else
                    throw new InvalidOperationException();
            }
        }
    }
}