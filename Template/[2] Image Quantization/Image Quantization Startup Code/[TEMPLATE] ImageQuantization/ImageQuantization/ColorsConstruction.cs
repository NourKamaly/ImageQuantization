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
        public bool Visited { get; set; }
    }
    
    
    public struct edges
    {
        public int source { set; get; }
        public int destination { set; get; }
        public double weight { set; get; }
    }


    class ColorsConstruction
    {

        /// <summary>
        /// Detection distinct colors from image.
        /// </summary>
        /// <function name="getDistincitColors">Extract distinct color from the image</function>
        /// <param name="ImageMatrix">Array of RGB pixels</param>
        /// <returns>List of distinct RGB pixels</returns>

        public static Dictionary<int, int> MapColor = new Dictionary<int, int>();
        public static List<RGBPixel> getDistincitColors(RGBPixel[,] ImageMatrix)
        {
            int counter = 0;

            //3D Array to mark visited color from the ImageMatrix.
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

                        //Conver each RGB-Pixel to hexadecimal. 
                        string Rstring, Gstring, Bstring, hexColor; int intColor;
                        
                        Rstring = color.red.ToString("X");
                        if (Rstring.Length == 1) Rstring = "0" + Rstring;
                        
                        Gstring = color.green.ToString("X");
                        if (Gstring.Length == 1) Gstring = "0" + Gstring;
                        
                        Bstring = color.blue.ToString("X");
                        if (Bstring.Length == 1) Bstring = "0" + Bstring;

                        //Convert hexadecimal to integer.
                        hexColor = Rstring + Gstring + Bstring;
                        intColor = Convert.ToInt32(hexColor, 16);

                        //Map each integer representation of distict color with its index.
                        MapColor.Add(intColor, counter);
                        
                        counter++;
                    }
                }
            }
            return dstinected_color;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// construction Minimum spanning tree.
        /// </summary>
        /// <function name="mininmumSpanningTree">construct Minimum spanning tree</function>
        /// <param name="DistinctColors">Array of distinct RGB pixels</param>
        /// <returns>Array of Struct of MST verticies</returns>
        
        public static double sum_mst = 0;
        public static Vertex[] mininmumSpanningTree(List<RGBPixel> DistinctColors)
        {

            int vertexCount = DistinctColors.Count;

            Vertex[] vertices = new Vertex[vertexCount];

            //Initialize struct of vertices with (Key -> Max value) - (parent -> -1) - (child -> index).
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = new Vertex() { Key = int.MaxValue, Parent = -1, child = i };
            }

            //Set random vertix key to zero. 
            vertices[0].Key = 0;


            double minimumEdge, weight;
            int current_vertix = 0;


            while (current_vertix < vertexCount)
            {
                //mark vertix as visited to prevent make cycles.
                vertices[current_vertix].Visited = true;
                minimumEdge = int.MaxValue;
                
                //start from the root which its parent = -1.
                int child_vertix = 0;
                
                //Summation the MST edges.
                sum_mst += vertices[current_vertix].Key;

                for (int i = 0; i < vertexCount; i++)
                {
                    if (vertices[i].Visited == false)
                    {
                        //Calculate weight between current vertix and others verticies. 
                        RGBPixel Currentvertix = DistinctColors[current_vertix], Childvertix = DistinctColors[i];

                        weight = Math.Sqrt((Currentvertix.red - Childvertix.red) * (Currentvertix.red - Childvertix.red) +
                                           (Currentvertix.blue - Childvertix.blue) * (Currentvertix.blue - Childvertix.blue) +
                                           (Currentvertix.green - Childvertix.green) * (Currentvertix.green - Childvertix.green));

                        //Replace key if weight smaller than the key of child.
                        if (vertices[i].Key > weight)
                        {
                            vertices[i].Key = weight; 
                            vertices[i].Parent = current_vertix;
                        }

                        //Set minimumEdge if key smaller than the current value. 
                        if (vertices[i].Key < minimumEdge)
                        {
                            minimumEdge = vertices[i].Key;
                            child_vertix = i;
                        }
                    }
                }

                if (child_vertix == 0)
                {
                    break;
                }

                //Set current vertix index with the child vertix index to find next minimum edge.
                current_vertix = child_vertix;
            }
            return vertices;
        }
    }
}
