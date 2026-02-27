namespace TomHuizing.Pathfinding;

public interface IEdge
{
    INode From { get; }
    INode To { get; }
}
