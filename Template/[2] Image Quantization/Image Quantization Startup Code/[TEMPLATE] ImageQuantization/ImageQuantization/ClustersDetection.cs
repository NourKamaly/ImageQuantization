using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    class ClustersDetection
    {
        public static List<edges> edges;
        public static double mean = 0;
        public static double standardDeviation = 0;
        public static double max = double.MinValue;
        public static int MaxIndex;
        public static int k;
        public static double previous = 0;

        /// <summary>
        /// Detection Number of clusters Automataticaly. 
        /// </summary>
        /// <function name="calculateMean">calculate mean of all edges</function>
        /// <function name="calculateStandardDeviation">calculate standard division of all edges</function>
        /// <function name="KClustersDetection">Calculate number of K</function>
        /// <param    name="alledges"> All edges of MST</param>
        /// <returns>Number of detected clusters</returns>

        public static void initializer(List<edges> alledges)
        {
            edges = Clustering.alledges;
            k = 0;
        }
        public static void calculateMean()
        {

            double sum = 0;

            for (int i = 0; i < edges.Count; i++)
            {
                sum += edges[i].weight;
            }

            mean = sum / edges.Count;
        }

        public static void calculateStandardDeviation()
        {
            double sum = 0;
            for (int i = 0; i < edges.Count; i++)
            {
                if ((edges[i].weight - mean) * (edges[i].weight - mean) > max)
                {
                    max = (edges[i].weight - mean) * (edges[i].weight - mean);
                    MaxIndex = i;
                }

                sum += ((edges[i].weight - mean) * (edges[i].weight - mean));
            }

            max = double.MinValue;
            standardDeviation = sum / (edges.Count - 1);
            standardDeviation = Math.Sqrt(standardDeviation);
        }

        public static int KClustersDetection()
        {

            calculateMean();
            calculateStandardDeviation();
            
            while (Math.Abs(standardDeviation - previous) > 0.0001)
            {
                edges.RemoveAt(MaxIndex);
                previous = standardDeviation;
                calculateMean();
                calculateStandardDeviation();
                k++;
            }
            return k;
        }


    }
}
