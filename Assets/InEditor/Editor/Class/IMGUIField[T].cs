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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            public SerializedProperty Find(IMGUIFieldInfo path)
            {
                return rawTarget switch
                {
                    SerializedObject serializedObject => serializedObject.FindProperty(path.member.Name),
                    SerializedProperty property => property.FindPropertyRelative(path.member.Name),
                    _ => throw new InvalidOperationException()
                };
            }

            /// <summary>
            /// Sets through SerializedObject / SerializedProperty / System.Object
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <param name="value"> passed value </param>
            /// <param name="isSerializedProperty"> the clue of whether field is getting value via path </param>
            public void SetValue(IMGUIFieldInfo path, T value, bool isSerializedProperty)
            {
                if (isSerializedProperty)
                    Find(path).boxedValue = value;
                else
                    path.SetValue(rawTarget, value);
            }

            /// <summary>
            /// Gets from SerializedObject / SerializedProperty / System.Object
            /// </summary>
            /// <param name="path"> the clue of path of serialized data </param>
            /// <param name="isSerializedProperty"> the clue of whether field is getting value via path </param>
            /// <returns> returned value </returns>
            public T GetValue(IMGUIFieldInfo path, bool isSerializedProperty)
            {
                if (isSerializedProperty)
                    return (T)Find(path).boxedValue;
                else
                    return (T)path.GetValue(rawTarget);
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
                    return Find(path);
                
                //TODO: Sets created instance back to rawTarget please.
                var obj = rawTarget switch
                {
                    SerializedObject serializedObject => serializedObject.targetObject,
                    SerializedProperty serializedProperty => serializedProperty.boxedValue,
                    _ => rawTarget,
                };
                return path.GetValue(obj) ?? Activator.CreateInstance(path.MemberType);
            }
        }

        /// <summary>
        /// Stores reflection information, avoiding asking its MemberType.
        /// </summary>
        protected class IMGUIFieldInfo
        {
            public MemberInfo member;

            public IMGUIFieldInfo(MemberInfo member)
            {
                this.member = member;
            }

            public Type MemberType
            {
                get
                {
                    return member switch
                    {
                        FieldInfo fieldInfo => fieldInfo.FieldType,
                        PropertyInfo propertyInfo => propertyInfo.PropertyType,
                        _ => throw new InvalidOperationException(),
                    };
                }
            }

            public object GetValue(object target)
            {
                return member switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(target),
                    PropertyInfo propertyInfo => propertyInfo.GetValue(target),
                    _ => throw new InvalidOperationException(),
                };
            }

            public void SetValue(object target, object value)
            {
                switch (member)
                {
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(target, value);
                        break;
                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(target, value);
                        break;
                    default:
                        throw new InvalidOperationException();
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
