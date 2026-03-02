using Microsoft.Extensions.DependencyInjection;
using TomHuizing.Pathfinding;

namespace Tomhuizing.Pathfinding.Tests
{
    [TestClass]
    public class PathfindingTests
    {
        private IPathfindingService _service = new ServiceCollection()
            .AddPathfinding()
            .BuildServiceProvider()
            .GetRequiredService<IPathfindingService>();

        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddPathfinding();
            var provider = services.BuildServiceProvider();
            _service = provider.GetRequiredService<IPathfindingService>();
        }

        #region IsValidPath tests

        [TestMethod]
        public void IsValidPath_EmptyPath_ReturnsTrue()
        {
            var costService = new TestCostService();
            Assert.IsTrue(_service.IsValidPath(Array.Empty<IEdge>(), costService));
        }

        [TestMethod]
        public void IsValidPath_NonContiguousEdges_ReturnsFalse()
        {
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");
            var edge1 = new Edge(a, b);
            var edge2 = new Edge(c, a); // not contiguous
            var costService = new TestCostService();
            costService.SetTraversalCost(edge1, 1);
            costService.SetTraversalCost(edge2, 1);

            Assert.IsFalse(_service.IsValidPath(new[] { edge1, edge2 }, costService));
        }

        [TestMethod]
        public void IsValidPath_MissingCost_ReturnsFalse()
        {
            var a = new Node("A");
            var b = new Node("B");
            var edge = new Edge(a, b);
            var costService = new TestCostService();
            // do not register cost

            Assert.IsFalse(_service.IsValidPath(new[] { edge }, costService));
        }

        [TestMethod]
        public void IsValidPath_ValidPath_ReturnsTrue()
        {
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");
            var e1 = new Edge(a, b);
            var e2 = new Edge(b, c);
            var costService = new TestCostService();
            costService.SetTraversalCost(e1, 1);
            costService.SetTraversalCost(e2, 2);

            Assert.IsTrue(_service.IsValidPath(new[] { e1, e2 }, costService));
        }

        #endregion

        #region TryFindPath tests

        [TestMethod]
        public void TryFindPath_StartEqualsTarget_ReturnsEmptyPath()
        {
            var node = new Node("X");
            var graph = new Graph(Array.Empty<IEdge>());
            var costService = new TestCostService();
            costService.SetHeuristic(node, node, 0);

            Assert.IsTrue(_service.TryFindPath(node, node, graph, costService, out var path));
            Assert.AreEqual(0, path.Length);
        }

        [TestMethod]
        public void TryFindPath_MissingStartOrTargetEdges_ReturnsFalse()
        {
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");
            var edge = new Edge(a, b);
            var graph = new Graph(new[] { edge });
            var costService = new TestCostService();
            costService.SetHeuristic(a, c, 0);
            costService.SetTraversalCost(edge, 1);

            Assert.IsFalse(_service.TryFindPath(a, c, graph, costService, out _));
            Assert.IsFalse(_service.TryFindPath(c, b, graph, costService, out _));
        }

        [TestMethod]
        public void TryFindPath_HeuristicUnavailable_ReturnsFalse()
        {
            var a = new Node("A");
            var b = new Node("B");
            var edge = new Edge(a, b);
            var graph = new Graph(new[] { edge });
            var costService = new TestCostService();
            // heuristic not set, traversal cost set so we get stuck in the early check
            costService.SetTraversalCost(edge, 1);

            Assert.IsFalse(_service.TryFindPath(a, b, graph, costService, out _));
        }

        [TestMethod]
        public void TryFindPath_TraversalCostUnavailable_SkipsEdgeAndFails()
        {
            var a = new Node("A");
            var b = new Node("B");
            var edge = new Edge(a, b);
            var graph = new Graph(new[] { edge });
            var costService = new TestCostService();
            costService.SetHeuristic(a, b, 0);
            // missing traversal cost

            Assert.IsFalse(_service.TryFindPath(a, b, graph, costService, out _));
        }

        [TestMethod]
        public void TryFindPath_SimpleGraph_FindsDirectPath()
        {
            var a = new Node("A");
            var b = new Node("B");
            var edge = new Edge(a, b);
            var graph = new Graph(new[] { edge });
            var costService = new TestCostService();
            costService.SetHeuristic(a, b, 5);
            costService.SetHeuristic(b, b, 0); // heuristic for target-to-self is required by algorithm
            costService.SetTraversalCost(edge, 1);

            bool found = _service.TryFindPath(a, b, graph, costService, out var path);
            Assert.IsTrue(found);
            Assert.AreEqual(1, path.Length);
            Assert.AreSame(edge, path[0]);
        }

        [TestMethod]
        public void TryFindPath_ChoosesLowestCostPath_WhenMultipleOptionsExist()
        {
            var a = new Node("A");
            var b = new Node("B");
            var c = new Node("C");

            var ab = new Edge(a, b);
            var ac = new Edge(a, c);
            var cb = new Edge(c, b);

            // graph with A->B (cost 10) and A->C->B (cost 1+1)
            var graph = new Graph(new IEdge[] { ab, ac, cb });

            var costService = new TestCostService();
            // heuristics all zero so algorithm reduces to Dijkstra-like
            costService.SetHeuristic(a, b, 0);
            costService.SetHeuristic(c, b, 0);
            costService.SetHeuristic(a, c, 0);
            costService.SetHeuristic(b, b, 0);

            costService.SetTraversalCost(ab, 10);
            costService.SetTraversalCost(ac, 1);
            costService.SetTraversalCost(cb, 1);

            bool found = _service.TryFindPath(a, b, graph, costService, out var path);
            Assert.IsTrue(found);
            Assert.AreEqual(2, path.Length);
            CollectionAssert.AreEqual(new[] { ac, cb }, path);
        }

        #endregion

        [TestMethod]
        public void DIExtensions_AddPathfinding_RegistersService()
        {
            var services = new ServiceCollection();
            services.AddPathfinding();
            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IPathfindingService>();
            Assert.IsNotNull(service);
        }

        #region Helper types

        private sealed class Node : INode
        {
            public string Name { get; }
            public Node(string name) => Name = name;
            public override string ToString() => Name;
        }

        private sealed class TestCostService : ICostService
        {
            private readonly Dictionary<IEdge, double> _costs = new();
            private readonly Dictionary<(INode, INode), double> _heuristics = new();

            public void SetTraversalCost(IEdge edge, double cost)
                => _costs[edge] = cost;

            public void SetHeuristic(INode a, INode b, double heuristic)
                => _heuristics[(a, b)] = heuristic;

            public bool TryGetTraversalCost(IEdge edge, out double cost)
                => _costs.TryGetValue(edge, out cost);

            public bool TryGetHeuristic(INode start, INode target, out double heuristic)
                => _heuristics.TryGetValue((start, target), out heuristic);
        }

        #endregion
    }
}
