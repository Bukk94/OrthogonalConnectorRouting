using OrthogonalConnectorRouting.PriorityQueue;
using System;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Graph.ShortestPathAlgorithm
{
    internal class DijkstraAlgorithm : ISearchAlgorithm 
    {
        public (List<N> pathNodes, List<E> pathEdges) ShortestPath<N, E, K>(IPriorityBST<Graph<N, E, K>.GraphVertex, K> tree, N startNode, N finishNode)
            where N : INode<K>
            where E : IEdge<N, K>
            where K : IComparable
        {
            IPriorityQueue<Graph<N, E, K>.GraphVertex, double> minPQ = new PriorityQueue<Graph<N, E, K>.GraphVertex, double>();
            var totalCosts = new Dictionary<Graph<N, E, K>.GraphVertex, double>();
            var prevNodes = new Dictionary<Graph<N, E, K>.GraphVertex, Graph<N, E, K>.GraphVertex>();
            var visited = new List<Graph<N, E, K>.GraphVertex>();
            var paths = new Dictionary<Graph<N, E, K>.GraphVertex, Graph<N, E, K>.GraphEdge>();

            var treeStartNode = tree.Find(startNode.X, startNode.Y);
            totalCosts.Add(treeStartNode, 0);
            minPQ.Enqueue(treeStartNode, 0);

            foreach (var node in tree.Nodes)
            {
                if (!node.Key.Equals(startNode.Key))
                {
                    totalCosts.Add(node, double.MaxValue);
                    minPQ.Enqueue(node, double.MaxValue);
                }
            }

            while (minPQ.Count > 0)
            {
                var newSmallest = minPQ.Dequeue();
                visited.Add(newSmallest);

                foreach (var edge in newSmallest.Edges)
                {
                    var possiblyUnvisitedNode = edge.Source == newSmallest ? edge.Destination : edge.Source;

                    if (!visited.Contains(possiblyUnvisitedNode))
                    {
                        var altPath = totalCosts[newSmallest] + edge.Weight;

                        if (totalCosts.ContainsKey(possiblyUnvisitedNode) && altPath < totalCosts[possiblyUnvisitedNode])
                        {
                            totalCosts[possiblyUnvisitedNode] = altPath;
                            prevNodes[possiblyUnvisitedNode] = newSmallest;
                            paths[possiblyUnvisitedNode] = edge;

                            minPQ.UpdatePriority(possiblyUnvisitedNode, altPath);
                        }
                    }
                }
            }

            var edges = new List<E>();
            var last = tree.Find(finishNode.X, finishNode.Y);
            var shortestPath = new List<N>();

            while (prevNodes.ContainsKey(last))
            {
                edges.Add(paths[last].Data);
                shortestPath.Add(last.Data);
                last = prevNodes[last];
            }

            shortestPath.Add(startNode);
            shortestPath.Reverse();

            return (shortestPath, edges);
        }
    }
}
