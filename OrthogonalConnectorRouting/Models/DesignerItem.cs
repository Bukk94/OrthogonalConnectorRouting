using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public class DesignerItem
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Right { get { return this.X + this.Width; } }

        public double Bottom { get { return this.Y + this.Height; } }

        public double Width { get; set; }

        public double Height { get; set; }

        public Rect Rect { get; set; }
    }
}
