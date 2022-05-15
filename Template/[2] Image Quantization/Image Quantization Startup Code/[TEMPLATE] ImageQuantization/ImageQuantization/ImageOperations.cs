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
        public int child { get; set; }
        public int color { get; set; }
        public int V { get; set; }
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
        public static List<int> MapColor = new List<int>();
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
                        MapColor.Add(counter);
                        counter++;
                        visited_color[color.red, color.green, color.blue] = true;
                        dstinected_color.Add(color);
                    }
                }
            }
            return dstinected_color;
        }



        public static double sum_mst = 0;
        public struct edges 
        {
            public  int source { set; get; }
            public  int destination { set; get; }
            public  double weight { set; get; }
        }
        public static edges[] alledges;
        public static Vertex[] MST(List<RGBPixel> DistinctColors)
        {

            int vertexCount = DistinctColors.Count;
            alledges = new edges[vertexCount - 1];
            Vertex[] vertices = new Vertex[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = new Vertex() { Key = 1000000, Parent = -1, V = i };
            }

            vertices[0].Key = 0;

            double minimumEdge, weight;
            int cur = 0;
            int j = 0;
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
                alledges[j] = new edges() { source = cur, destination = child, weight = minimumEdge };
                j++;
               
                cur = child;
            }
            MessageBox.Show(alledges[1].weight.ToString());
            return vertices;
        }

        public static Dictionary<int, int> Clusters;
        public static Dictionary<int, int> getKClusters(Vertex[] MST, int K, List<RGBPixel> DistinctColors)
        {
            PriorityQueue<Vertex> SortedMST = new PriorityQueue<Vertex>(DistinctColors);
            Clusters = new Dictionary<int, int>();
            int ctr;
            for (ctr = 0; ctr < MST.Length; ctr++)
            {

                Clusters.Add(MST[ctr].V, ctr);
                SortedMST.Enqueue(MST[ctr].Key,MST[ctr]);
            }
            //SortedMST.Sort((x, y) => x.Key.CompareTo(y.Key));
            for (ctr = 0; ctr < K; ctr++)
            {
                Vertex SmallestDistance = SortedMST.Dequeue();
                Union(Clusters[SmallestDistance.Parent], Clusters[SmallestDistance.V]);
            }
            return Clusters;
        }
        public static void Union(int ReplaceBy, int Replaced)
        {
            foreach (var cluster in Clusters)
            {
                if (cluster.Value == Replaced)
                {
                    Clusters[cluster.Key] = ReplaceBy;
                }
            }
        }

        public static Dictionary<int, double> getClusterRepresentitive(Dictionary<int, int> Clusters)
        {
            Dictionary <int, double> ClustersColors = new Dictionary <int, double> ();
            Dictionary<int,int> NumOfElementsPerCluster = new Dictionary<int, int> ();  

            foreach(var ClusterNumber in Clusters)
            {
                ClustersColors.Add(ClusterNumber.Value, 0.0);
                NumOfElementsPerCluster.Add(ClusterNumber.Value,0);
            }
            foreach (var Node in Clusters)
            {
                ClustersColors[Node.Value] += Node.Key;
                ClustersColors[Node.Value]++;
            }
            foreach (var cluster in ClustersColors)
            {
                ClustersColors[cluster.Key] /= NumOfElementsPerCluster[cluster.Key];
            }
            return ClustersColors;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------//

    }


}