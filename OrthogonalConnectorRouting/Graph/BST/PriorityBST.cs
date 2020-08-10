using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OrthogonalConnectorRouting.Graph
{
    public class PriorityBST<N, K> : IPriorityBST<N, K>, IEnumerable<N> where N : INode<K>
    {
        private PriorityBSTNode _root;

        public N Root => this._root.Data;

        public List<N> Nodes { get; set; }

        public int Count { get; private set; }

        public bool IsEmpty => this.Count == 0 && this._root == null;

        public PriorityBST()
        {
            this.Count = 0;
            this.Nodes = new List<N>();
        }

        public void BuildTree(IEnumerable<N> nodes)
        {
            if (nodes == null || nodes.Count() == 0)
            {
                return;
            }

            this.Clear();
            this.Count = nodes.Count();
            this.Nodes = nodes.ToList();

            var orderedNodes = nodes.OrderBy(node => node.Y).ToList();

            var last = orderedNodes.Last();
            this._root = new PriorityBSTNode
            {
                Data = last
            };

            orderedNodes.Remove(last);

            if (orderedNodes.Count == 0)
            {
                _root.Border = _root.Data.X;
            }
            else
            {
                orderedNodes = orderedNodes.OrderBy(node => node.X).ToList();
                int median = (orderedNodes.Count()) / 2;
                _root.Border = orderedNodes.ElementAt(median).X;

                this.BuildSubTree(orderedNodes.Where(x => x.X >= 0 && x.X <= _root.Border), this._root, true);
                this.BuildSubTree(orderedNodes.Where(x => x.X > _root.Border), this._root, false);
            }
        }

        public void Insert(N node)
        {
            if (node == null)
            {
                return;
            }

            this.Nodes.Add(node);
            this.BuildTree(this.Nodes.ToList());
        }

        public void Remove(N node)
        {
            if (node == null)
            {
                return;
            }

            this.Nodes.Remove(node);
            this.BuildTree(this.Nodes);
        }

        public List<N> ToList()
        {
            return this.GetEnumerator().ToEnumerable().ToList();
        }

        public void Clear()
        {
            this.Count = 0;
            this._root = null;
            this.Nodes.Clear();
        }

        public bool Contains(N item)
        {
            return Find(item) != null;
        }

        private void BuildSubTree(IEnumerable<N> nodes, PriorityBSTNode parent, bool isLeft)
        {
            if (nodes.Count() == 0)
            {
                return;
            }

            var orderedNodes = nodes.OrderBy(node => node.Y).ToList();

            var last = orderedNodes.Last();
            if (isLeft)
            {
                parent.Left = new PriorityBSTNode { Data = last, Parent = parent };
            }
            else
            {
                parent.Right = new PriorityBSTNode { Data = last, Parent = parent };
            }

            orderedNodes.Remove(last);

            if (orderedNodes.Count == 0)
            {
                if (isLeft)
                {
                    parent.Left.Border = parent.Left.Data.X;
                }
                else
                {
                    parent.Right.Border = parent.Right.Data.X;
                }
            }
            else
            {
                orderedNodes = orderedNodes.OrderBy(node => node.X).ToList();
                int median = (orderedNodes.Count()) / 2;

                if (isLeft)
                {
                    parent.Left.Border = orderedNodes.ElementAt(median).X;

                    this.BuildSubTree(orderedNodes.Where(x => x.X >= 0 && x.X <= parent.Left.Border), parent.Left, true);
                    this.BuildSubTree(orderedNodes.Where(x => x.X > parent.Left.Border), parent.Left, false);
                }
                else
                {
                    parent.Right.Border = orderedNodes.ElementAt(median).X;

                    this.BuildSubTree(orderedNodes.Where(x => x.X >= 0 && x.X <= parent.Right.Border), parent.Right, true);
                    this.BuildSubTree(orderedNodes.Where(x => x.X > parent.Right.Border), parent.Right, false);
                }
            }
        }

        public List<N> IntervalFind(double x1, double y1, double x2, double y2)
        {
            return this.IntervalFind(x1, y1, x2, y2, this._root);
        }

        private List<N> IntervalFind(double x1, double y1, double x2, double y2, PriorityBSTNode node)
        {
            List<N> nodes = new List<N>();
            if (node == null)
            {
                return nodes;
            }

            if (node.Data.X >= x1 && node.Data.X <= x2 && node.Data.Y >= y1 && node.Data.Y <= y2)
            {
                nodes.Add(node.Data);
            }

            if (node.Left != null && y1 <= node.Data.Y && x1 <= node.Border)
            {
                nodes.AddRange(this.IntervalFind(x1, y1, x2, y2, node.Left));
            }

            if (node.Right != null && y1 <= node.Data.Y && x2 > node.Border)
            {
                nodes.AddRange(this.IntervalFind(x1, y1, x2, y2, node.Right));
            }

            return nodes;
        }

        public N Find(K key)
        {
            return this.Nodes.FirstOrDefault(x => x.Key.Equals(key));
        }

        public N Find(N node)
        {
            return this.Find(node.X, node.Y);
        }

        public N Find(double x, double y)
        {
            var current = this._root;
            while (true)
            {
                if (current == null)
                {
                    return default(N);
                }

                if (current.Data.X == x && current.Data.Y == y)
                {
                    return current.Data;
                }

                if (current.Data.Y < y)
                {
                    return default(N);
                }

                if (current.Border >= x)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }
        }

        public IEnumerator<N> GetEnumerator()
        {
            var enumerator = new PriorityBTSEnumerator(this);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PriorityBTSEnumerator(this);
        }

        #region Private classes
        private class PriorityBTSEnumerator : IEnumerator<N>
        {
            private PriorityBSTNode _current;
            private PriorityBST<N, K> _tree;
            private Queue<PriorityBSTNode> _traverseQueue;

            public PriorityBTSEnumerator(PriorityBST<N, K> tree)
            {
                this._tree = tree;

                this._traverseQueue = new Queue<PriorityBSTNode>();
                this.VisitNode(this._tree._root);
            }

            private void VisitNode(PriorityBSTNode node)
            {
                if (node == null)
                {
                    return;
                }

                this._traverseQueue.Enqueue(node);
                this.VisitNode(node.Left);
                this.VisitNode(node.Right);
            }

            public N Current => this._current.Data;

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                this._current = null;
                this._tree = null;
            }

            public void Reset()
            {
                this._current = null;
            }

            public bool MoveNext()
            {
                if (this._traverseQueue.Count > 0)
                {
                    this._current = this._traverseQueue.Dequeue();
                }
                else
                {
                    this._current = null;
                }

                return this._current != null;
            }
        }

        private class PriorityBSTNode
        {
            public N Data { get; set; }

            public double Border { get; set; }

            public PriorityBSTNode Parent { get; set; }

            public PriorityBSTNode Left { get; set; }

            public PriorityBSTNode Right { get; set; }
        }
        #endregion
    }
}
