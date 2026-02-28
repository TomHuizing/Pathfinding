namespace TomHuizing.Pathfinding;

internal class PathfindingService : IPathfindingService
{
    public bool IsValidPath(IEdge[] path, ICostService costService)
    {
        if(path.Length == 0)
            return true;
        for(int i = 0; i < path.Length; i++)
        {
            if(i < path.Length - 1 && path[i].To != path[i+1].From)
                return false;
            if(!costService.TryGetTraversalCost(path[i], out double _))
                return false;
        }
        return true;
    }

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
        List<IEdge> cameFrom = [];

        if(!costService.TryGetHeuristic(start, target, out double startHeuristic))
        {
            path = [];
            return false;
        }

        Dictionary<INode, double> gScore = [];
        Dictionary<INode, double> fScore = [];
        fScore.Add(start, startHeuristic);
        gScore.Add(start, 0);

        while (openSet.Count > 0)
        {
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
