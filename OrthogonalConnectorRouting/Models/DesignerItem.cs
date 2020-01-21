using OrthogonalConnectorRouting.Graph;

namespace OrthogonalConnectorRouting
{
    public class DesignerItem 
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Right => this.X + this.Width;

        public double Bottom => this.Y + this.Height;

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
