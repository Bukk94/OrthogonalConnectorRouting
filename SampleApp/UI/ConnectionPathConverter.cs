using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;

namespace SampleApp
{
    [ValueConversion(typeof(List<OrthogonalConnectorRouting.Models.Point>), typeof(PointCollection))]
    public class ConnectionPathConverter : IValueConverter
    {
        static ConnectionPathConverter()
        {
            Instance = new ConnectionPathConverter();
        }

        public static ConnectionPathConverter Instance { get; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pointCollection = new PointCollection();

            if (value is List<OrthogonalConnectorRouting.Models.Point> points)
            {
                foreach (var point in points)
                {
                    pointCollection.Add(new System.Windows.Point(point.X, point.Y));
                }
            }

             return pointCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
