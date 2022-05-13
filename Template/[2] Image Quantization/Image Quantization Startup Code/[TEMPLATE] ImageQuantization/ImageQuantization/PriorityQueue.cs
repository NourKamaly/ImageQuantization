using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;

namespace ImageQuantization
{
    //public class Vertex
    //{
    //    public double Key { get; set; } = double.MaxValue;
    //    public int Parent { get; set; } = -1;
    //    public int V { get; set; }
    //    public int Color { get; set; }
    //    public bool IsProcessed { get; set; }
    //}
    internal class PriorityQueue<T>
    {
        class Node
        {
            public double Priority { get; set; }
            public Vertex Object { get; set; }
        }

        //object array
        List<Node> queue = new List<Node>();
        int heapSize = -1;
        public PriorityQueue(double[,] fullyconnectedgraph) 
        {
            int noOfVertexes = fullyconnectedgraph.GetLength(0);
            indexes = new int[noOfVertexes];
        }
        public int[] indexes;
        public int Count { get { return queue.Count; } }

        public void Enqueue(double priority, Vertex obj)
        {
            Node node = new Node() { Priority = priority, Object = obj };
            queue.Add(node);
            heapSize++;
            indexes[obj.V] = heapSize;
            //index[1]=5
            BuildHeapMin(heapSize);
        }
        private void BuildHeapMin(int i)
        {
            //for (int i = (heapSize - 1 / 2); i >= 0; i--)
            //{
            //    MinHeapify(i);
            //}

            while (i >= 0 && queue[(i - 1) / 2].Priority > queue[i].Priority)
            {

                int child = queue[i].Object.V;
                int parent = queue[(i - 1) / 2].Object.V;
                indexes[child] = (i - 1) / 2;
                indexes[parent] = i;

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

                int child = queue[lowest].Object.V;
                int parent = queue[i].Object.V;
                indexes[child] = i;
                indexes[parent] = lowest;
                Swap(lowest, i);
                MinHeapify(lowest);
            }
        }
        public Vertex Dequeue()
        {
            if (heapSize > -1)
            {
                var returnVal = queue[0].Object;
                queue[0] = queue[heapSize];
                indexes[queue[heapSize].Object.V] = 0;
                queue.RemoveAt(heapSize);
                heapSize--;
                //Maintaining lowest or highest at root based on min or max queue
                MinHeapify(0);
                return returnVal;
            }
            else
                throw new Exception("Queue is empty");
        }


 

        public void UpdatePriority(Vertex obj, double priority)
        {
                int realInd = indexes[obj.V];
                Node node = queue[realInd];
                node.Priority = priority;
                BuildHeapMin(realInd);
                //MinHeapify(realInd);
        }

        //public bool IsInQueue(Vertex obj)
        //{
        //    foreach (Node node in queue)
        //        if (object.ReferenceEquals(node.Object, obj))
        //            return true;
        //    return false;
        //}

        private void Swap(int i, int j)
        {
            var temp = queue[i];
            queue[i] = queue[j];
            queue[j] = temp;
        }


    }


}
