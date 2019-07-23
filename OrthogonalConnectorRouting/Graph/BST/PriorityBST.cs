using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OrthogonalConnectorRouting.Graph
{
    public class PriorityBST<N, K> : IPriorityBST<N, K>, IEnumerable<N> where N : INode<K>
    {
        private int _numberOfNodes = 0;
        private PriorityBSTNode _root;

        public List<N> Nodes { get; set; }

        public N Root { get { return this._root.Data; } }

        public int Count { get { return this._numberOfNodes; } }

        public bool IsEmpty { get { return _numberOfNodes == 0 && this._root == null; } }

        public PriorityBST()
        {
            this.Nodes = new List<N>();
        }

        public void BuildTree(IEnumerable<N> nodes)
        {
            if (nodes == null || nodes.Count() == 0)
            {
                return;
            }

            this.Clear();
            this._numberOfNodes = nodes.Count();
            this.Nodes = nodes.ToList();

            var orderedNodes = nodes.OrderBy(node => node.Y).ToList();

            var last = orderedNodes.Last();
            this._root = new PriorityBSTNode()
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

                this.BuildSubTree(orderedNodes.Where(x => x.X >= 0 && x.X <= _root.Border).ToList(), this._root, true);
                this.BuildSubTree(orderedNodes.Where(x => x.X > _root.Border).ToList(), this._root, false);
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
            this.BuildTree(this.Nodes.ToList());
        }

        public List<N> ToList()
        {
            return this.GetEnumerator().ToEnumerable().ToList();
        }

        public void Clear()
        {
            this._numberOfNodes = 0;
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
                parent.Left = new PriorityBSTNode() { Data = last, Parent = parent };
            }
            else
            {
                parent.Right = new PriorityBSTNode() { Data = last, Parent = parent };
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

                    this.BuildSubTree(orderedNodes.Where(x => x.X >= 0 && x.X <= parent.Left.Border).ToList(), parent.Left, true);
                    this.BuildSubTree(orderedNodes.Where(x => x.X > parent.Left.Border).ToList(), parent.Left, false);
                }
                else
                {
                    parent.Right.Border = orderedNodes.ElementAt(median).X;

                    this.BuildSubTree(orderedNodes.Where(x => x.X >= 0 && x.X <= parent.Right.Border), parent.Right, true);
                    this.BuildSubTree(orderedNodes.Where(x => x.X > parent.Right.Border).ToList(), parent.Right, false);
                }
            }
        }

        public List<N> IntervalFind(double x1, double y1, double x2, double y2)
        {
            return IntervalFind(x1, y1, x2, y2, this._root);
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
                nodes.AddRange(IntervalFind(x1, y1, x2, y2, node.Left));
            }

            if (node.Right != null && y1 <= node.Data.Y && x2 > node.Border)
            {
                nodes.AddRange(IntervalFind(x1, y1, x2, y2, node.Right));
            }

            return nodes;
        }

        public N Find(K key)
        {
            return this.Nodes.Where(x => x.Key.Equals(key)).FirstOrDefault();
        }

        public N Find(N node)
        {
            return Find(node.X, node.Y);
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

        public void PrintTree()
        {
            List<List<string>> lines = new List<List<string>>();
            List<PriorityBSTNode> level = new List<PriorityBSTNode>();
            List<PriorityBSTNode> next = new List<PriorityBSTNode>();
            level.Add(this._root);
            int counter = 1;
            int widest = 0;

            while (counter != 0)
            {
                List<string> line = new List<string>();
                counter = 0;

                foreach (PriorityBSTNode element in level)
                {
                    if (element == null)
                    {
                        line.Add(null);
                        next.Add(null);
                        next.Add(null);
                    }
                    else
                    {
                        string value = element.Data.ToString() + "(" + element.Data.X + ", " + element.Data.Y + ") ";
                        line.Add(value);

                        if (value.Length > widest)
                        {
                            widest = value.Length;
                        }

                        next.Add(element.Left);
                        next.Add(element.Right);

                        if (element.Left != null)
                        {
                            counter++;
                        }

                        if (element.Right != null)
                        {
                            counter++;
                        }
                    }
                }

                if (widest % 2 == 1)
                {
                    widest++;
                }

                lines.Add(line);
                List<PriorityBSTNode> tmp = level;
                level = next;
                next = tmp;
                next.Clear();
            }

            int perpiece = lines.ElementAt(lines.Count - 1).Count * (widest + 4);

            for (int i = 0; i < lines.Count; i++)
            {
                List<string> line = lines.ElementAt(i);
                int hpw = (int)Math.Floor(perpiece / 2f) - 1;

                if (i > 0)
                {
                    for (int j = 0; j < line.Count; j++)
                    {
                        char c = ' ';
                        if (j % 2 == 1)
                        {
                            if (line.ElementAt(j - 1) != null)
                            {
                                c = (line.ElementAt(j) != null) ? '┴' : '┘';
                            }
                            else
                            {
                                if (j < line.Count && line.ElementAt(j) != null)
                                {
                                    c = '└';
                                }
                            }
                        }

                        Console.Write(c);

                        if (line.ElementAt(j) == null)
                        {
                            for (int k = 0; k < perpiece - 1; k++)
                            {
                                Console.Write(" ");
                            }
                        }
                        else
                        {
                            for (int k = 0; k < hpw; k++)
                            {
                                Console.Write(j % 2 == 0 ? " " : "-");
                            }

                            Console.Write(j % 2 == 0 ? "┌" : "┐");

                            for (int k = 0; k < (hpw); k++)
                            {
                                Console.Write(j % 2 == 0 ? "-" : " ");
                            }
                        }
                    }

                    Console.WriteLine();
                }

                for (int j = 0; j < line.Count; j++)
                {
                    string value = line.ElementAt(j);

                    if (value == null)
                    {
                        value = "";
                    }

                    int gap1 = (int)Math.Ceiling((perpiece / 2f) - (value.Length / 2f));
                    int gap2 = (int)Math.Floor((perpiece / 2f) - (value.Length / 2f));

                    for (int k = 0; k < gap1; k++)
                    {
                        Console.Write(" ");
                    }

                    Console.Write(value);

                    for (int k = 0; k < gap2; k++)
                    {
                        Console.Write(" ");
                    }
                }

                Console.WriteLine();
                perpiece /= 2;
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
            private PriorityBSTNode current;
            private PriorityBST<N, K> tree;
            private Queue<PriorityBSTNode> traverseQueue;

            public PriorityBTSEnumerator(PriorityBST<N, K> tree)
            {
                this.tree = tree;

                traverseQueue = new Queue<PriorityBSTNode>();
                this.VisitNode(this.tree._root);
            }

            private void VisitNode(PriorityBSTNode node)
            {
                if (node == null)
                {
                    return;
                }

                traverseQueue.Enqueue(node);
                this.VisitNode(node.Left);
                this.VisitNode(node.Right);
            }

            public N Current
            {
                get { return current.Data; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                current = null;
                tree = null;
            }

            public void Reset()
            {
                current = null;
            }

            public bool MoveNext()
            {
                if (traverseQueue.Count > 0)
                {
                    current = traverseQueue.Dequeue();
                }
                else
                {
                    current = null;
                }

                return (current != null);
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
