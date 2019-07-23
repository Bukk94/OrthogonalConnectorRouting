using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthogonalConnectorRouting.Graph
{
    public class Edge : IEdge<Node, string> // Edge<N, T> : IEdge<N, T> where N : INode<T>
    {
        public string Key => throw new NotImplementedException();

        public Node Source { get; set; }

        public Node Destination { get; set; }

        public double Weight { get; set; }
    }
}
