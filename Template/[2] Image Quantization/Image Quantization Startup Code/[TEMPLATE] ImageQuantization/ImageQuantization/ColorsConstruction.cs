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
        /// V -> NUMBER OF VERTICIES
        /// E -> NUMBER OF EDGES
        /// D -> NUMBER OF DISTINCT COLORS
        /// N -> LENGTH OF IMAGE-MATRIX  
        
        public static Dictionary<int, int> MapColor;                                                                               //O(1)
        public static List<RGBPixel> getDistincitColors(RGBPixel[,] ImageMatrix)
        {
            int counter = 0;                                                                                                                                    //O(1)
            MapColor = new Dictionary<int, int>();
            //3D Array to mark visited color from the ImageMatrix.
            bool[,,] visited_color = new bool[256, 256, 256];                                                                                                   //O(1)

            RGBPixel color;

            List<RGBPixel> dstinected_color = new List<RGBPixel>();                                                                                             //O(1)

            int Height = ImageMatrix.GetLength(0);                                                                                                              //O(1)
            int Width = ImageMatrix.GetLength(1);                                                                                                               //O(1)

            for (int i = 0; i < Height; i++)                                                                                                                    //O(N)
            {
                for (int j = 0; j < Width; j++)                                                                                                                 //O(N)
                {
                    color = ImageMatrix[i, j];                                                                                                                  //O(1)

                    if (visited_color[color.red, color.green, color.blue] == false)                                                                             //O(1)
                    {
                        visited_color[color.red, color.green, color.blue] = true;                                                                               //O(1)
                        dstinected_color.Add(color);                                                                                                            //O(1)

                        //Conver each RGB-Pixel to hexadecimal. 
                        string Rstring, Gstring, Bstring, hexColor; int intColor;                                                                               //O(1)

                        Rstring = color.red.ToString("X");                                                                                                      //O(1)
                        if (Rstring.Length == 1) Rstring = "0" + Rstring;                                                                                       //O(1)

                        Gstring = color.green.ToString("X");                                                                                                    //O(1)
                        if (Gstring.Length == 1) Gstring = "0" + Gstring;                                                                                       //O(1)

                        Bstring = color.blue.ToString("X");                                                                                                     //O(1)
                        if (Bstring.Length == 1) Bstring = "0" + Bstring;                                                                                       //O(1)

                        //Convert hexadecimal to integer.
                        hexColor = Rstring + Gstring + Bstring;                                                                                                 //O(1)
                        intColor = Convert.ToInt32(hexColor, 16);                                                                                               //O(1)

                        //Map each integer representation of distict color with its index.
                        MapColor.Add(intColor, counter);                                                                                                        //O(1)

                        counter++;                                                                                                                              //O(1)
                    }
                }
            }
            return dstinected_color;                                                                                                                            //O(1)
            //Complixity of function: O(N*N) ====> O(N^2)  Overall.
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// construction Minimum spanning tree.
        /// </summary>
        /// <function name="mininmumSpanningTree">construct Minimum spanning tree</function>
        /// <param name="DistinctColors">Array of distinct RGB pixels</param>
        /// <returns>Array of Struct of MST verticies</returns>
        /// V -> NUMBER OF VERTICIES
        /// E -> NUMBER OF EDGES
        /// D -> NUMBER OF DISTINCT COLORS

        public static double sum_mst = 0;                                                                                                                       //O(1)
        public static Vertex[] mininmumSpanningTree(List<RGBPixel> DistinctColors)
        {

            int vertexCount = DistinctColors.Count;                                                                                                             //O(1)

            Vertex[] vertices = new Vertex[vertexCount];                                                                                                        //O(1)

            //Initialize struct of vertices with (Key -> Max value) - (parent -> -1) - (child -> index).
            for (int i = 0; i < vertexCount; i++)                                                                                                               //O(V)
            {
                vertices[i] = new Vertex() { Key = int.MaxValue, Parent = -1, child = i };                                                                      //O(1)
            }

            //Set random vertix key to zero. 
            vertices[0].Key = 0;                                                                                                                                //O(1)


            double minimumEdge, weight;                                                                                                                         //O(1)
            int current_vertix = 0;                                                                                                                             //O(1)


            while (current_vertix < vertexCount)                                                                                                                //O(V)
            {
                //mark vertix as visited to prevent make cycles.
                vertices[current_vertix].Visited = true;                                                                                                        //O(1)
                minimumEdge = int.MaxValue;                                                                                                                     //O(1)

                //start from the root which its parent = -1.
                int child_vertix = 0;                                                                                                                           //O(1)

                //Summation the MST edges.
                sum_mst += vertices[current_vertix].Key;                                                                                                        //O(1)

                for (int i = 0; i < vertexCount; i++)                                                                                                           //O(V)
                {
                    if (vertices[i].Visited == false)                                                                                                           //O(1)
                    {
                        //Calculate weight between current vertix and others verticies. 
                        RGBPixel Currentvertix = DistinctColors[current_vertix];                                                                                //O(1)
                        RGBPixel Childvertix = DistinctColors[i];                                                                                               //O(1)

                        weight = Math.Sqrt((Currentvertix.red - Childvertix.red) * (Currentvertix.red - Childvertix.red) +
                                           (Currentvertix.blue - Childvertix.blue) * (Currentvertix.blue - Childvertix.blue) +
                                           (Currentvertix.green - Childvertix.green) * (Currentvertix.green - Childvertix.green));                              //O(1)                              

                        //Replace key if weight smaller than the key of child.
                        if (vertices[i].Key > weight)                                                                                                           //O(1)
                        {
                            vertices[i].Key = weight;                                                                                                           //O(1)
                            vertices[i].Parent = current_vertix;                                                                                                //O(1)
                        }

                        //Set minimumEdge if key smaller than the current value. 
                        if (vertices[i].Key < minimumEdge)                                                                                                      //O(1)
                        {
                            minimumEdge = vertices[i].Key;                                                                                                      //O(1)
                            child_vertix = i;                                                                                                                   //O(1)
                        }
                    }
                }

                if (child_vertix == 0)                                                                                                                          //O(1)
                {
                    break;                                                                                                                                      //O(1)
                }

                //Set current vertix index with the child vertix index to find next minimum edge.
                current_vertix = child_vertix;                                                                                                                  //O(1)
            }
            return vertices;                                                                                                                                    //O(1)
        }
        //Complixity of function: O( V + (V*V) ) ====> O(V^2)  Overall
    }
}
