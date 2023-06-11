namespace InEditor
{
    /// <summary>
    /// Acts like FieldInfo or PropertyInfo can set and get.
    /// </summary>
    public interface IGetSet
    {
        object GetValue(object target);
        void SetValue(object target, object value);
    }
}