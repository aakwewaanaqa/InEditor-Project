using System;
using System.Linq;

namespace InEditor.Editor.Class.Attribute
{
    /// <summary>
    /// Attribute to add to field class for finding the right target type with right IMGUIField type.
    /// </summary>
    public class IMGUIFieldAttribute : System.Attribute
    {
        /// <summary>
        /// Stores target types.
        /// </summary>
        public readonly Type[] Targets;
        /// <summary>
        /// Stores finding type that inherited type in Targets.
        /// </summary>
        public bool IsAlsoInherited;
        /// <summary>
        /// Attribute to add to IMGUIField[T] class.
        /// </summary>
        /// <param name="targets">the target editing types for this field</param>
        public IMGUIFieldAttribute(params Type[] targets)
        {
            Targets = targets;
        }
        /// <summary>
        /// Tests attribute has type.
        /// </summary>
        /// <param name="type">tested type</param>
        /// <returns>result</returns>
        public bool Match(Type type)
        {
            if (!IsAlsoInherited)
                return Targets.Contains(type);
            else 
                return Targets.Any(t => t.IsAssignableFrom(type));
        }
    }
}
