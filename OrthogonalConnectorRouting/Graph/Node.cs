using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting.Graph
{
    public class Node : INode<string>
    {
        public double X { get; set; }

        public double Y { get; set; }

        public string Key { get { return String.Format("N({0},{1})", this.X, this.Y); } }

        public Point Position => new Point(this.X, this.Y);

        public Node(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is DesignerItem))
                return base.Equals(obj);

            return this.X.Equals(((DesignerItem)obj).X) && this.Y.Equals(((DesignerItem)obj).Y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
