namespace OrthogonalConnectorRouting
{
    public class Edge<T>
    {
        public Edge(Vertex<T> from, Vertex<T> to)
        {
            this.From = from;
            this.To = to;
        }

        public Vertex<T> From { get; }

        public Vertex<T> To { get; }
    }
}
