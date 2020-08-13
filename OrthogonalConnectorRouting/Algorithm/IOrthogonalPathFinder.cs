using OrthogonalConnectorRouting.Enums;
using OrthogonalConnectorRouting.Graph;
using OrthogonalConnectorRouting.Models;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting
{
    public interface IOrthogonalPathFinder
    {
        double Margin { get; set; }

        AlgResults OrthogonalPath(IEnumerable<IInput> items, double maxWidth, double maxHeight, SearchAlgorithm searchAlgorithm, Connector targetConnector);

        #region Algorithm steps
        IEnumerable<Connection> CreateLeadLines(IEnumerable<IInput> items, double maxWidth, double maxHeight);

        Models.Point FindIntersection(Connection lineA, Connection lineB);

        void ConstructGraph(IEnumerable<Models.Point> intersections);

        ShortestGraphPath ShortestPath(Node startNode, Node finishNode, SearchAlgorithm searchAlgorithm);

        ShortestGraphPath CalculatePathForConnector(Connector targetConnector, SearchAlgorithm searchAlgorithm);
        #endregion

        ConnectorOrientation CalculateOrientation(IInput item, Models.Point relativeCoords);
    }
}
