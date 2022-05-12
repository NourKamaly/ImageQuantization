//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ImageQuantization
//{
//    class MinHeap
//    {
//        public static Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> ConnectedGraph;
//        public static int numberOfvertices;
//        public static int i = 0;
//        public static [] int parent;
//        public static int heapSize = 0;
//        public struct HeapVertix
//        {
//            public int position;
//            public int key;
//            public int source;
//            public int parent;
//            public bool mstSet;
//            public List<KeyValuePair<int, double>> destinationAndweight;
//        }
//        public static HeapVertix[] heapVertices;
//        public MinHeap(Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> FullyConnectedGraph)
//        {
//            ConnectedGraph = FullyConnectedGraph;
//            numberOfvertices = FullyConnectedGraph.Count;
//            heapSize = numberOfvertices - 1;
//            heapVertices = new HeapVertix[numberOfvertices];
//            foreach (var Vertix in ConnectedGraph)
//            {
//                heapVertices[i].source = Vertix.Key;
//                heapVertices[i].destinationAndweight = Vertix.Value;
//                heapVertices[i].position = i;

//                heapVertices[i].parent.red = -1;
//                heapVertices[i].parent.green = -1;
//                heapVertices[i].parent.blue = -1;
//                heapVertices[i].mstSet = false;
//                heapVertices[i].key = int.MaxValue;
//                i++;

//            }
//            heapVertices[0].key = 0;

//            while (heapSize != 0)
//            {
//                HeapVertix u = extractMin();
//                foreach (var adjecent in u.destinationAndweight)
//                {
//                    if (adjecent.Key)
//                }
//            }
//        }
//        public static void constructMst()
//        {





//        }
//        public static void MinHeapify(int i)
//        {

//            int leftChiled = i * 2 + 1;
//            int rightChild = i * 2 + 2;
//            int lowest = -1;
//            if (leftChiled <= heapSize && heapVertices[i].key > heapVertices[leftChiled].key)
//                lowest = leftChiled;
//            if (rightChild <= heapSize && heapVertices[i].key > heapVertices[rightChild].key)
//                lowest = rightChild;
//            if (lowest != i)
//            {
//                swap(lowest, i);
//                MinHeapify(lowest);
//            }
//        }

//        public static void swap(int i, int j)
//        {
//            var temp = heapVertices[i];
//            heapVertices[i] = heapVertices[j];
//            heapVertices[j] = temp;
//        }

//        public static void buildMinHeap()
//        {
//            for (int i = (heapSize / 2); i >= 0; i--)
//            {
//                MinHeapify(i);
//            }

//        }
//        public static HeapVertix extractMin()
//        {
//            HeapVertix min = heapVertices[0];
//            heapVertices[0] = heapVertices[heapSize];
//            heapSize--;
//            MinHeapify(0);
//            return min;
//        }
//    }
//}
