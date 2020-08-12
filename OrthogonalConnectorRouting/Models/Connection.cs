using System.Collections.Generic;

namespace OrthogonalConnectorRouting
{
    public class Connection
    {
        public List<Models.Point> Points { get; set; }

        public Models.Point Start => this.Points[0];

        public Models.Point End => this.Points[1];

        public Connection(Models.Point start, Models.Point end)
        {
            Points = new List<Models.Point>() { start, end };
        }
    }
}
