using System;
using System.Collections.Generic;

namespace OrthogonalConnectorRouting.PriorityQueue
{
    public class PriorityQueue<D, P> : IPriorityQueue<D, P>
    {
        private const int DefaultCapacity = 20;
        private IComparer<P> _comparer;
        private HeapNode[] _heap;
        private Dictionary<D, HeapNode> _cache;
        private long _nodeIDCounter = 0;

        public bool IsEmpty => this.Count == 0;

        public int Count { get; private set; }

        #region Constructors
        public PriorityQueue() : this(Comparer<P>.Default)
        {
        }

        public PriorityQueue(Comparer<P> cmp)
        {
            this._comparer = cmp;
            this._heap = new HeapNode[DefaultCapacity];
            this.Count = 0;
            this._cache = new Dictionary<D, HeapNode>();
        }
        #endregion

        #region Public methods
        public void Enqueue(D data, P priority)
        {
            if (data == null)
            {
                return;
            }

            if (this._cache.TryGetValue(data, out _))
            {
                // Adding existing node
                throw new InvalidOperationException("Node is already in queue");
            }

            if (this.Count >= this._heap.Length - 1)
            {
                this.ResizeHeap();
            }

            this.Count++;
            var node = new HeapNode(this._nodeIDCounter++) { Data = data, Priority = priority, PositionInQueue = Count };
            this._cache[data] = node;
            this._heap[this.Count] = node;

            this.HeapifyUp(node);
        }

        public D Dequeue()
        {
            if (this.IsEmpty)
            {
                return default(D);
            }
            else if (this.Count == 1)
            {
                var tmp = this._heap[1].Data;
                this._heap[1] = null;
                this.Count = 0;
                return tmp;
            }

            if (this.Count < (this._heap.Length / 2))
            {
                this.ShrinkHeap();
            }

            var maxPrio = this._heap[1];
            var lastNode = this._heap[this.Count];
            this.Switch(1, this.Count);
            lastNode.PositionInQueue = 1;
            this._heap[this.Count] = null;
            this.Count--;

            this.HeapifyDown(lastNode);

            return maxPrio.Data;
        }

        public void UpdatePriority(D inputNode, P newPriority)
        {
            var node = this._cache[inputNode];
            node.Priority = newPriority;

            int parentIndex = node.PositionInQueue >> 1;
            if (parentIndex > 0 && this.IsHigherPriority(node, _heap[parentIndex]))
            {
                this.HeapifyUp(node);
            }
            else
            {
                this.HeapifyDown(node);
            }
        }

        public bool Contains(D data)
        {
            return this._cache.TryGetValue(data, out _);
        }

        public D Peek()
        {
            return this.IsEmpty ? default(D) : this._heap[1].Data;
        }

        public void Clear()
        {
            this.Count = 0;
            this._heap = new HeapNode[DefaultCapacity];
            this._cache.Clear();
        }
        #endregion

        private void HeapifyUp(HeapNode node)
        {
            if (node.PositionInQueue < 1)
            {
                return;
            }

            int parent = node.PositionInQueue >> 1;
            if (node.PositionInQueue > 1)
            {
                parent = node.PositionInQueue >> 1;
                if (this.IsHigherPriority(this._heap[parent], node))
                {
                    return;
                }

                this.Switch(parent, node);
            }

            while (parent > 1)
            {
                parent = parent / 2;
                if (this.IsHigherPriority(this._heap[parent], node))
                {
                    break;
                }

                this.Switch(parent, node);
            }

            this._heap[node.PositionInQueue] = node;
        }

        private void HeapifyDown(HeapNode node)
        {
            int lastIndex = node.PositionInQueue;
            int leftChildIdx = 2 * lastIndex;

            // If leaf, do nothing
            if (this.IsLeaf(leftChildIdx))
            {
                return;
            }

            int rightChildIdx = leftChildIdx + 1;
            var leftChild = this._heap[leftChildIdx];
            var rightChild = this._heap[rightChildIdx];

            // Check if the left-child has higher priority than the current node
            if (this.IsHigherPriority(leftChild, node))
            {
                // If right child is leaf, swap and finish
                if (this.IsLeaf(rightChildIdx))
                {
                    node.PositionInQueue = leftChildIdx;
                    leftChild.PositionInQueue = lastIndex;
                    this._heap[lastIndex] = leftChild;
                    this._heap[leftChildIdx] = node;

                    return;
                }

                // Check if the left-child has higher priority than the right-child
                if (this.IsHigherPriority(leftChild, rightChild))
                {
                    this.MoveNode(leftChild, leftChildIdx, lastIndex);
                }
                else
                {
                    this.MoveNode(rightChild, rightChildIdx, lastIndex);
                }
            }
            else if (this.IsLeaf(rightChildIdx))
            {
                // Return if right child doesnt exist or is leaf
                return;
            }
            else
            {
                // Left child is fine and node is not leaf, swap right child
                if (this.IsHigherPriority(rightChild, node))
                {
                    this.MoveNode(rightChild, rightChildIdx, lastIndex);
                }
                else
                {
                    // Everything is sorted
                    return;
                }
            }

            while (true)
            {
                leftChildIdx = 2 * lastIndex;

                // Finish if node is leaf
                if (this.IsLeaf(leftChildIdx))
                {
                    node.PositionInQueue = lastIndex;
                    this._heap[lastIndex] = node;
                    break;
                }

                // Check if the left-child is higher-priority than the current node
                rightChildIdx = leftChildIdx + 1;
                leftChild = this._heap[leftChildIdx];
                rightChild = this._heap[rightChildIdx];

                if (this.IsHigherPriority(leftChild, node))
                {

                    // Check if there is a right child. If not, swap and finish.
                    if (this.IsLeaf(rightChildIdx))
                    {
                        node.PositionInQueue = leftChildIdx;
                        leftChild.PositionInQueue = lastIndex;
                        this._heap[lastIndex] = leftChild;
                        this._heap[leftChildIdx] = node;

                        break;
                    }
                    // Check if the left-child is higher-priority than the right-child
                    
                    if (this.IsHigherPriority(leftChild, rightChild))
                    {
                        // left is highest, move it up and continue
                        leftChild.PositionInQueue = lastIndex;
                        this._heap[lastIndex] = leftChild;
                        lastIndex = leftChildIdx;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        rightChild.PositionInQueue = lastIndex;
                        this._heap[lastIndex] = rightChild;
                        lastIndex = rightChildIdx;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if (this.IsLeaf(rightChildIdx))
                {
                    node.PositionInQueue = lastIndex;
                    this._heap[lastIndex] = node;
                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    if (this.IsHigherPriority(rightChild, node))
                    {
                        rightChild.PositionInQueue = lastIndex;
                        this._heap[lastIndex] = rightChild;
                        lastIndex = rightChildIdx;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        node.PositionInQueue = lastIndex;
                        this._heap[lastIndex] = node;
                        break;
                    }
                }
            }
        }

        private void MoveNode(HeapNode node, int childIndex, int lastIndex)
        {
            node.PositionInQueue = lastIndex;
            this._heap[lastIndex] = node;
            lastIndex = childIndex;
        }

        private bool IsLeaf(int index)
        {
            return index > this.Count;
        }

        private bool IsHigherPriority(HeapNode higher, HeapNode lower)
        {
            var result = this._comparer.Compare(higher.Priority, lower.Priority);
            return (result < 0 || (result == 0 && higher.ID < lower.ID));
        }

        private void ResizeHeap()
        {
            var newArray = new HeapNode[this._heap.Length * 2];
            Array.Copy(this._heap, newArray, this.Count + 1);
            this._heap = newArray;
        }

        private void ShrinkHeap()
        {
            var newArray = new HeapNode[this._heap.Length / 2];
            Array.Copy(this._heap, newArray, this.Count + 1);
            this._heap = newArray;
        }

        private void Switch(int firstIdx, int secondIdx)
        {
            var tmp = this._heap[firstIdx];
            this._heap[firstIdx] = this._heap[secondIdx];
            this._heap[secondIdx] = tmp;
        }

        private void Switch(int parent, HeapNode node)
        {
            var parentNode = this._heap[parent];
            this._heap[node.PositionInQueue] = parentNode;
            parentNode.PositionInQueue = node.PositionInQueue;
            node.PositionInQueue = parent;
        }

        internal class HeapNode
        {
            public HeapNode(long id)
            {
                this.ID = id;
            }

            public long ID { get; private set; }

            public D Data { get; set; }

            public P Priority { get; set; }

            public int PositionInQueue { get; set; }
        }
    }
}
