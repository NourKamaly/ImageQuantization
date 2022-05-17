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
        public static Dictionary<int, int> Clusters;
        //public static PriorityQueue<Vertex> SortedMST;
        public static FibonacciHeap<double, edges> SortedMST;
        public static List<edges> alledges;
        public static Dictionary<int, int> getKClusters(Vertex[] MST, int K, List<RGBPixel> DistinctColors)
        {
            SortedMST = new FibonacciHeap<double, edges>();
            Clusters = new Dictionary<int, int>();
            alledges = new List<edges>(MST.Length);
            //int knum = DistinctColors.Count;
            int ctr;
            for (ctr = 0; ctr < MST.Length; ctr++)
            {
                Clusters.Add(MST[ctr].child, ctr);
                alledges.Add(new edges() { source = MST[ctr].Parent, destination = MST[ctr].child, weight = MST[ctr].Key });
            }
            //MessageBox.Show(Clusters.Count.ToString());
            for (ctr = 0; ctr < alledges.Count; ctr++)
            {
                SortedMST.Enqueue(alledges[ctr].weight, alledges[ctr]);
            }
            //SortedMST.Sort((x, y) => x.Key.CompareTo(y.Key));
            edges SmallestDistance = SortedMST.Dequeue().Value;
            for (ctr = 0; ctr < (DistinctColors.Count - K); ctr++)
            {
                SmallestDistance = SortedMST.Dequeue().Value;
                Union(Clusters[SmallestDistance.source], Clusters[SmallestDistance.destination]);
            }
            //Dictionary<int, int> valCount = new Dictionary<int, int>();

            return Clusters;
        }
        public static void Union(int ReplaceBy, int Replaced)
        {
            int[] Keys = Clusters.Keys.ToArray();
            for (int ctr = 0; ctr < Clusters.Count; ctr++)
            {
                if (Clusters[Keys[ctr]] == Replaced)
                {
                    Clusters[Keys[ctr]] = ReplaceBy;
                }
            }
        }

        public static Dictionary<int, int[]> getClusterRepresentitive(Dictionary<int, int> Clusters, List<RGBPixel> DistinctColors)
        {
            Dictionary<int, int[]> ClustersColors = new Dictionary<int, int[]>();
            Dictionary<int, int> NumOfElementsPerCluster = new Dictionary<int, int>();
            //Cluster : key -> color number, value -> cluster number
            // Cluster Colors: key -> cluster number, value -> representitive color in red, green , blue
            //Dinstinct colors : list of RGBPixels that can be accessed by index
            foreach (var ClusterNumber in Clusters)
            {
                if (!ClustersColors.ContainsKey(ClusterNumber.Value))
                {
                    ClustersColors.Add(ClusterNumber.Value, new int[3]);
                    NumOfElementsPerCluster.Add(ClusterNumber.Value, 0);
                }
            }
            foreach (var cluster in Clusters)
            {
                int ColorNumber = cluster.Key;
                ClustersColors[cluster.Value][0] += DistinctColors[ColorNumber].red;
                ClustersColors[cluster.Value][1] += DistinctColors[ColorNumber].green;
                ClustersColors[cluster.Value][2] += DistinctColors[ColorNumber].blue;
                NumOfElementsPerCluster[cluster.Value]++;
            }
            foreach (var cluster in ClustersColors)
            {
                ClustersColors[cluster.Key][0] /= NumOfElementsPerCluster[cluster.Key];
                ClustersColors[cluster.Key][1] /= NumOfElementsPerCluster[cluster.Key];
                ClustersColors[cluster.Key][2] /= NumOfElementsPerCluster[cluster.Key];
            }
            return ClustersColors;
        }
    }
}

