namespace OrthogonalConnectorRouting.Graph
{
    public interface IEdge<N, K> where N : INode<K>
    {
        K Key { get; }

        N Source { get; set; }

        N Destination { get; set; }

        double Weight { get; set; }
    }
}
