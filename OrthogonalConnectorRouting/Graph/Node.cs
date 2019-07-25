using System;
using System.Windows;

namespace OrthogonalConnectorRouting.Graph
{
    public class Node : INode<string>
    {
        public double X { get; set; }

        public double Y { get; set; }

        public string Key => String.Format("N({0},{1})", this.X, this.Y);

        public Point Position => new Point(this.X, this.Y);

        public Node(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Node))
            {
                return base.Equals(obj);
            }

            var node = (Node)obj;
            return this.X.Equals(node.X) && this.Y.Equals(node.Y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
