using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InEditor
{
    /// <summary>
    /// IMGUI Drawing base class of type T.
    /// </summary>
    /// <typeparam name="T">the field's dealing type T</typeparam>
    public abstract class IMGUIField<T> : IMGUIField
    {
        /// <summary>
        /// Gets and sets value whether target is SerializedObject, SerializedProperty or raw object...
        /// </summary>
        protected class FieldTarget
        {
            public object rawTarget;

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
            public void SetValue(IMGUIFieldInfo path, T value, bool isSerializedProperty)
            {
                if (isSerializedProperty)
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
            public T GetValue(IMGUIFieldInfo path, bool isSerializedProperty)
            {
                if (isSerializedProperty)
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

            /// <summary>
            /// Gets target for relatives from SerializedObject / SerializedProperty / System.Object
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <param name="isPropertyPath"> relatives control this to get property or not </param>
            /// <returns> returned target for relatives </returns>
            /// <exception cref="InvalidOperationException"> not a field or property </exception>
            public object PassTarget(IMGUIFieldInfo path, bool isPropertyPath)
            {
                if (isPropertyPath)
                {
                    return Find(path);
                }
                else
                {
                    var obj = rawTarget switch
                    {
                        SerializedObject serializedObject => serializedObject.targetObject,
                        SerializedProperty serializedProperty => serializedProperty.boxedValue,
                        _ => rawTarget,
                    };

                    object value = null;
                    if (path.IsField)
                    {
                        value = path.Field.GetValue(obj);
                    }
                    else if (path.IsProperty)
                    {
                        value = path.Property.GetValue(obj);
                    }

                    if (value is null)
                    {
                        value = Activator.CreateInstance(path.MemberType);
                        return value;
                    }

                    return value;
                }
            }

            /// <summary>
            /// Creates default value of the 
            /// </summary>
            /// <returns></returns>
            public object CreateDefault(IMGUIFieldInfo path)
            {
                return Activator.CreateInstance(path.MemberType);
            }
        }

        /// <summary>
        /// Stores reflection information.
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
                get => member as FieldInfo;
            }

            public PropertyInfo Property
            {
                get => member as PropertyInfo;
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

        /// <summary>
        /// Gets and sets value whether target is SerializedObject, SerializedProperty or raw object...
        /// </summary>
        protected readonly FieldTarget target;

        /// <summary>
        /// Stores reflection information.
        /// </summary>
        protected readonly IMGUIFieldInfo member;

        /// <summary>
        /// Used in Editor.OnInspectorGUI()
        /// to draw EditorGUILayout
        /// </summary>
        public override void Layout()
        {
            using var scope = new EditorGUI.ChangeCheckScope();

            var value = Layout(GetValue());
            if (scope.changed)
                SetValue(value);
        }

        /// <summary>
        /// Used in Layout()
        /// to set value with IMGUIFieldInfo via FieldTarget.
        /// </summary>
        /// <param name="value">passed value</param>
        protected virtual void SetValue(T value)
        {
            target.SetValue(member, value, IsSerializedProperty);
        }

        /// <summary>
        /// Used in Layout()
        /// to get value with IMGUIFieldInfo via FieldTarget.
        /// </summary>
        protected virtual T GetValue()
        {
            return target.GetValue(member, IsSerializedProperty);
        }

        /// <summary>
        /// Used in InEditorElement.ctor.Reflect()
        /// to pass down target for relatives
        /// </summary>
        /// <returns> the target for relatives </returns>
        public override object PassTarget()
        {
            return target.PassTarget(member, IsSerializedProperty);
        }

        public override void Retarget(object target)
        {
            this.target.rawTarget = target;
        }

        public override bool IsExpended
        {
            get => false;
        }

        /// <summary>
        /// To implement, override it with EditorGUILayout of the field
        /// </summary>
        /// <param name="input"> value input </param>
        /// <returns> value output </returns>
        protected abstract T Layout(T input);

        protected IMGUIField(object target, MemberInfo member) : base()
        {
            this.target = new FieldTarget(target);
            this.member = new IMGUIFieldInfo(member);
        }
    }
}
