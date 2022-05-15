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
        public static double mean;
        public static int k;
        public static void initializer(List<edges> alledges) 
        {
            edges = alledges;
            k = 1;
        }
        public static double calculateMean(){
        mean = 0;
        for(int i=0;i<edges.Count;i++)
        {
          mean+=edges[i].weight;
        }
          mean=mean/edges.Count;
          return mean;
        }

        public static double max;
        public static int MaxIndex;
        public static double calculateStandardDeviation()
        {

        mean=calculateMean();
        max = double.MinValue;
        double standardDeviation=0;
        for(int i=0;i<edges.Count;i++)
        {   
          if(edges[i].weight>max)
          {
            max=edges[i].weight;
            MaxIndex=i;    
          }
          standardDeviation+=((edges[i].weight-mean)*(edges[i].weight-mean));
        }
         standardDeviation=Math.Sqrt(standardDeviation/edges.Count-1); 
         return standardDeviation;
       }


        public static int KClustersDetection(){
        double current = calculateStandardDeviation();
        double previous=0;
        while (Math.Abs(current - previous) > 0.0001)
        {
            previous = current;
            edges.RemoveAt(MaxIndex);
            k++;
            current = calculateStandardDeviation();
        }
         return k;
        }
    }
}
