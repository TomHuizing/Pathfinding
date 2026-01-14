using System;

namespace Pathfinding;

public class Graph(IEnumerable<IEdge> edges) : IGraph
{
    public IEnumerable<IEdge> Edges => edges;
}
