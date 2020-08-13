using OrthogonalConnectorRouting.Graph.ShortestPathAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrthogonalConnectorRouting.Graph
{
    internal class Graph<N, E, K> : IGraph<N, E, K> where N : INode<K>
                                                    where E : IEdge<N, K>
                                                    where K : IComparable
    {
        private readonly IPriorityBST<GraphVertex, K> tree;

        public Graph()
        {
            this.tree = new PriorityBST<GraphVertex, K>();
        }

        public int NodesCount => this.tree.Count;

        public void AddEdge(N firstNode, N secondNode, E edge)
        {
            var genericEdge = new GraphEdge
            {
                Source = this.tree.Find(edge.Source.X, edge.Source.Y),
                Destination = this.tree.Find(edge.Destination.X, edge.Destination.Y),
                Weight = edge.Weight,
                Data = edge
            };

            var first = this.tree.Find(firstNode.X, firstNode.Y);
            first?.Edges.Add(genericEdge);

            var second = this.tree.Find(secondNode.X, secondNode.Y);
            second?.Edges.Add(genericEdge);
        }

        public void AddNode(N node)
        {
            var genericNode = new GraphVertex
            {
                Key = node.Key,
                X = node.X,
                Y = node.Y,
                Data = node
            };

            this.tree.Insert(genericNode);
        }

        public void AddNodes(IEnumerable<N> collection)
        {
            foreach (var node in collection)
            {
                var genericNode = new GraphVertex
                {
                    Key = node.Key,
                    X = node.X,
                    Y = node.Y,
                    Data = node
                };

                this.tree.Insert(genericNode);
            }
        }

        public void Clear()
        {
            this.tree.Clear();
        }

        public N Find(K key)
        {
            var result = this.tree.Find(key);
            return result != default(GraphVertex) ? result.Data : default(N);
        }

        public N Find(double x, double y)
        {
            var result = this.tree.Find(x, y);
            return result != default(GraphVertex) ? result.Data : default(N);
        }

        public List<E> FindEdges(N node)
        {
            var edges = new List<E>();

            var treeNode = this.tree.Find(node.X, node.Y);
            if (treeNode == null)
            {
                return edges;
            }

            foreach (var n in this.tree.Nodes)
            {
                foreach (var e in n.Edges)
                {
                    if (e.Source.Key.Equals(node.Key) || e.Destination.Key.Equals(node.Key))
                    {
                        edges.Add(e.Data);
                    }
                }
            }

            return edges;
        }

        public List<N> IntervalFind(double x1, double y1, double x2, double y2)
        {
            var result = this.tree.IntervalFind(x1, y1, x2, y2);
            var interval = new List<N>();

            foreach (var item in result)
            {
                interval.Add(item.Data);
            }

            return interval;
        }

        public void RemoveEdge(E edge)
        {
            this.RemoveEdge(edge.Source, edge.Destination);
        }

        public void RemoveEdge(N firstNode, N secondNode)
        {
            var first = this.tree.Find(firstNode.X, firstNode.Y);
            var second = this.tree.Find(secondNode.X, secondNode.Y);

            if (first == null || second == null)
            {
                return;
            }

            var edge = first.Edges.Except(second.Edges).FirstOrDefault();
            first.Edges.Remove(edge);
            second.Edges.Remove(edge);
        }

        public void RemoveNode(N node)
        {
            this.tree.Remove(this.tree.Find(node.X, node.Y));
        }

        public (List<N> pathNodes, List<E> pathEdges) ShortestPath<T>(N startNode, N finishNode)
            where T : ISearchAlgorithm, new()
        {

            var searchAlgorithm = new T();
            var results = searchAlgorithm.ShortestPath(this.tree, startNode, finishNode);

            return results;
        }

        #region Private classes
        internal class GraphVertex : INode<K>
        {
            public K Key { get; set; }

            public double X { get; set; }

            public double Y { get; set; }

            public N Data { get; set; }

            public List<GraphEdge> Edges { get; set; }

            public GraphVertex()
            {
                this.Edges = new List<GraphEdge>();
            }

            public override string ToString()
            {
                return this.Data.ToString();
            }
        }

        internal class GraphEdge : IEdge<GraphVertex, K>
        {
            public K Key => this.Data.Key;

            public E Data { get; set; }

            public GraphVertex Source { get; set; }

            public GraphVertex Destination { get; set; }

            public double Weight { get; set; }
        }
        #endregion
    }
}
