using OrthogonalConnectorRouting.Graph;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Models
{
    public class ShortestGraphPath
    {
        public IEnumerable<Node> PathNodes { get; set; }
        public IEnumerable<Edge> PathEdges { get; set; }
    }
}
