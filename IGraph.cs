using System;

namespace Pathfinding;

public interface IGraph
{
    IEnumerable<IEdge> Edges { get; }
}
