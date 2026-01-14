using System;

namespace Pathfinding;

public interface IHeuristicService
{
    bool TryGetHeuristic(INode start, INode target, out double heuristic);
}
