using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public class OrthogonalPathFinder
    {
        private List<Connection> _connections;
        private Graph<Point, Point> _graph;

        public double Margin { get; set; } = 10;

        public OrthogonalPathFinder()
        {
            this._connections = new List<Connection>();
            this._graph = new Graph<Point, Point>();
        }

        public void ConstructGraph(List<Point> intersections)
        {
            this._graph.Clear();

            foreach (var intersection in intersections)
            {
                var edges = new List<Connection>();

                // Find edges
                foreach(var connection in this._connections)
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
                        if (this.IsInsideLine(edge,point.X,point.Y) && intersection != point)
                        {
                            possibleNeighbors.Add(point);
                        }
                    }
                }

                // Find neareast neighbors
                Point? left = null;
                Point? right = null;
                Point? top = null;
                Point? bottom = null;
                foreach (var neighbor in possibleNeighbors)
                {
                    // Left
                    if (neighbor.X < intersection.X && neighbor.Y == intersection.Y && (!left.HasValue || left.Value.X < neighbor.X))
                    {
                        left = neighbor;
                    }
                    else if (neighbor.X > intersection.X && neighbor.Y == intersection.Y && (!right.HasValue || right.Value.X > neighbor.X))
                    {
                        // Right
                        right = neighbor;
                    }
                    else if (neighbor.Y < intersection.Y && neighbor.X == intersection.X && (!top.HasValue || top.Value.Y < neighbor.Y))
                    {
                        // Top
                        top = neighbor;
                    }
                    else if (neighbor.Y > intersection.Y && neighbor.X == intersection.X && (!bottom.HasValue || bottom.Value.Y > neighbor.Y))
                    {
                        // Bottom
                        bottom = neighbor;
                    }
                }
                
                // Add vertices and edges to the graph
                this._graph.AddVertex(intersection, intersection);
                var vertices = new Point?[] { left, right, top, bottom };
                foreach (var vertex in vertices)
                {
                    if (vertex.HasValue)
                    {
                        this._graph.AddVertex(vertex.Value, vertex.Value);
                        this._graph.AddEdge(intersection, vertex.Value);
                    }
                } 
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

        private bool IsInsideLine(Connection line, double x, double y)
        {
            return (x >= line.Start.X && x <= line.End.X || x >= line.End.X && x <= line.Start.X)
                   && (y >= line.Start.Y && y <= line.End.Y || y >= line.End.Y && y <= line.Start.Y);
        }

        public List<Connection> CreateLeadLines(List<DesignerItem> items, double maxWidth, double maxHeight)
        {
            // maxWidth -= Margin;
            // maxHeight -= Margin;
            this._connections = new List<Connection>();
            var xAxisItems = new Dictionary<(double Y, double Bottom), (double X, double Right)>();
            var yAxisItems = new Dictionary<(double X, double Right), (double Y, double Bottom)>();

            // Create Horizontal Dictionary
            foreach (var item in items)
            {
                var yRange = (item.Y, item.Bottom);
                var xRange = (item.X, item.Right);
                if (!xAxisItems.ContainsKey(yRange))
                {
                    xAxisItems.Add(
                        (item.Y, item.Bottom),
                        (item.X, item.Right)
                    );
                }

                if (!yAxisItems.ContainsKey(xRange))
                {
                    yAxisItems.Add(
                        (item.X, item.Right),
                        (item.Y, item.Bottom)
                    );
                }
            }

            // Create lead lines
            foreach (var item in items)
            {
                #region Vertical Lines

                var leftCollisions = new List<(double Y, double Bottom)>();
                var rightCollisions = new List<(double Y, double Bottom)>();
                foreach (var range in yAxisItems.Keys)
                {
                    if (item.X == range.X && item.Right == range.Right)
                    {
                        continue;
                    }

                    if (item.X >= range.X && item.X <= range.Right)
                    {
                        leftCollisions.Add(yAxisItems[range]);
                    }

                    if (item.Right >= range.X && item.Right <= range.Right)
                    {
                        rightCollisions.Add(yAxisItems[range]);
                    }
                }

                this.VerticalCollisionDetection(leftCollisions, item.Y, item.X, maxHeight);
                this.VerticalCollisionDetection(rightCollisions, item.Y, item.Right, maxHeight);
                #endregion

                #region Horizontal Lines

                var topCollisions = new List<(double X, double Right)>();
                var bottomCollisions = new List<(double X, double Right)>();
                foreach (var range in xAxisItems.Keys)
                {
                    if (item.Y == range.Y && item.Bottom == range.Bottom)
                    {
                        continue;
                    }

                    if (item.Y >= range.Y && item.Y <= range.Bottom)
                    {
                        topCollisions.Add(xAxisItems[range]);
                    }

                    if (item.Bottom >= range.Y && item.Bottom <= range.Bottom)
                    {
                        bottomCollisions.Add(xAxisItems[range]);
                    }
                }

                this.HorizontalCollisionDetection(topCollisions, item.X, item.Y, maxWidth);
                this.HorizontalCollisionDetection(bottomCollisions, item.X, item.Bottom, maxWidth);
                #endregion
            }

            return this._connections;
        }

        private void VerticalCollisionDetection(List<(double Y, double Bottom)> detectedCollisions, double Y, double flowXCoord, double maxHeight)
        {
            // Single collision
            if (detectedCollisions.Count == 1)
            {
                // Top collision
                if (detectedCollisions[0].Bottom < Y)
                {
                    this.AddConnection(flowXCoord, detectedCollisions[0].Bottom, flowXCoord, maxHeight);
                }
                else
                {
                    // Bottom collision
                    this.AddConnection(flowXCoord, 0, flowXCoord, detectedCollisions[0].Y);
                }
            }
            else if (detectedCollisions.Count > 1)
            {
                // Multiple collisions
                double minimum = 0;
                double maximum = maxHeight;
                foreach (var collision in detectedCollisions)
                {
                    if (collision.Bottom < Y && collision.Bottom > minimum)
                    {
                        minimum = collision.Bottom;
                    }
                    else if (collision.Y > Y && collision.Y < maximum)
                    {
                        maximum = collision.Y;
                    }
                }

                this.AddConnection(flowXCoord, minimum, flowXCoord, maximum);
            }
            else
            {
                // No collision
                this.AddConnection(flowXCoord, 0, flowXCoord, maxHeight);
            }
        }

        private void HorizontalCollisionDetection(List<(double X, double Right)> detectedCollisions, double X, double flowYCoord, double maxWidth)
        {
            // Single collision
            if (detectedCollisions.Count == 1)
            {
                // Left collision
                if (detectedCollisions[0].Right < X)
                {
                    this.AddConnection(detectedCollisions[0].Right, flowYCoord, maxWidth, flowYCoord);
                }
                else
                {
                    // Right collision
                    this.AddConnection(0, flowYCoord, detectedCollisions[0].X, flowYCoord);
                }
            }
            else if (detectedCollisions.Count > 1)
            {
                // Multiple collisions
                double minimum = 0;
                double maximum = maxWidth;
                foreach (var collision in detectedCollisions)
                {
                    if (collision.Right < X && collision.Right > minimum)
                    {
                        minimum = collision.Right;
                    }
                    else if (collision.X > X && collision.X < maximum)
                    {
                        maximum = collision.X;
                    }
                }

                this.AddConnection(minimum, flowYCoord, maximum, flowYCoord);
            }
            else
            {
                // No collision
                this.AddConnection(0, flowYCoord, maxWidth, flowYCoord);
            }
        }

        private void AddConnection(double startX, double startY, double endX, double endY)
        {
            this._connections.Add(new Connection(new Point(startX, startY), new Point(endX, endY)));
        }
    }
}
