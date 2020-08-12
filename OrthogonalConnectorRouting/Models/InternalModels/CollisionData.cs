namespace OrthogonalConnectorRouting.Models.InternalModels
{
    internal class CollisionData
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
