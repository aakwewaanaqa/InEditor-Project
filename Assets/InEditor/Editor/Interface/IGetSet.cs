namespace InEditor
{
    /// <summary>
    /// Acts like FieldInfo or PropertyInfo can set and get.
    /// </summary>
    public interface IGetSet<T>
    {
        T GetValue(string path);
        void SetValue(string path, T value);
    }
}