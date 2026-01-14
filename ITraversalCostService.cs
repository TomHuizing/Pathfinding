using System;

namespace Pathfinding;

public interface ITraversalCostService
{
    bool TryGetTraversalCost(IEdge edge, out double cost);
}
