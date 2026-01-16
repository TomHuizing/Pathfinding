using System;

namespace Pathfinding;

internal class PathfindingService : IPathfindingService
{
    public bool TryFindPath(INode start, INode target, IGraph graph, ICostService costService, out IEdge[] path)
    {
        if(start == target)
        {
            path = [];
            return true;
        }
        if(!graph.Edges.Any(edge => edge.From == start) || !graph.Edges.Any(edge => edge.To == target))
        {
            path = [];
            return false;
        }
        List<INode> openSet = [start];
        // Dictionary<INode, IEdge> cameFrom = [];
        List<IEdge> cameFrom = [];

        if(!costService.TryGetHeuristic(start, target, out double startHeuristic))
        {
            path = [];
            return false;
        }

        // gScore[n] is the cost of the cheapest path from start to n currently known
        Dictionary<INode, double> gScore = [];
        // fScore[n] = gScore[n] + h(n) where h is the heuristic
        Dictionary<INode, double> fScore = [];
        fScore.Add(start, startHeuristic);
        gScore.Add(start, 0);

        while (openSet.Count > 0)
        {
            // Find node with lowest f-score
            INode current = openSet.OrderBy(n => fScore[n]).First();
            openSet.Remove(current);
            
            if (current == target)
            {
                path = ReconstructPath(cameFrom, current);
                return true;
            }
            foreach (var edge in graph.Edges.Where(edge => edge.From == current))
            {
                INode neighbor = edge.To;

                if(!costService.TryGetTraversalCost(edge, out double cost) || !costService.TryGetHeuristic(neighbor, target, out double heuristic))
                    continue;
                
                double tentativeGScore = gScore[current] + cost;
                if (gScore.ContainsKey(neighbor) && tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom.RemoveAll(e => e.To == neighbor);
                cameFrom.Add(edge);
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = tentativeGScore + heuristic;
                
                if(!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
            }
        }
        
        path = [];
        return false;
    }

    private IEdge[] ReconstructPath(IEnumerable<IEdge> cameFrom, INode current)
    {
        // List<IEdge> path = [cameFrom[current]];
        // while (cameFrom.ContainsKey(current))
        // {
        //     current = cameFrom[current].From;
        //     path.Insert(0, cameFrom[current]);
        // }
        List<IEdge> path = [];
        INode node = current;
        while (true)
        {
            IEdge? edge = cameFrom.FirstOrDefault(e => e.To == node);
            if (edge == null)
                break;
            path.Insert(0, edge);
            node = edge.From;
        }
        return path.ToArray();
    }
}
