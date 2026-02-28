namespace TomHuizing.Pathfinding;

public interface IGraph
{
    IEnumerable<IEdge> Edges { get; }
}
