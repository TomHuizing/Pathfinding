using System;

namespace Pathfinding;

public class Edge(INode from, INode to) : IEdge
{
    public INode From => from;
    public INode To => to;
}
