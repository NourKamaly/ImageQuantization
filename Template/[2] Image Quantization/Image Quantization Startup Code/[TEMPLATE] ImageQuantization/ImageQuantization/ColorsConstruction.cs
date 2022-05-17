using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
    public class Vertex
    {
        public double Key { get; set; } = double.MaxValue;
        public int Parent { get; set; } = -1;
        public int child { get; set; }
        public RGBPixel color { get; set; }
        public int V { get; set; }
        public bool IsProcessed { get; set; }
    }
    public struct edges
    {
        public int source { set; get; }
        public int destination { set; get; }
        public double weight { set; get; }
    }
    class ColorsConstruction
    {
        public static Dictionary<int, int> MapColor = new Dictionary<int, int>();
        public static List<RGBPixel> getDistincitColors(RGBPixel[,] ImageMatrix)
        {
            int counter = 0;
            bool[,,] visited_color = new bool[256, 256, 256];

            RGBPixel color;

            List<RGBPixel> dstinected_color = new List<RGBPixel>();

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
                        dstinected_color.Add(color);

                        string Rstring, Gstring, Bstring, hexColor; int intColor;
                        Rstring = color.red.ToString("X");
                        if (Rstring.Length == 1) Rstring = "0" + Rstring;
                        Gstring = color.green.ToString("X");
                        if (Gstring.Length == 1) Gstring = "0" + Gstring;
                        Bstring = color.blue.ToString("X");
                        if (Bstring.Length == 1) Bstring = "0" + Bstring;

                        hexColor = Rstring + Gstring + Bstring;
                        intColor = Convert.ToInt32(hexColor, 16);

                        MapColor.Add(intColor, counter);
                        counter++;
                    }
                }
            }
            return dstinected_color;
        }


        public static double sum_mst = 0;
        public static Vertex[] MST(List<RGBPixel> DistinctColors)
        {

            int vertexCount = DistinctColors.Count;

            Vertex[] vertices = new Vertex[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = new Vertex() { Key = 1000000, Parent = -1, V = i };
            }

            vertices[0].Key = 0;

            double minimumEdge, weight;
            int cur = 0;
            while (cur < vertexCount)
            {
                vertices[cur].IsProcessed = true;
                minimumEdge = 1000000000;
                int child = 0;
                sum_mst += vertices[cur].Key;

                for (int ch = 0; ch < vertexCount; ch++)
                {
                    if (vertices[ch].IsProcessed == false)
                    {
                        RGBPixel new_one1 = DistinctColors[cur], new_one2 = DistinctColors[ch];
                        weight = Math.Sqrt((new_one1.red - new_one2.red) * (new_one1.red - new_one2.red) + (new_one1.blue - new_one2.blue) * (new_one1.blue - new_one2.blue) + (new_one1.green - new_one2.green) * (new_one1.green - new_one2.green));

                        if (vertices[ch].Key > weight)
                        {
                            vertices[ch].Key = weight; vertices[ch].Parent = cur;
                        }

                        if (vertices[ch].Key < minimumEdge)
                        {
                            minimumEdge = vertices[ch].Key;
                            child = ch;
                        }
                    }
                }
                if (child == 0) break;

                cur = child;
            }
            return vertices;
        }
    }
}
