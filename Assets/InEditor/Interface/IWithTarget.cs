namespace InEditor
{
    public interface IWithTarget
    {
        object Target { get; }
        bool IsTargetLost { get; }
    }
}