using OrthogonalConnectorRouting.Algorithm;
using OrthogonalConnectorRouting.Graph;
using OrthogonalConnectorRouting.Graph.ShortestPathAlgorithm;
using OrthogonalConnectorRouting.Models;
using OrthogonalConnectorRouting.Models.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrthogonalConnectorRouting
{
    public class OrthogonalPathFinder : IOrthogonalPathFinder
    {
        private readonly IGraph<Node, Edge, string> _graph;
        private readonly CollisionDetection _collisionDetection;
        private List<Connection> _connections;

        public double Margin { get; set; } = 0;
    
        public OrthogonalPathFinder()
        {
            this._graph = new Graph<Node, Edge, string>();
            this._collisionDetection = new CollisionDetection();
            this._connections = new List<Connection>();
        }

        public void ConstructGraph(IEnumerable<Models.Point> intersections)
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
                var possibleNeighbors = new List<Models.Point>();
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
                Models.Point left, right, top, bottom;
                left = right = top = bottom = null;
                foreach (var neighbor in possibleNeighbors)
                {
                    if (neighbor.X < intersection.X && neighbor.Y == intersection.Y && (left == null || left.X < neighbor.X))
                    {
                        left = neighbor;
                    }
                    else if (neighbor.X > intersection.X && neighbor.Y == intersection.Y && (right == null || right.X > neighbor.X))
                    {
                        right = neighbor;
                    }
                    else if (neighbor.Y < intersection.Y && neighbor.X == intersection.X && (top == null || top.Y < neighbor.Y))
                    {
                        top = neighbor;
                    }
                    else if (neighbor.Y > intersection.Y && neighbor.X == intersection.X && (bottom == null || bottom.Y > neighbor.Y))
                    {
                        bottom = neighbor;
                    }
                }

                // Add vertices and edges to the graph
                this.AddNode(intersection);
                var vertices = new Models.Point[] { left, right, top, bottom };
                foreach (var vertex in vertices)
                {
                    if (vertex != null)
                    {
                        this.AddNode(vertex);
                        this.AddEdge(intersection, vertex);
                    }
                }
            }
        }

        public ShortestGraphPath ShortestPathDijkstra(Node startNode, Node finishNode)
        {
            return GetShortestPathInternal<DijkstraAlgorithm>(startNode, finishNode);
        }

        public ShortestGraphPath ShortestPathAStar(Node startNode, Node finishNode)
        {
            return GetShortestPathInternal<AStarAlgorithm>(startNode, finishNode);
        }

        private ShortestGraphPath GetShortestPathInternal<T>(Node startNode, Node finishNode)
            where T : ISearchAlgorithm, new()
        {
            var (pathNodes, pathEdges) = this._graph.ShortestPath<T>(startNode, finishNode);

            return new ShortestGraphPath
            {
                PathNodes = pathNodes,
                PathEdges = pathEdges
            };
        }

        private void AddEdge(Models.Point sourcePoint, Models.Point destinationPoint)
        {
            var source = this._graph.Find(sourcePoint.X, sourcePoint.Y);
            var dest = this._graph.Find(destinationPoint.X, destinationPoint.Y);
            Edge edge = new Edge
            {
                Source = source,
                Destination = dest,
                Weight = Distance(new Models.Point(sourcePoint.X, sourcePoint.Y), new Models.Point(destinationPoint.X, destinationPoint.Y))
            };

            this._graph.AddEdge(source, dest, edge);
        }

        private void AddNode(Models.Point data)
        {
            Node node = new Node(data.X, data.Y);
            if (this._graph.Find(data.X, data.Y) == null)
            {
                this._graph.AddNode(node);
            }
        }

        public Models.Point FindIntersection(Connection lineA, Connection lineB)
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
                return new Models.Point(x, y);
            }

            return null;
        }

        public IEnumerable<Connection> CreateLeadLines(IEnumerable<IInput> items, double maxWidth, double maxHeight)
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

                var detectedLeftCollisions = this._collisionDetection.DetectCollision(leftCollisions, data);
                this._connections.AddRange(detectedLeftCollisions);

                data.StartX = data.EndX = item.Right + this.Margin;
                var detectedRightCollisions = this._collisionDetection.DetectCollision(rightCollisions, data);
                this._connections.AddRange(detectedRightCollisions);

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

                var detectedVerticalCollisions = this._collisionDetection.ConnectorPointsCollisionDetection(verticalMiddleCollisions, data);
                this._connections.AddRange(detectedVerticalCollisions);

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

                var detectedTopCollisions = this._collisionDetection.DetectCollision(topCollisions, data);
                this._connections.AddRange(detectedTopCollisions);

                data.StartY = data.EndY = item.Bottom + this.Margin;
                var detectedBottomCollisions = this._collisionDetection.DetectCollision(bottomCollisions, data);
                this._connections.AddRange(detectedBottomCollisions);

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

                var detectedHorizontalCollisions = this._collisionDetection.ConnectorPointsCollisionDetection(horizontalMiddleCollisions, data);
                this._connections.AddRange(detectedHorizontalCollisions);
                #endregion
            }

            return this._connections;
        }

        public AlgResults OrthogonalPath(IEnumerable<IInput> items, double maxWidth, double maxHeight, Connector targetConnector)
        {
            var connections = this.CreateLeadLines(items, maxWidth, maxHeight);

            var intersections = new List<Models.Point>();
            foreach (var conn in connections)
            {
                foreach (var conn2 in connections)
                {
                    if (conn == conn2)
                    {
                        continue;
                    }

                    var intersection = this.FindIntersection(conn, conn2);
                    if (intersection != null && !intersections.Contains(intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }

            this.ConstructGraph(intersections.OrderBy(x => x.X).ThenBy(y => y.Y));

            var shortestPath = this.CalculatePathForConnector(targetConnector);

            return new AlgResults
            {
                Connections = connections,
                Intersections = intersections,
                ShortestPath = shortestPath
            };
        }

        public ShortestGraphPath CalculatePathForConnector(Connector targetConnector)
        {
            // TODO: Allow to specify search algorithm
            var shortestPath = this.ShortestPathDijkstra(targetConnector.SourceNode, targetConnector.DestinatonNode);

            foreach (var pathEdge in shortestPath.PathEdges)
            {
                targetConnector.ConnectorPath.Add(new Connection(pathEdge.Source.Position, pathEdge.Destination.Position));
            }

            return shortestPath;
        }

        public ConnectorOrientation CalculateOrientation(IInput item, Models.Point relativeCoords)
        {
            var coords = new List<Models.Point>
            {
                new Models.Point(0, item.Height / 2), // LEFT
                new Models.Point(item.Width / 2, 0),   // TOP
                new Models.Point(item.Width, item.Height / 2),  // RIGHT
                new Models.Point(item.Width / 2, item.Height) // BOTTOM
            };

            var closestPoint = new Models.Point();
            double closestDistanceSquared = double.MaxValue;
            foreach (var point in coords)
            {
                var distanceSquared = Math.Pow(point.X - relativeCoords.X, 2) + Math.Pow(point.Y - relativeCoords.Y, 2);

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestPoint = point;
                }
            }

            var orientation = coords.IndexOf(closestPoint);
            return (ConnectorOrientation)Enum.Parse(typeof(ConnectorOrientation), orientation.ToString());
        }

        private double CalcMedian(double a, double b)
        {
            return (a + b) / 2;
        }

        private bool IsInsideLine(Connection line, double x, double y)
        {
            return (x >= line.Start.X && x <= line.End.X || x >= line.End.X && x <= line.Start.X) &&
                   (y >= line.Start.Y && y <= line.End.Y || y >= line.End.Y && y <= line.Start.Y);
        }

        private double Distance(Models.Point source, Models.Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        }
    }
}
