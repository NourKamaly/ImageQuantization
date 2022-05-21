using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
using QuikGraph.Algorithms;
using QuikGraph.Collections;

namespace ImageQuantization
{
    public class Clustering
    {
        // key is the color number, value is the number of cluster it is belonging to
        public  Dictionary<int, int> Clusters;
        //public static PriorityQueue<Vertex> SortedMST;
        public  FibonacciHeap<double, edges> SortedMST;
        public  List<edges> alledges;
        public  Dictionary<int, int> getKClusters(Vertex[] MST, int K, List<RGBPixel> DistinctColors)
        {
            SortedMST = new FibonacciHeap<double, edges>();                                                  //O(1)
            Clusters = new Dictionary<int, int>();                                                           //O(1)
            alledges = new List<edges>(MST.Length);                                                          //O(1)
            int ctr;                                                                                         //O(1)
            for (ctr = 0; ctr < MST.Length; ctr++)                                                           //O(E)
            {
                Clusters.Add(MST[ctr].child, ctr);                                                           //O(1)
                alledges.Add(new edges() { source = MST[ctr].Parent, destination = MST[ctr].child, weight = MST[ctr].Key });  //O(1)
            }
            for (ctr = 0; ctr < alledges.Count; ctr++)                                                       //O(E)
            {
                SortedMST.Enqueue(alledges[ctr].weight, alledges[ctr]);                                      //O(1)
            }
            edges SmallestDistance = SortedMST.Dequeue().Value;                                              //O(log(D))
            for (ctr = 0; ctr < (DistinctColors.Count - K); ctr++)                                           
            {
                SmallestDistance = SortedMST.Dequeue().Value;                                                //O(log(D))
                Union(Clusters[SmallestDistance.source], Clusters[SmallestDistance.destination]);            //O(D) (D is distinct colors)
            }

            return Clusters;
        }
        public  void Union(int ReplaceBy, int Replaced)
        {
            int[] Keys = Clusters.Keys.ToArray();                                                            
            for (int ctr = 0; ctr < Clusters.Count; ctr++)                                                   // O(D)
            {
                if (Clusters[Keys[ctr]] == Replaced)                                                         //O(1)
                {
                    Clusters[Keys[ctr]] = ReplaceBy;                                                         //O(1)
                }
            }
        }

        public  Dictionary<int, int[]> getClusterRepresentitive(Dictionary<int, int> Clusters, List<RGBPixel> DistinctColors)
        {
            Dictionary<int, int[]> ClustersColors = new Dictionary<int, int[]>();         //O(1)
            Dictionary<int, int> NumOfElementsPerCluster = new Dictionary<int, int>();    //O(1)
            //Cluster : key -> color number, value -> cluster number
            //Cluster Colors: key -> cluster number, value -> representitive color in red, green , blue
            //Dinstinct colors : list of RGBPixels that can be accessed by index
            foreach (var ClusterNumber in Clusters)                                       //O(D)
            {
                if (!ClustersColors.ContainsKey(ClusterNumber.Value))                     //O(1)
                {
                    ClustersColors.Add(ClusterNumber.Value, new int[3]);                  //O(1)
                    NumOfElementsPerCluster.Add(ClusterNumber.Value, 0);                  //O(1)
                }
            }
            foreach (var cluster in Clusters)                                             //O(D)
            {
                int ColorNumber = cluster.Key;                                            //O(1)
                ClustersColors[cluster.Value][0] += DistinctColors[ColorNumber].red;      //O(1)
                ClustersColors[cluster.Value][1] += DistinctColors[ColorNumber].green;    //O(1)
                ClustersColors[cluster.Value][2] += DistinctColors[ColorNumber].blue;     //O(1)
                NumOfElementsPerCluster[cluster.Value]++;
            }
            foreach (var cluster in ClustersColors)                                       //O(K)
            {
                ClustersColors[cluster.Key][0] /= NumOfElementsPerCluster[cluster.Key];
                ClustersColors[cluster.Key][1] /= NumOfElementsPerCluster[cluster.Key];
                ClustersColors[cluster.Key][2] /= NumOfElementsPerCluster[cluster.Key];
            }
            //Overall complixity of function: O(D+D+K) ====> O(D)  
            return ClustersColors;
        }
    }
}

