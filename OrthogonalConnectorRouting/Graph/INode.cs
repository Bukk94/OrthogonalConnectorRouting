using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthogonalConnectorRouting.Graph
{
    public interface INode<K>
    {
        double X { get; set; }

        double Y { get; set; }

        K Key { get; }
    }
}
