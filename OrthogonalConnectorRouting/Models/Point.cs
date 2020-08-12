namespace OrthogonalConnectorRouting.Models
{
    public class Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Point()
        {
        }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
