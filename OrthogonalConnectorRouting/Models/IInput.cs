namespace OrthogonalConnectorRouting.Models
{
    public interface IInput
    {
        double X { get; set; }

        double Y { get; set; }

        double Right { get; }

        double Bottom { get; }

        double Width { get; set; }

        double Height { get; set; }
    }
}
