using System;

namespace Pathfinding;

public class Graph(IEnumerable<IEdge> edges, IHeuristicService heuristicService, ITraversalCostService traversalCostService) : IGraph
{
    public IEnumerable<IEdge> Edges => edges;
    public IHeuristicService HeuristicService => heuristicService;
    public ITraversalCostService TraversalCostService => traversalCostService;
}
