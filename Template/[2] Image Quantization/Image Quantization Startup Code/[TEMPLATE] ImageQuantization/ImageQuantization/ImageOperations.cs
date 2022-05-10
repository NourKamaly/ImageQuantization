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






        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// get the distinected color in a Set of pair 
        /// 1- Counter of the color
        /// 2- The RGB color 
        /// from the rgb pixel array to a Dictionary
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <returns>Dictionary of distinected color and its number </returns>
        /// 
        //public static Dictionary<RGBPixel, Boolean> getDistincitColors(RGBPixel[,] ImageMatrix)
        //{
        //    Dictionary<RGBPixel, Boolean> distinct_colors = new Dictionary<RGBPixel, Boolean>();//O(1)
        //    int Height = ImageMatrix.GetLength(0);              //O(1)
        //    int Width = ImageMatrix.GetLength(1);               //O(1)
        //    for(int i = 0; i < Height; i++)                     //O(N)
        //    {
        //        for(int j = 0; j<Width; j++)                    //O(N)
        //        {
        //            if (distinct_colors.Count == 0 || !distinct_colors.ContainsKey(ImageMatrix[i, j]))//O(1) 
        //            {
        //                distinct_colors.Add(ImageMatrix[i, j],true);//O(1)
        //            }
        //        }
        //    }
        //    return distinct_colors;   //Total Function's Complexity = E(N^2)
        //}

        //public static HashSet<RGBPixel> getDistincitColors2(RGBPixel[,] ImageMatrix) 
        //{
        //    int Height = ImageMatrix.GetLength(0);              //O(1)
        //    int Width = ImageMatrix.GetLength(1);               //O(1)
        //    HashSet<RGBPixel> distict_colors = new HashSet<RGBPixel>(); //O(1)
        //    for (int i = 0; i < Height; i++)                    //O(N) 
        //    {
        //        for (int j = 0; j < Width; j++)                 //O(N) 
        //        {
        //            distict_colors.Add(ImageMatrix[i, j]);      //O(N)
        //        }
        //    }
        //    return distict_colors;                            //Total Function's Complexity = E(N^3)
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------//

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
        public static Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> getDistanceBetweenColors(List<RGBPixel> DistinctColor)
        {
            Dictionary<RGBPixel, List<KeyValuePair<RGBPixel,double>>> FullyconnectedGraph = new Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>>();
            for(int i=0;i< DistinctColor.Count;i++)
            {
                RGBPixel Current = DistinctColor[i];
                double R = Current.red;
                double G = Current.green;
                double B = Current.blue;
                List<KeyValuePair<RGBPixel, double>> edges = new List<KeyValuePair<RGBPixel, double>>();
                for (int j = 0; j < DistinctColor.Count; j++)
                {
                    if (j == i) continue;
                    RGBPixel next = DistinctColor[j];
                    double r = next.red;
                    double g = next.green;
                    double b = next.blue;
                    double result = Math.Sqrt(Math.Pow(R - r, 2) + Math.Pow(G - g, 2) + Math.Pow(B - b, 2));

                    edges.Add(new KeyValuePair<RGBPixel, double>(next, result));

                }
                FullyconnectedGraph.Add(Current,edges);
            }
            return FullyconnectedGraph;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//

        struct edge 
        {
            public RGBPixel From, To;
        }
        /*public static Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> getKruksalMinimumSpanningTree(Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> FullyConnectedGraph)
        {
            Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> MST =  new Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>>();
            Dictionary<RGBPixel, int> IndexSet = new Dictionary<RGBPixel, int>();
            int position = 0;
            // Complexity : O(V)
            foreach (var Vertex in FullyConnectedGraph.Keys)
            {
                IndexSet.Add(Vertex, position);
                position++;
            }  
            Dictionary<double,List<edge>> distances = new Dictionary<double, List<edge>>();

            // Complexity : O(V+E)
            foreach (var Node in FullyConnectedGraph)
            {
                foreach (var Neighbor in Node.Value)
                {
                    // Neighbour key is the RGBPixel
                    // Neighbour value is the distance
                    if (!distances.ContainsKey(Neighbor.Value))
                    {
                        distances.Add(Neighbor.Value, new List<edge>());
                    }
                    edge e = new edge();
                    e.From = Node.Key;
                    e.To = Neighbor.Key;
                    distances[Neighbor.Value].Add(e);
                }
            }
            // Sorting by key which is the distance
            //Complexity : O(E)
            var SortedDistances = distances.OrderBy(Dist => Dist.Key);
            int vertices = FullyConnectedGraph.Keys.Count, MSTEdges = 0;
            bool MSTIsComplete = false;
            // MinimumDistance key is distance 
            // MinimumDistance value is list of structs (edges)
            foreach (var MinimumDistance in SortedDistances)
            {
                //Looping over every struct
                foreach (var Neighbour in MinimumDistance.Value)
                {
                    if (MSTEdges == vertices - 1)
                    {
                        MSTIsComplete = true;
                        break;
                    }
                    
                    if (IndexSet[Neighbour.From] == IndexSet[Neighbour.To])
                    {
                        continue;
                    }
                    else
                    {
                        if (!MST.ContainsKey(Neighbour.From))
                        {
                            MST.Add(Neighbour.From, new List<KeyValuePair<RGBPixel, double>>());
                        }
                        AddToMST(MST, Neighbour.From, Neighbour.To, MinimumDistance.Key, ref MSTEdges);
                        Union( IndexSet, IndexSet[Neighbour.From], IndexSet[Neighbour.To]);
                    }
                }
                if (MSTIsComplete)
                {
                    break;
                }
            }
            return MST;
           
        }*/
        //Complexity : O(V)
        public static void Union ( Dictionary<RGBPixel, int> IndexSet, int ReplaceBy, int Replaced)
        {
            for(int i=0;i<IndexSet.Count;i++)
            {
                if(IndexSet.ElementAt(i).Value == Replaced)
                {
                    IndexSet[IndexSet.ElementAt(i).Key] = ReplaceBy;
                }
            }
        }
        //Complexity : O(1)
        public static double MSTSUM= 0;
        public static void AddToMST(Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> MST, RGBPixel From, RGBPixel To, double distance, ref int MSTEdges)
        {
            MSTSUM += distance;
            KeyValuePair<RGBPixel, double> KVP = new KeyValuePair<RGBPixel, double>(To, distance);
            MST[From].Add(KVP);
            MSTEdges++;
        }
        public static Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> getEagerPrimMinimumSpanningTree(Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> FullyConnectedGraph)
        {
            Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>> MST = new Dictionary<RGBPixel, List<KeyValuePair<RGBPixel, double>>>();
            int vertices = FullyConnectedGraph.Keys.Count;
            SortedDictionary<double, KeyValuePair<RGBPixel,RGBPixel>> SortedDistances = new SortedDictionary<double, KeyValuePair<RGBPixel,RGBPixel>>();
            bool[] IsVisited = new bool[vertices];
            bool MSTIsComplete = false;
            int MSTEdges = 0;

            return MST;
        }


        //////////////////////////////////////////////////////////////


    }






}