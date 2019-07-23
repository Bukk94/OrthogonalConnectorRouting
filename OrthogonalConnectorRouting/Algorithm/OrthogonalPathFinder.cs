using OrthogonalConnectorRouting.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public class OrthogonalPathFinder : IOrthogonalPathFinder
    {
        private List<Connection> _connections;
        private IGraph<Node, Edge, string> _graph;

        public double Margin { get; set; } = 0;
    
        public OrthogonalPathFinder()
        {
            this._connections = new List<Connection>();
            this._graph = new Graph<Node, Edge, string>();
        }

        public void ConstructGraph(List<Point> intersections)
        {
            this._graph.Clear();

            foreach (var intersection in intersections)
            {
                var edges = new List<Connection>();

                // Find edges
                foreach (var connection in this._connections)
                {
                    if (this.IsInsideLine(connection, intersection.X, intersection.Y))
                    {
                        edges.Add(connection);
                    }

                    // Every intersection has only two edges
                    if (edges.Count == 2)
                    {
                        break;
                    }
                }

                // Find possible neighbors
                var possibleNeighbors = new List<Point>();
                foreach (var edge in edges)
                {
                    foreach (var point in intersections)
                    {
                        if (this.IsInsideLine(edge, point.X, point.Y) && intersection != point)
                        {
                            possibleNeighbors.Add(point);
                        }
                    }
                }

                // Find neareast neighbors
                Point? left, right, top, bottom;
                left = right = top = bottom = null;
                foreach (var neighbor in possibleNeighbors)
                {
                    if (neighbor.X < intersection.X && neighbor.Y == intersection.Y && (!left.HasValue || left.Value.X < neighbor.X))
                    {
                        left = neighbor;
                    }
                    else if (neighbor.X > intersection.X && neighbor.Y == intersection.Y && (!right.HasValue || right.Value.X > neighbor.X))
                    {
                        right = neighbor;
                    }
                    else if (neighbor.Y < intersection.Y && neighbor.X == intersection.X && (!top.HasValue || top.Value.Y < neighbor.Y))
                    {
                        top = neighbor;
                    }
                    else if (neighbor.Y > intersection.Y && neighbor.X == intersection.X && (!bottom.HasValue || bottom.Value.Y > neighbor.Y))
                    {
                        bottom = neighbor;
                    }
                }

                // Add vertices and edges to the graph
                this.AddNode(intersection);
                var vertices = new Point?[] { left, right, top, bottom };
                foreach (var vertex in vertices)
                {
                    if (vertex.HasValue)
                    {
                        this.AddNode(vertex.Value);
                        this.AddEdge(intersection, vertex.Value);
                    }
                }
            }
        }

        public (List<Node> pathNodes, List<Edge> pathEdges) ShortestPath(Node startNode, Node finishNode)
        {
            return this._graph.ShortestPath(startNode, finishNode);
        }

        private void AddEdge(Point sourcePoint, Point destinationPoint)
        {
            var source = this._graph.Find(sourcePoint.X, sourcePoint.Y);
            var dest = this._graph.Find(destinationPoint.X, destinationPoint.Y);
            Edge edge = new Edge
            {
                Source = source,
                Destination = dest,
                Weight = Distance(new Point(sourcePoint.X, sourcePoint.Y), new Point(destinationPoint.X, destinationPoint.Y))
            };

            this._graph.AddEdge(source, dest, edge);
        }

        public void AddNode(Point data)
        {
            Node node = new Node(data.X, data.Y);
            if (this._graph.Find(data.X, data.Y) == null)
            {
                this._graph.AddNode(node);
            }
        }

        public Point? FindIntersection(Connection lineA, Connection lineB)
        {
            double A1 = lineA.End.Y - lineA.Start.Y;
            double B1 = lineA.Start.X - lineA.End.X;
            double C1 = A1 * lineA.Start.X + B1 * lineA.Start.Y;

            double A2 = lineB.End.Y - lineB.Start.Y;
            double B2 = lineB.Start.X - lineB.End.X;
            double C2 = A2 * lineB.Start.X + B2 * lineB.Start.Y;

            double determinant = A1 * B2 - A2 * B1;
            if (determinant == 0)
            {
                return null;
            }

            double x = (B2 * C1 - B1 * C2) / determinant;
            double y = (A1 * C2 - A2 * C1) / determinant;

            // x,y can intersect outside the line since line is infinitely long
            // so finally check if x, y is within both the line segments
            if (this.IsInsideLine(lineA, x, y) && this.IsInsideLine(lineB, x, y))
            {
                return new Point(x, y);
            }

            return null;
        }

        public List<Connection> CreateLeadLines(List<DesignerItem> items, double maxWidth, double maxHeight)
        {
            maxWidth -= this.Margin;
            maxHeight -= this.Margin;
            this._connections = new List<Connection>();
            var xAxisItems = new Dictionary<(double Y, double Bottom), (double X, double Right)>();
            var yAxisItems = new Dictionary<(double X, double Right), (double Y, double Bottom)>();

            // Create Horizontal & Vertical dictionaries
            foreach (var item in items)
            {
                var yRange = (item.Y - this.Margin, item.Bottom + this.Margin);
                var xRange = (item.X - this.Margin, item.Right + this.Margin);
                if (!xAxisItems.ContainsKey(yRange))
                {
                    xAxisItems.Add(yRange, xRange);
                }

                if (!yAxisItems.ContainsKey(xRange))
                {
                    yAxisItems.Add(xRange, yRange);
                }
            }

            // Create lead lines
            foreach (var item in items)
            {
                #region Vertical Lines

                var leftCollisions = new List<(double Y, double Bottom)>();
                var rightCollisions = new List<(double Y, double Bottom)>();
                var verticalMiddleCollisions = new List<(double Y, double Bottom)>();

                var median = this.CalcMedian(item.X, item.Right);
                foreach (var range in yAxisItems.Keys)
                {
                    if ((item.X - this.Margin) == range.X && (item.Right + this.Margin) == range.Right)
                    {
                        continue;
                    }

                    if ((item.X - this.Margin) >= range.X && (item.X - this.Margin) <= range.Right)
                    {
                        leftCollisions.Add(yAxisItems[range]);
                    }

                    if ((item.Right + this.Margin) >= range.X && (item.Right + this.Margin) <= range.Right)
                    {
                        rightCollisions.Add(yAxisItems[range]);
                    }

                    if (median >= range.X && median <= range.Right)
                    {
                        verticalMiddleCollisions.Add(yAxisItems[range]);
                    }
                }

                var data = new CollisionData
                {
                    StartPoint = item.Y - this.Margin,
                    StartX = item.X - this.Margin,
                    StartY = this.Margin, // Zero position
                    EndX = item.X - this.Margin,
                    EndY = maxHeight,
                    Maximum = maxHeight,
                    IsVertical = true
                };

                this.CollisionDetection(leftCollisions, data);

                data.StartX = data.EndX = item.Right + this.Margin;
                this.CollisionDetection(rightCollisions, data);

                data = new CollisionData
                {
                    StartPoint = item.Y - this.Margin,
                    StartX = this.CalcMedian(item.X, item.Right),
                    StartY = this.Margin, // Zero position
                    EndX = this.CalcMedian(item.X, item.Right),
                    EndY = item.Y - this.Margin,
                    OppositeSide = item.Bottom + this.Margin,
                    Maximum = maxHeight,
                    IsVertical = true
                };
                
                this.ConnectorPointsCollisionDetection(verticalMiddleCollisions, data);
                #endregion

                #region Horizontal Lines

                var topCollisions = new List<(double X, double Right)>();
                var bottomCollisions = new List<(double X, double Right)>();
                var horizontalMiddleCollisions = new List<(double X, double Right)>();

                median = this.CalcMedian(item.Y, item.Bottom);
                foreach (var range in xAxisItems.Keys)
                {
                    if ((item.Y - this.Margin) == range.Y && (item.Bottom + this.Margin) == range.Bottom)
                    {
                        continue;
                    }

                    if ((item.Y - this.Margin) >= range.Y && (item.Y - this.Margin) <= range.Bottom)
                    {
                        topCollisions.Add(xAxisItems[range]);
                    }

                    if ((item.Bottom + this.Margin) >= range.Y && (item.Bottom + this.Margin) <= range.Bottom)
                    {
                        bottomCollisions.Add(xAxisItems[range]);
                    }

                    if (median >= range.Y && median <= range.Bottom)
                    {
                        horizontalMiddleCollisions.Add(xAxisItems[range]);
                    }
                }

                data = new CollisionData
                {
                    StartPoint = item.X - this.Margin,
                    StartX = this.Margin, // Zero position
                    StartY = item.Y - this.Margin,
                    EndX = maxWidth,
                    EndY = item.Y - this.Margin,
                    Maximum = maxWidth,
                    IsVertical = false
                };

                this.CollisionDetection(topCollisions, data);
                
                data.StartY = data.EndY = item.Bottom + this.Margin;
                this.CollisionDetection(bottomCollisions, data);

                data = new CollisionData
                {
                    StartPoint = item.X - this.Margin,
                    StartX = this.Margin, // Zero position
                    StartY = this.CalcMedian(item.Y, item.Bottom),
                    EndX = item.X - this.Margin,
                    EndY = this.CalcMedian(item.Y, item.Bottom),
                    OppositeSide = item.Right + this.Margin,
                    Maximum = maxWidth,
                    IsVertical = false
                };
                
                this.ConnectorPointsCollisionDetection(horizontalMiddleCollisions, data);
                #endregion
            }

            return this._connections;
        }

        private void CollisionDetection(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            if (detectedCollisions.Count == 1)
            {
                this.SingleCollision(detectedCollisions, data);
            }
            else if (detectedCollisions.Count > 1)
            {
                this.MultipleCollisions(detectedCollisions, data);
            }
            else
            {
                // No collision
                this.AddConnection(data.StartX, data.StartY, data.EndX, data.EndY);
            }
        }

        private void ConnectorPointsCollisionDetection(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            if (detectedCollisions.Count == 0)
            {
                this.AddConnection(data.StartX, data.StartY, data.EndX, data.EndY);
                if (data.IsVertical)
                {
                    this.AddConnection(data.StartX, data.OppositeSide, data.EndX, data.Maximum);
                }
                else
                {
                    this.AddConnection(data.OppositeSide, data.StartY, data.Maximum, data.EndY);
                }
            }
            else if (detectedCollisions.Count > 0)
            {
                this.FindMinMax(detectedCollisions, data, out double minimum, out double maximum);
                if (data.IsVertical)
                {
                    this.AddConnection(data.StartX, minimum, data.EndX, data.EndY);
                    this.AddConnection(data.StartX, data.OppositeSide, data.EndX, maximum);
                }
                else
                {
                    this.AddConnection(minimum, data.StartY, data.EndX, data.EndY);
                    this.AddConnection(data.OppositeSide, data.StartY, maximum, data.EndY);
                }
            }
        }

        private void MultipleCollisions(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            this.FindMinMax(detectedCollisions, data, out double minimum, out double maximum);
            if (data.IsVertical)
            {
                this.AddConnection(data.StartX, minimum, data.EndX, maximum);
            }
            else
            {
                this.AddConnection(minimum, data.StartY, maximum, data.EndY);
            }
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

        private void SingleCollision(List<(double A, double B)> detectedCollisions, CollisionData data)
        {
            // Left collision
            if (detectedCollisions[0].B < data.StartPoint)
            {
                if (data.IsVertical)
                {
                    this.AddConnection(data.StartX, detectedCollisions[0].B, data.EndX, data.EndY);
                }
                else
                {
                    this.AddConnection(detectedCollisions[0].B, data.StartY, data.EndX, data.EndY);
                }
            }
            else
            {
                // Right collision
                if (data.IsVertical)
                {
                    this.AddConnection(data.StartX, data.StartY, data.EndX, detectedCollisions[0].A);
                }
                else
                {
                    this.AddConnection(data.StartX, data.StartY, detectedCollisions[0].A, data.EndY);
                }
            }
        }

        private void AddConnection(double startX, double startY, double endX, double endY)
        {
            this._connections.Add(new Connection(new Point(startX, startY), new Point(endX, endY)));
        }

        private double CalcMedian(double a, double b)
        {
            return (a + b) / 2;
        }

        private bool IsInsideLine(Connection line, double x, double y)
        {
            return (x >= line.Start.X && x <= line.End.X || x >= line.End.X && x <= line.Start.X)
                   && (y >= line.Start.Y && y <= line.End.Y || y >= line.End.Y && y <= line.Start.Y);
        }

        private double Distance(Point source, Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        }

        private class CollisionData
        {
            public double StartPoint { get; set; }

            public double StartX { get; set; }

            public double StartY { get; set; }

            public double EndX { get; set; }

            public double EndY { get; set; }

            public double Maximum { get; set; }

            public double OppositeSide { get; set; }

            public bool IsVertical { get; set; }
        }
    }
}
