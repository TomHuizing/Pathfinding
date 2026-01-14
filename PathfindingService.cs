using System;

namespace Pathfinding;

internal class PathfindingService : IPathfindingService
{
    public bool TryFindPath(INode start, INode target, IGraph graph, out INode[] path)
    {
        if(start == target)
        {
            path = [start];
            return true;
        }
        if(!graph.Edges.Any(edge => edge.From == start) || !graph.Edges.Any(edge => edge.To == target))
        {
            path = [];
            return false;
        }
        List<INode> openSet = [start];
        Dictionary<INode, INode> cameFrom = [];

        if(!graph.HeuristicService.TryGetHeuristic(start, target, out double startHeuristic))
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

                if(!graph.TraversalCostService.TryGetTraversalCost(edge, out double cost) || !graph.HeuristicService.TryGetHeuristic(neighbor, target, out double heuristic))
                    continue;
                
                double tentativeGScore = gScore[current] + cost;
                if (gScore.ContainsKey(neighbor) && tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = tentativeGScore + heuristic;
                
                if(!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
            }
        }
        
        path = [];
        return false;
    }

    private INode[] ReconstructPath(Dictionary<INode, INode> cameFrom, INode current)
    {
        List<INode> path = [current];
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path.ToArray();
    }
}
