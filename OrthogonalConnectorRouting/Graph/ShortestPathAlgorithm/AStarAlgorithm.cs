using System;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Graph.ShortestPathAlgorithm
{
    internal class AStarAlgorithm : ISearchAlgorithm
    {
        public (List<N> pathNodes, List<E> pathEdges) ShortestPath<N, E, K>(IPriorityBST<Graph<N, E, K>.GraphVertex, K> tree, N startNode, N finishNode)
            where N : INode<K>
            where E : IEdge<N, K>
            where K : IComparable
        {
            throw new NotImplementedException();
        }
    }
}
