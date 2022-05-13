using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    public class Vertex
    {
        public double Key { get; set; } = double.MaxValue;
        public int Parent { get; set; } = -1;
        public int V { get; set; }
        public int Color { get; set; }
        public bool IsProcessed { get; set; }
    }

    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }






        //---------------------------------------------------------OUR CODE HERE------------------------------------------------------//

        /// <summary>
        /// get the distinected color in a Set of pair 
        /// 1- Counter of the color
        /// 2- The RGB color 
        /// from the rgb pixel array to a Dictionary
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <returns>Dictionary of distinected color and its number </returns>
        /// 

        public static List<RGBPixel> getDistincitColors(RGBPixel[,] ImageMatrix)
        {
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
                    }
                }
            }
            return dstinected_color;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------//

        /// <summary>
        /// get the distance between nodes
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <returns>Dictionary of distinected color and its number </returns>
        /// 
        public static Dictionary<int, List<KeyValuePair<int, double>>> getDistanceBetweenColors(List<RGBPixel> DistinctColor)
        {
            Dictionary<int, List<KeyValuePair<int, double>>> FullyconnectedGraph = new Dictionary<int, List<KeyValuePair<int, double>>>();
            for (int i = 0; i < DistinctColor.Count; i++)
            {
                RGBPixel Current = DistinctColor[i];
                double R = Current.red;
                double G = Current.green;
                double B = Current.blue;

                string RED_1 = Current.red.ToString("X2");
                string GREEN_1 = Current.green.ToString("X2");
                string BLUE_1 = Current.blue.ToString("X2");
                string hexColor_1 = RED_1 + GREEN_1 + BLUE_1;

                int Node_1 = Convert.ToInt32(hexColor_1, 16);

                List<KeyValuePair<int, double>> edges = new List<KeyValuePair<int, double>>();
                for (int j = 0; j < DistinctColor.Count; j++)
                {
                    if (j == i) continue;
                    RGBPixel next = DistinctColor[j];
                    double r = next.red;
                    double g = next.green;
                    double b = next.blue;
                    double result = Math.Sqrt(Math.Pow(R - r, 2) + Math.Pow(G - g, 2) + Math.Pow(B - b, 2));

                    string RED_2 = next.red.ToString("X2");
                    string GREEN_2 = next.green.ToString("X2");
                    string BLUE_2 = next.blue.ToString("X2");
                    string hexColor_2 = RED_2 + GREEN_2 + BLUE_2;

                    int Node_2 = Convert.ToInt32(hexColor_2, 16);

                    edges.Add(new KeyValuePair<int, double>(Node_2, result));

                }
                FullyconnectedGraph.Add(Node_1, edges);
            }
            return FullyconnectedGraph;

        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
        
        public static Dictionary<int, Vertex> MST(Dictionary<int, List<KeyValuePair<int, double>>> graph)
        {
            PriorityQueue<Vertex> queue = new PriorityQueue<Vertex>(graph);
            int vertexCount = graph.Count;
            Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();

            for (int i = 0; i < vertexCount; i++)
            {
                vertices.Add(graph.ElementAt(i).Key, new Vertex() { Key = double.MaxValue, Parent = -1, V = i, Color = graph.ElementAt(i).Key });
                if (i == 0)
                {
                    vertices.ElementAt(0).Value.Key = 0;
                }
                queue.Enqueue(vertices.ElementAt(i).Value.Key, vertices.ElementAt(i).Value);
            }


            while (queue.Count > 0)
            {
                Vertex minVertex = queue.Dequeue();
                int u = minVertex.Color;
                vertices[u].IsProcessed = true;
                //alll edges from vertex u
                List<KeyValuePair<int, double>> edges = graph[minVertex.Color];
                foreach (var Neighbour in edges)
                {
                    if (vertices[Neighbour.Key].Key > 0 && !vertices[Neighbour.Key].IsProcessed && Neighbour.Value < vertices[Neighbour.Key].Key)
                    {
                        vertices[Neighbour.Key].Parent = u;
                        vertices[Neighbour.Key].Key = Neighbour.Value;
                        //updating priority in queue since key is priority
                        queue.UpdatePriority(vertices[Neighbour.Key], vertices[Neighbour.Key].Key);
                    }
                }
            }

            return vertices;
        }
        public static double totalWeight = 0;
        public static double CalculateMST(Dictionary<int, Vertex> MST)
        {

            foreach (var u in MST)
            {
                if (u.Value.Parent >= 0)
                {
                    totalWeight += u.Value.Key;
                }
            }
            return totalWeight;
            //return 1;
        }



        //-----------------------------------------------------------------------------------------------------------------------------------------//

    }


}