namespace OrthogonalConnectorRouting
{
    using System.Windows;

    public enum ConnectorOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }

    public enum ConnectorType
    {
        Routing,
        Dependency
    }

    public enum ReConnectorPosition
    {
        Start,
        End
    }

    public struct ConnectorInfo
    {
        public Rect BoundingBox { get; set; }

        public double DesignerItemLeft { get; set; }

        public double DesignerItemTop { get; set; }

        public Size DesignerItemSize { get; set; }

        public Point Position { get; set; }

        public ConnectorOrientation Orientation { get; set; }

        public ConnectorType ConnectorType { get; set; }
    }
}
