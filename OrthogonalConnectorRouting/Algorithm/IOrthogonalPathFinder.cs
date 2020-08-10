using OrthogonalConnectorRouting.Graph;
using OrthogonalConnectorRouting.Models;
using System.Collections.Generic;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public interface IOrthogonalPathFinder
    {
        double Margin { get; set; }

        List<Connection> CreateLeadLines(List<IInput> items, double maxWidth, double maxHeight);

        Point? FindIntersection(Connection lineA, Connection lineB);

        void ConstructGraph(List<Point> intersections);

        (List<Node> pathNodes, List<Edge> pathEdges) ShortestPath(Node startNode, Node finishNode);
    }
}
