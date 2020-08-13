using OrthogonalConnectorRouting.Graph;
using OrthogonalConnectorRouting.Models;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting
{
    public interface IOrthogonalPathFinder
    {
        double Margin { get; set; }

        AlgResults OrthogonalPath(IEnumerable<IInput> items, double maxWidth, double maxHeight, Connector targetConnector);

        #region Algorithm steps
        IEnumerable<Connection> CreateLeadLines(IEnumerable<IInput> items, double maxWidth, double maxHeight);

        Models.Point FindIntersection(Connection lineA, Connection lineB);

        void ConstructGraph(IEnumerable<Models.Point> intersections);

        ShortestGraphPath ShortestPathDijkstra(Node startNode, Node finishNode);
        ShortestGraphPath ShortestPathAStar(Node startNode, Node finishNode);

        ShortestGraphPath CalculatePathForConnector(Connector targetConnector);
        #endregion

        ConnectorOrientation CalculateOrientation(IInput item, Models.Point relativeCoords);
    }
}
