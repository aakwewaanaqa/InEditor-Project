using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace InEditor
{
    public struct ReflectiveInfo : IGetSet
    {
        public readonly MemberInfo MemberInfo;
        public readonly Type FieldOrPropertyType;

        /// <summary>
        /// Determines the type fits the rule to draw as an InEditorMember
        /// </summary>
        public bool CanBeInEditorElement
        {
            get
            {
                return MemberInfo.IsInEditorElement();
            }
        }
        /// <summary>
        /// Determines the type will have members inside.
        /// </summary>
        public bool CanBeInEditorElementParent
        {
            get => MemberInfo.IsParentInEditorElement();
        }
        /// <summary>
        /// Tells the type is a array type
        /// </summary>
        public bool IsIList
        {
            get => typeof(IList).IsAssignableFrom(FieldOrPropertyType);
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
                return;
            }
            else if (info is PropertyInfo prop)
            {
                MemberInfo = prop;
                FieldOrPropertyType = prop.PropertyType;
                return;
            }
            throw new NotImplementedException();
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
            while (target is IByTarget by)
                target = by.Target;

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
            while (target is IByTarget by)
                target = by.Target;

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