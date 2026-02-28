namespace TomHuizing.Pathfinding;

public interface IPathfindingService
{
    bool IsValidPath(IEdge[] path, ICostService costService);
    bool TryFindPath(INode start, INode target, IGraph graph, ICostService costService, out IEdge[] path);
}
