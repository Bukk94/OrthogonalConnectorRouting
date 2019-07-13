using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public class Connection
    {
        public List<Point> Points { get; set; }

        public Point Start => this.Points[0];

        public Point End => this.Points[1];

        public Connection(Point start, Point end)
        {
            Points = new List<Point>() { start, end };
        }

    }
}
