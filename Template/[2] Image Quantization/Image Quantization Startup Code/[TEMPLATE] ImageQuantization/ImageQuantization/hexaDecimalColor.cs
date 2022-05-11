using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    class hexaDecimalColor
    {
        public static List<int> getDistincitColors(RGBPixel[,] ImageMatrix)
        {
            bool[,,] visited_color = new bool[256, 256, 256];

            RGBPixel color;

            List<int> dstinected_color = new List<int>();

            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    color = ImageMatrix[i, j];
                    if (visited_color[color.red, color.green, color.blue] == false)
                    {
                        visited_color[color.red, color.green, color.blue] = true;

                        string left, middle, right, hexColor, back; int coloor;
                        left = color.red.ToString("X");
                        middle = color.green.ToString("X");
                        right = color.blue.ToString("X");
                        hexColor = left + middle + right;
                        coloor = Convert.ToInt32(hexColor, 16);

                        dstinected_color.Add(coloor);
                    }
                }
            }
            return dstinected_color;
        }
    }
}
