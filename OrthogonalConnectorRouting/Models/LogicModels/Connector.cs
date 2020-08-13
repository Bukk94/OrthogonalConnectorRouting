using OrthogonalConnectorRouting.Enums;
using OrthogonalConnectorRouting.Graph;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Models
{
    public class Connector
    {
        public Connector()
        {
            this.ConnectorPath = new List<Connection>();
        }

        public List<Connection> ConnectorPath { get; set; }

        public IInput Source { get; set; }

        public IInput Destinaton { get; set; }

        public ConnectorOrientation SourceOrientation { get; set; }

        public ConnectorOrientation DestinationOrientation { get; set; }

        public Node SourceNode => this.CalculateOrientationNode(this.SourceOrientation, this.Source);

        public Node DestinatonNode => this.CalculateOrientationNode(this.DestinationOrientation, this.Destinaton);

        private Node CalculateOrientationNode(ConnectorOrientation orientation, IInput item)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    return new Node(item.X, item.Y + item.Height / 2);
                case ConnectorOrientation.Right:
                    return new Node(item.Right, item.Y + item.Height / 2);
                case ConnectorOrientation.Top:
                    return new Node(item.X + item.Width / 2, item.Y);
                case ConnectorOrientation.Bottom:
                    return new Node(item.X + item.Width / 2, item.Bottom);
                default:
                    return null;
            }
        }
    }
}
