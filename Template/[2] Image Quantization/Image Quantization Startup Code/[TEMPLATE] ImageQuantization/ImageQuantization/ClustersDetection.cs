using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    class ClustersDetection
    {
        public  List<edges> edges;
        public  double mean = 0;
        public  double standardDeviation = 0;
        public  double max = double.MinValue;
        public  int MaxIndex;
        public  int k;
        public  double previous = 0;

        /// <summary>
        /// Detection Number of clusters Automataticaly. 
        /// </summary>
        /// <function name="calculateMean">calculate mean of all edges</function>
        /// <function name="calculateStandardDeviation">calculate standard division of all edges</function>
        /// <function name="KClustersDetection">Calculate number of K</function>
        /// <param    name="alledges"> All edges of MST</param>
        /// <returns>Number of detected clusters</returns>

        public  void initializer(List<edges> alledges)
        {
            edges = alledges;                                                   //O(1)
            k = 0;                                                                         //O(1)
        }
        public  void calculateMean()
        {

            double sum = 0;                                                                //O(1)

            for (int i = 0; i < edges.Count; i++)                                          //O(E)
            {
                sum += edges[i].weight;                                                    //O(1)
            }

            mean = sum / edges.Count;                                                      //O(1)
            //Complixity of function: O(E+1) ====> O(E)  Overall.
        }

        public  void calculateStandardDeviation()
        {
            double sum = 0;                                                                //O(1)
            for (int i = 0; i < edges.Count; i++)                                          //O(E)
            {
                if ((edges[i].weight - mean) * (edges[i].weight - mean) > max)             //O(1)
                {
                    max = (edges[i].weight - mean) * (edges[i].weight - mean);             //O(1)
                    MaxIndex = i;                                                          //O(1)
                }

                sum += ((edges[i].weight - mean) * (edges[i].weight - mean));              //O(1)
            }

            max = double.MinValue;                                                         //O(1)
            standardDeviation = sum / (edges.Count - 1);                                   //O(1)
            standardDeviation = Math.Sqrt(standardDeviation);                              //O(1)
            //Complixity of function: O(E + 1) ====> O(E)  Overall.
        }

        public  int KClustersDetection()
        {

            calculateMean();                                                               //O(E)
            calculateStandardDeviation();                                                  //O(E)

            while (Math.Abs(standardDeviation - previous) >= 0.0001)                        //O(E)                       
            {
                edges.RemoveAt(MaxIndex);                                                  //O(1)
                previous = standardDeviation;                                              //O(1)
                calculateMean();                                                           //O(E)
                calculateStandardDeviation();                                              //O(E)
                k++;                                                                       //O(1)
            }
            return k;                                                                      //O(1)
            //Complixity of function: O( E + E + E * ( E + E ) ) ====> O(E^2)  Overall.
        }

    }
}
