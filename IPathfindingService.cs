using System;

namespace Pathfinding;

public interface IPathfindingService
{
    bool TryFindPath(INode start, INode target, IGraph graph, out INode[] path);
}
