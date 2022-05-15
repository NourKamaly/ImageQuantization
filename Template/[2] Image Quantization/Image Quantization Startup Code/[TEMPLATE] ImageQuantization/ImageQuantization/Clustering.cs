using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    public class Clustering
    {
        private static Vertex[] MST;
        private static int K;
        private static List<RGBPixel> DistinctColors;
        public Clustering(Vertex[] MinimumSpanningTree, int NumOfClusters, List<RGBPixel> DistinctColorsOfGraph)
        {
            MST = MinimumSpanningTree;
            K = NumOfClusters;
            DistinctColors = DistinctColorsOfGraph;
        }
        public static Dictionary<int, int> getKClusters()
        {
            // SortedMST : priority queue of vertices that sorts them according to the edge weight between source node & destination node
            PriorityQueue<Vertex> SortedMST = new PriorityQueue<Vertex>(DistinctColors);

            // Clusters : key -> distinct color ID, value -> ID of cluster this distinct color belongs to 
            Dictionary<int, int> Clusters = new Dictionary<int, int>();

            int ctr;
            for (ctr = 0; ctr < MST.Length; ctr++)
            {
                Clusters.Add(MST[ctr].V, ctr);
                SortedMST.Enqueue(MST[ctr].Key, MST[ctr]);
            }

            for (ctr = 0; ctr < K; ctr++)
            {
                Vertex SmallestDistance = SortedMST.Dequeue();
                // Node with parent = -1 is the first node of the priority queue, thus it has no parent
                if (SmallestDistance.Parent != -1)
                {
                    Union(Clusters[SmallestDistance.Parent], Clusters[SmallestDistance.V], ref Clusters);
                }
            }
            return Clusters;
        }
        private static void Union(int ReplaceBy, int Replaced, ref Dictionary<int, int> Clusters)
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
            //Cluster : key -> distinct color ID, value -> ID of cluster this distinct color belongs to 
            //Cluster Colors: key -> cluster ID, value -> representitive color in red, green, blue
            //Dinstinct colors : list of RGBPixels that can be accessed by index
            //NumOfElementsPerCluster : key -> cluster ID, value -> number of distinct colors belonging to that cluster

            Dictionary<int, int[]> ClustersColors = new Dictionary<int, int[]>();
            Dictionary<int, int> NumOfElementsPerCluster = new Dictionary<int, int>();
            int ColorID, ClusterID;

            //In this loop we add the ID of each unique cluster, and array of size 3 for red,gree,blue
            foreach (var ClusterNumber in Clusters)
            {
                if (!ClustersColors.ContainsKey(ClusterNumber.Value))
                {
                    ClustersColors.Add(ClusterNumber.Value, new int[3]);
                    NumOfElementsPerCluster.Add(ClusterNumber.Value, 0);
                }
            }

            /* 1. Get the color ID as it is the key in Clusters dictionary
             * 2. Get the cluster ID as is is the value in Clusters dictionary
             * 3. Add the rgb color to the cluster this color belongs to by accessing the rgb information from 
             *    DistinctColors list (color ID is the position of the color in the list)
             * 4. Repeat for all distinct colors
             */
            foreach (var Color in Clusters)
            {
                ColorID = Color.Key;
                ClusterID = Color.Value;
                ClustersColors[ClusterID][0] += DistinctColors[ColorID].red;
                ClustersColors[ClusterID][1] += DistinctColors[ColorID].green;
                ClustersColors[ClusterID][2] += DistinctColors[ColorID].blue;
                NumOfElementsPerCluster[ClusterID]++;
            }

            // Getting the average of all distinct colors belonging to the same cluster
            foreach (var cluster in ClustersColors)
            {
                ClusterID = cluster.Key;
                ClustersColors[ClusterID][0] /= NumOfElementsPerCluster[ClusterID];
                ClustersColors[ClusterID][1] /= NumOfElementsPerCluster[ClusterID];
                ClustersColors[ClusterID][2] /= NumOfElementsPerCluster[ClusterID];
            }

            return ClustersColors;
        }
    }
}

