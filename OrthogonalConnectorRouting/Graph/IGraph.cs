using System;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Graph
{
    internal interface IGraph<N, E, K> where N : INode<K>  
                                       where E : IEdge<N, K>
                                       where K : IComparable
    {
        int NodesCount { get; }

        void AddNodes(IEnumerable<N> collection);

        void AddNode(N node);

        void AddEdge(N firstNode, N secondNode, E edge);

        void RemoveNode(N node);

        void RemoveEdge(N firstNode, N secondNode);

        void RemoveEdge(E edge);

        void Clear();

        N Find(double x, double y);

        N Find(K key);

        List<E> FindEdges(N node);

        List<N> IntervalFind(double x1, double y1, double x2, double y2);

        (List<N> pathNodes, List<E> pathEdges) ShortestPath<T>(N startNode, N finishNode)
            where T : ShortestPathAlgorithm.ISearchAlgorithm, new();
    }
}
