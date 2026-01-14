using System;

namespace Pathfinding;

public interface IEdge
{
    INode From { get; }
    INode To { get; }
}
