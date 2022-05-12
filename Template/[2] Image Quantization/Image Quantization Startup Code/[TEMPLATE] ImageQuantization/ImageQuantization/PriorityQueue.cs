using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageQuantization
{
    public class Vertex
    {
        public double Key { get; set; } = double.MaxValue;
        public int Parent { get; set; } = -1;
        public int V { get; set; }
        public int Color { get; set; }
        public bool IsProcessed { get; set; }
    }
    internal class PriorityQueue<T>
    {
        class Node
        {
            public double Priority { get; set; }
            public T Object { get; set; }
        }

        //object array
        List<Node> queue = new List<Node>();
        int heapSize = -1;
        public int Count { get { return queue.Count; } }

        public void Enqueue(double priority, T obj)
        {
            Node node = new Node() { Priority = priority, Object = obj };
            queue.Add(node);
            heapSize++;
            BuildHeapMin(heapSize);
        }
        public T Dequeue()
        {
            if (heapSize > -1)
            {
                var returnVal = queue[0].Object;
                queue[0] = queue[heapSize];
                queue.RemoveAt(heapSize);
                heapSize--;
                //Maintaining lowest or highest at root based on min or max queue
                MinHeapify(0);
                return returnVal;
            }
            else
                throw new Exception("Queue is empty");
        }

        private void BuildHeapMin(int i)
        {
            while (i >= 0 && queue[(i - 1) / 2].Priority > queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }
        private void MinHeapify(int i)
        {
            int left = i * 2 + 1; ;
            int right = i * 2 + 2;

            int lowest = i;

            if (left <= heapSize && queue[lowest].Priority > queue[left].Priority)
                lowest = left;
            if (right <= heapSize && queue[lowest].Priority > queue[right].Priority)
                lowest = right;

            if (lowest != i)
            {
                Swap(lowest, i);
                MinHeapify(lowest);
            }
        }

        public void UpdatePriority(T obj, double priority)
        {
            int i = 0;
            for (; i <= heapSize; i++)
            {
                Node node = queue[i];
                if (object.ReferenceEquals(node.Object, obj))
                {
                    node.Priority = priority;
                    BuildHeapMin(i);
                    MinHeapify(i);
                }
            }
        }

        public bool IsInQueue(T obj)
        {
            foreach (Node node in queue)
                if (object.ReferenceEquals(node.Object, obj))
                    return true;
            return false;
        }

        private void Swap(int i, int j)
        {
            var temp = queue[i];
            queue[i] = queue[j];
            queue[j] = temp;
        }


    }


}

