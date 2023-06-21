namespace InEditor
{
    /// <summary>
    /// The object representing the target
    /// </summary>
    public interface IByTarget
    {
        object rawTarget { get; }
        object GetTarget();
    }
}