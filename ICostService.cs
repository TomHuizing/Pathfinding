using System;

namespace Pathfinding;

public interface ICostService
{
    bool TryGetHeuristic(INode start, INode target, out double heuristic);
    bool TryGetTraversalCost(IEdge edge, out double cost);
}
