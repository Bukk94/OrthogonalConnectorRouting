using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrthogonalConnectorRouting.Graph;
using OrthogonalConnectorRouting.PriorityQueue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthogonalConnectorRouting
{
    [TestClass()]
    public class PriorityQueueTests
    {
        IPriorityQueue<int, double> _priorityQueue;
        public PriorityQueueTests()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
        }

        [TestMethod()]
        public void EnqueueTest()
        {
            var testSize = 10;
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 0; i < testSize; i++)
            {
                this._priorityQueue.Enqueue(i, i);
            }

            Assert.AreEqual(testSize, this._priorityQueue.Count);
        }

        [TestMethod()]
        public void ResizeTest()
        {
            var testSize = 1000;
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 0; i < testSize; i++)
            {
                this._priorityQueue.Enqueue(i, i);
            }

            Assert.AreEqual(testSize, this._priorityQueue.Count);
        }

        [TestMethod()]
        public void DequeueTest()
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            this._priorityQueue = new PriorityQueue<int, double>();
            var data = new List<(int data, double prio)>();
            for (int i = 0; i < 1000; i++)
            {
                var testNode = (data:i, prio:rand.NextDouble());
                data.Add(testNode);
                this._priorityQueue.Enqueue(testNode.data, testNode.prio);
                Assert.AreEqual(i + 1, this._priorityQueue.Count);
            }

            data = data.OrderBy(x => x.prio).ToList();

            foreach (var d in data)
            {
                var deq = this._priorityQueue.Dequeue();
                Assert.AreEqual(d.data, deq);
            }
        }

        [TestMethod()]
        public void PeekTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 1; i < 6; i++)
            {
                this._priorityQueue.Enqueue(i, i);

            }

            Assert.AreEqual(1, this._priorityQueue.Peek());
        }

        [TestMethod()]
        public void IsEmptyTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 0; i < 10; i++)
            {
                this._priorityQueue.Enqueue(i, i);
            }

            for (int i = 0; i < 10; i++)
            {
                this._priorityQueue.Dequeue();
            }

            Assert.AreEqual(true, this._priorityQueue.IsEmpty);
        }

        [TestMethod()]
        public void ClearTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 0; i < 10; i++)
            {
                this._priorityQueue.Enqueue(i, i);
            }

            this._priorityQueue.Clear();

            Assert.IsTrue(this._priorityQueue.IsEmpty);
            Assert.AreEqual(0, this._priorityQueue.Count);
        }

        [TestMethod()]
        public void UpdatePriorityTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            this._priorityQueue.Enqueue(1, 1);
            this._priorityQueue.Enqueue(2, 2);
            this._priorityQueue.Enqueue(3, 3);
            this._priorityQueue.Enqueue(4, 4);

            this._priorityQueue.UpdatePriority(1, 500);
            this._priorityQueue.UpdatePriority(3, 1000);

            Assert.AreEqual(2, this._priorityQueue.Dequeue());
            Assert.AreEqual(4, this._priorityQueue.Dequeue());
            Assert.AreEqual(1, this._priorityQueue.Dequeue());
            Assert.AreEqual(3, this._priorityQueue.Dequeue());
        }

        [TestMethod()]
        public void UpdatePriorityBatchTest()
        {
            var testSize = 1000;
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 0; i < testSize; i++)
            {
                this._priorityQueue.Enqueue(i, i);
            }

            for (int i = 0; i < testSize; i++)
            {
                if (i % 2 == 0)
                {
                    this._priorityQueue.UpdatePriority(i, double.MaxValue - i);
                    Debug.WriteLine(i);
                }
            }

            for (int i = 0; i < testSize / 2; i++)
            {
                if (i % 2 != 0)
                {
                    Assert.AreEqual(i, this._priorityQueue.Dequeue());
                }
            }
        }

        [TestMethod()]
        public void ContainsTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            this._priorityQueue.Enqueue(1, 1);
            this._priorityQueue.Enqueue(2, 2);
            this._priorityQueue.Enqueue(3, 3);

            Assert.IsTrue(this._priorityQueue.Contains(1));
            Assert.IsTrue(this._priorityQueue.Contains(2));
            Assert.IsTrue(this._priorityQueue.Contains(3));
        }

        [TestMethod()]
        public void DoesntContainTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            this._priorityQueue.Enqueue(1, 1);
            this._priorityQueue.Enqueue(2, 2);
            this._priorityQueue.Enqueue(3, 3);

            this._priorityQueue.Dequeue();

            Assert.IsFalse(this._priorityQueue.Contains(1));
        }

        [TestMethod()]
        public void ContainsBatchTest()
        {
            var testSize = 1000;
            this._priorityQueue = new PriorityQueue<int, double>();
            for (int i = 0; i < testSize; i++)
            {
                this._priorityQueue.Enqueue(i, i);
            }

            for (int i = 0; i < testSize; i++)
            {
                Assert.IsTrue(this._priorityQueue.Contains(i));
            }
        }

        [TestMethod()]
        public void NullUpdatePriorityTest()
        {
            var queue = new PriorityQueue<int?, double>();
            queue.Enqueue(1, 1);

            Assert.ThrowsException<NullReferenceException>(() => queue.UpdatePriority(null, 5));
        }

        [TestMethod()]
        public void AddingExistingItemTest()
        {
            this._priorityQueue = new PriorityQueue<int, double>();
            this._priorityQueue.Enqueue(1, 1);

            Assert.ThrowsException<InvalidOperationException>(() => this._priorityQueue.Enqueue(1, 1));
        }
    }
}