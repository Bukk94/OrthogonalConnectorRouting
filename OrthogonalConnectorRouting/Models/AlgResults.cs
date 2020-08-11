using System.Collections.Generic;
using System.Windows;

namespace OrthogonalConnectorRouting.Models
{
    public class AlgResults
    {
        public IEnumerable<Connection> Connections { get; set; }
        public IEnumerable<Point> Intersections { get; set; }
        public ShortestGraphPath ShortestPath { get; set; }
    }
}
