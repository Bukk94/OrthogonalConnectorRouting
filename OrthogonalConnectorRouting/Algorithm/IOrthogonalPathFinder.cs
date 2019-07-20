using System.Collections.Generic;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public interface IOrthogonalPathFinder
    {
        double Margin { get; set; }

        List<Connection> CreateLeadLines(List<DesignerItem> items, double maxWidth, double maxHeight);

        Point? FindIntersection(Connection lineA, Connection lineB);

        void ConstructGraph(List<Point> intersections);
    }
}
