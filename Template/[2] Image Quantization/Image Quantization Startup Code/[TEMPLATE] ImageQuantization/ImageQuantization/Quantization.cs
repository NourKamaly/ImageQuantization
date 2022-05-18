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
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    color = ImageMatrix[i, j];

                    string Rstring, Gstring, Bstring, hexColor; int intColor;
                    Rstring = color.red.ToString("X");
                    if (Rstring.Length == 1) Rstring = "0" + Rstring;
                    Gstring = color.green.ToString("X");
                    if (Gstring.Length == 1) Gstring = "0" + Gstring;
                    Bstring = color.blue.ToString("X");
                    if (Bstring.Length == 1) Bstring = "0" + Bstring;

                    hexColor = Rstring + Gstring + Bstring;
                    intColor = Convert.ToInt32(hexColor, 16);
                    ColorsConstruction obj = new ColorsConstruction();
                    int colorIndex = MapColor[intColor];
                    int ClusterNumber = Clusters[colorIndex];

                    ImageMatrix[i, j].red = (byte)ClustersColors[ClusterNumber][0];
                    ImageMatrix[i, j].green = (byte)ClustersColors[ClusterNumber][1];
                    ImageMatrix[i, j].blue = (byte)ClustersColors[ClusterNumber][2];


                }
            }

            return ImageMatrix;
        }


    }
}
