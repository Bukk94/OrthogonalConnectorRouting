using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace OrthogonalConnectorRouting
{
    public class Graph<TKey, T>
    {
        public Graph()
        {
            this.Vertices = new Dictionary<TKey, Vertex<T>>();
        }

        public IDictionary<TKey, Vertex<T>> Vertices { get; }

        public void AddVertex(TKey key, T value)
        {
            if (this.Vertices.ContainsKey(key))
            {
                return;
            }

            this.Vertices.Add(key, new Vertex<T>(value));
        }

        public void AddEdge(TKey from, TKey to)
        {
            if (!(this.Vertices.ContainsKey(from) && this.Vertices.ContainsKey(to)))
            {
                throw new ArgumentNullException();
            }

            this.Vertices.TryGetValue(from, out var fromVertex);
            this.Vertices.TryGetValue(to, out var toVertex);

            var edge = new Edge<T>(fromVertex, toVertex);
            fromVertex?.Edges.Add(edge);
            toVertex?.Edges.Add(edge);
        }

        public void DeleteEdge(TKey from, TKey to)
        {
            this.Vertices.TryGetValue(from, out var fromVertex);
            this.Vertices.TryGetValue(to, out var toVertex);

            var edge = fromVertex?.Edges.FirstOrDefault(x => x.From == fromVertex && x.To == toVertex);
            fromVertex?.Edges.Remove(edge);
            toVertex?.Edges.Remove(edge);
        }

        public void DeleteVertex(TKey key)
        {
            if (!this.Vertices.ContainsKey(key))
            {
                throw new ArgumentNullException();
            }

            this.Vertices.TryGetValue(key, out var vertexToDel);

            foreach (var edge in vertexToDel.Edges)
            {
                var from = edge.From;
                var to = edge.To;

                if (!vertexToDel.Equals(from))
                {
                    from.Edges.Remove(from.Edges.FirstOrDefault(x => x.Equals(edge)));
                }

                if (!vertexToDel.Equals(to))
                {
                    to.Edges.Remove(to.Edges.FirstOrDefault(x => x.Equals(edge)));
                }
            }

            this.Vertices.Remove(key);
        }

        public void Clear()
        {
            this.Vertices.Clear();
        }
    }
}
