using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthogonalConnectorRouting.Graph
{
    public interface IPriorityBST<N, K> where N : INode<K>
    {
        int Count { get; }

        bool IsEmpty { get; }

        N Root { get; }

        List<N> Nodes { get; set; }

        IEnumerator<N> GetEnumerator();

        N Find(double x, double y);

        N Find(N node);

        N Find(K key);

        List<N> IntervalFind(double x1, double y1, double x2, double y2);

        bool Contains(N item);
        
        void Insert(N node);

        void Remove(N node);

        void BuildTree(IEnumerable<N> nodes);

        void PrintTree();

        void Clear();

        List<N> ToList();
    }
}
