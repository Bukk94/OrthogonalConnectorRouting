using OrthogonalConnectorRouting.Models;

namespace SampleApp
{
    public class DesignerItem : IInput
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Right => this.X + this.Width;

        public double Bottom => this.Y + this.Height;

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
