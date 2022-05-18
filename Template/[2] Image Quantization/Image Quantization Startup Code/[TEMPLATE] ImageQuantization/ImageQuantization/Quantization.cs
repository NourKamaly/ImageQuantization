using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    public class Quantization
    {
        public  RGBPixel[,] Quantize(RGBPixel[,] ImageMatrix, Dictionary<int, int[]> ClustersColors, Dictionary<int, int> Clusters,Dictionary<int,int> MapColor)
        {
            RGBPixel color;
            int Height = ImageMatrix.GetLength(0);                                  //o(1)
            int Width = ImageMatrix.GetLength(1);                                   //o(1)

            for (int i = 0; i < Height; i++)                                         //o(N)
            {
                for (int j = 0; j < Width; j++)                                      //o(N)
                {
                    color = ImageMatrix[i, j];                                       //o(1)

                    string Rstring, Gstring, Bstring, hexColor; int intColor;       //o(1)
                    Rstring = color.red.ToString("X");                              //o(1)
                    if (Rstring.Length == 1) Rstring = "0" + Rstring;               //o(1)
                    Gstring = color.green.ToString("X");                            //o(1)
                    if (Gstring.Length == 1) Gstring = "0" + Gstring;               //o(1)
                    Bstring = color.blue.ToString("X");                             //o(1)
                    if (Bstring.Length == 1) Bstring = "0" + Bstring;               //o(1)

                    hexColor = Rstring + Gstring + Bstring;                         //o(1)
                    intColor = Convert.ToInt32(hexColor, 16);                       //o(1)
                         
                    int colorIndex = MapColor[intColor];                            //o(1)
                    int ClusterNumber = Clusters[colorIndex];                       //o(1)

                    ImageMatrix[i, j].red = (byte)ClustersColors[ClusterNumber][0];     //o(1)
                    ImageMatrix[i, j].green = (byte)ClustersColors[ClusterNumber][1];   //o(1)
                    ImageMatrix[i, j].blue = (byte)ClustersColors[ClusterNumber][2];    //o(1)


                }
            }

            return ImageMatrix;
        }

        //Total Complexity o(N^2)
    }
}
