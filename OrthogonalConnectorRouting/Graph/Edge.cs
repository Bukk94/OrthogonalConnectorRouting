namespace OrthogonalConnectorRouting.Graph
{
    public class Edge : IEdge<Node, string> // Edge<N, T> : IEdge<N, T> where N : INode<T>
    {
        public string Key { get; private set; }

        public Node Source { get; set; }

        public Node Destination { get; set; }

        public double Weight { get; set; }

        public Edge()
        {
        }

        public Edge(string key)
        {
            this.Key = key;
        }
    }
}
