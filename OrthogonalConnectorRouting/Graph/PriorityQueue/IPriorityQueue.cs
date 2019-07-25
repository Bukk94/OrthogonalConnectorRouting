namespace OrthogonalConnectorRouting.PriorityQueue
{
    public interface IPriorityQueue<D, P>
    {
        bool IsEmpty { get; }

        int Count { get; }

        void Enqueue(D data, P priority);

        D Dequeue();

        void UpdatePriority(D node, P newPriority);

        bool Contains(D data);

        D Peek();

        void Clear();
    }
}
