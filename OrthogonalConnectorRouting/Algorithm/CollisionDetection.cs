using OrthogonalConnectorRouting.Models.InternalModels;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Algorithm
{
    internal class CollisionDetection
    {
        internal IEnumerable<Connection> DetectCollision(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            if (detectedCollisions.Count == 1)
            {
                return this.SingleCollision(detectedCollisions, data);
            }
            else if (detectedCollisions.Count > 1)
            {
                return this.MultipleCollisions(detectedCollisions, data);
            }
            else
            {
                // No collision
                var self = new List<Connection>();
                this.CreateConnection(data.StartX, data.StartY, data.EndX, data.EndY, self);
                return self;
            }
        }

        internal IEnumerable<Connection> SingleCollision(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            var collisionConnections = new List<Connection>();

            // Left collision
            if (detectedCollisions[0].B < data.StartPoint)
            {
                if (data.IsVertical)
                {
                    this.CreateConnection(data.StartX, detectedCollisions[0].B, data.EndX, data.EndY, collisionConnections);
                }
                else
                {
                    this.CreateConnection(detectedCollisions[0].B, data.StartY, data.EndX, data.EndY, collisionConnections);
                }
            }
            else
            {
                // Right collision
                if (data.IsVertical)
                {
                    this.CreateConnection(data.StartX, data.StartY, data.EndX, detectedCollisions[0].A, collisionConnections);
                }
                else
                {
                    this.CreateConnection(data.StartX, data.StartY, detectedCollisions[0].A, data.EndY, collisionConnections);
                }
            }

            return collisionConnections;
        }

        internal IEnumerable<Connection> MultipleCollisions(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            var collisionConnections = new List<Connection>();

            this.FindMinMax(detectedCollisions, data, out double minimum, out double maximum);
            if (data.IsVertical)
            {
                this.CreateConnection(data.StartX, minimum, data.EndX, maximum, collisionConnections);
            }
            else
            {
                this.CreateConnection(minimum, data.StartY, maximum, data.EndY, collisionConnections);
            }

            return collisionConnections;
        }

        internal IEnumerable<Connection> ConnectorPointsCollisionDetection(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            var collisionConnections = new List<Connection>();

            if (detectedCollisions.Count == 0)
            {
                this.CreateConnection(data.StartX, data.StartY, data.EndX, data.EndY, collisionConnections);

                if (data.IsVertical)
                {
                    this.CreateConnection(data.StartX, data.OppositeSide, data.EndX, data.Maximum, collisionConnections);
                }
                else
                {
                    this.CreateConnection(data.OppositeSide, data.StartY, data.Maximum, data.EndY, collisionConnections);
                }
            }
            else if (detectedCollisions.Count > 0)
            {
                this.FindMinMax(detectedCollisions, data, out double minimum, out double maximum);
                if (data.IsVertical)
                {
                    this.CreateConnection(data.StartX, minimum, data.EndX, data.EndY, collisionConnections);
                    this.CreateConnection(data.StartX, data.OppositeSide, data.EndX, maximum, collisionConnections);
                }
                else
                {
                    this.CreateConnection(minimum, data.StartY, data.EndX, data.EndY, collisionConnections);
                    this.CreateConnection(data.OppositeSide, data.StartY, maximum, data.EndY, collisionConnections);
                }
            }

            return collisionConnections;
        }

        private Connection CreateConnection(double startX, double startY, double endX, double endY, List<Connection> connections)
        {
            if (connections == null)
            {
                connections = new List<Connection>();
            }

            var collisionConnection = new Connection(new Models.Point(startX, startY), new Models.Point(endX, endY));
            connections.Add(collisionConnection);

            return collisionConnection;
        }

        private void FindMinMax(List<(double A, double B)> detectedCollisions, CollisionData data, out double minimum, out double maximum)
        {
            minimum = 0;
            maximum = data.Maximum;
            foreach (var collision in detectedCollisions)
            {
                if (collision.B < data.StartPoint && collision.B > minimum)
                {
                    minimum = collision.B;
                }
                else if (collision.A > data.StartPoint && collision.A < maximum)
                {
                    maximum = collision.A;
                }
            }
        }
    }
}
