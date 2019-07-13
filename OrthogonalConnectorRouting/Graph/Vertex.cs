using System.Collections.Generic;

namespace OrthogonalConnectorRouting
{
    public class Vertex<T>
    {
        public Vertex(T value)
        {
            this.Value = value;
            this.Edges = new List<Edge<T>>();
        }

        public T Value { get; }

        public IList<Edge<T>> Edges { get; }
    }
}
