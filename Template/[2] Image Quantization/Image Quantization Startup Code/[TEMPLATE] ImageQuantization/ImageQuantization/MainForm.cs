using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public System.Timers.Timer timer;
        public MainForm()
        {
            InitializeComponent();
            
        }
        RGBPixel[,] ImageMatrix;
        RGBPixel[,] InputImageMatrix;
        public static string openfilepath;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            //setting text boxs as empty
            textBox1.Text = "...";
            textBox2.Text = "...";
            textBox4.Text = "...";
            msttime.Text = "00:00:00";
            ditincttime.Text = "00:00:00";
            detectedtime.Text = "00:00:00";
            quantizationtime.Text = "00:00:00";
            textBox3.Text = "00:00:00";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                openfilepath = OpenedFilePath;
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
           // pictureBox2.Image.Save("C:\\Users\\Norhan\\Desktop\\[2] Image Quantization\\Testcases\\Testcases\\Complete\\Complete Test\\Complete Test\\Large\\Filtered.jpg");
        }
        private void button1_Click(object sender, EventArgs e)
        {

            InputImageMatrix = ImageOperations.OpenImage(openfilepath);
            Stopwatch overallTime = new Stopwatch();
            overallTime.Start();

            ColorsConstruction colorsconstruction = new ColorsConstruction();
            Clustering clusterting = new Clustering();
            Quantization quantization = new Quantization();

            //distinct colors
            Stopwatch distinctcolorstopwathc = new Stopwatch();
            distinctcolorstopwathc.Start();
            List<RGBPixel> distinctcolors = colorsconstruction.getDistincitColors(InputImageMatrix);
            textBox2.Text = distinctcolors.Count.ToString();
            distinctcolorstopwathc.Stop();
            TimeSpan dcsw = distinctcolorstopwathc.Elapsed;
            ditincttime.Text = dcsw.Minutes + ":" + dcsw.Seconds + ":" + dcsw.Milliseconds;

            //mst
            Stopwatch mststopwathctime = new Stopwatch();
            mststopwathctime.Start();
            Vertex[] mst = colorsconstruction.mininmumSpanningTree(distinctcolors);
            textBox1.Text = colorsconstruction.sum_mst.ToString();
            mststopwathctime.Stop();
            TimeSpan msts = mststopwathctime.Elapsed;
            msttime.Text = msts.Minutes + ":" + msts.Seconds + ":" + msts.Milliseconds;

            //quantization
            Stopwatch quantizationstopwatch = new Stopwatch();
            quantizationstopwatch.Start();
            Dictionary<int, int> kclusters = clusterting.getKClusters(mst, Int32.Parse(numberofclusters.Text), distinctcolors);
            Dictionary<int, int[]> representitiveColors = clusterting.getClusterRepresentitive(kclusters, distinctcolors);
            RGBPixel[,] QuantizedMatrix = quantization.Quantize(InputImageMatrix, representitiveColors, kclusters, colorsconstruction.MapColor);
            ImageOperations.DisplayImage(QuantizedMatrix, pictureBox2);
            quantizationstopwatch.Stop();
            TimeSpan qusw = quantizationstopwatch.Elapsed;
            quantizationtime.Text = qusw.Minutes + ":" + qusw.Seconds + ":" + qusw.Milliseconds;

            //clustering
            Stopwatch clusteringdetectionstopwatch = new Stopwatch();
            clusteringdetectionstopwatch.Start();
            ClustersDetection clustersdetection = new ClustersDetection();
            clustersdetection.initializer(clusterting.alledges);
            int k = clustersdetection.KClustersDetection();
            textBox4.Text = k.ToString();
            clusteringdetectionstopwatch.Stop();
            TimeSpan cldsw = clusteringdetectionstopwatch.Elapsed;
            detectedtime.Text = cldsw.Minutes + ":" + cldsw.Seconds + ":" + cldsw.Milliseconds;

            //overall
            overallTime.Stop();
            TimeSpan ts = overallTime.Elapsed;
            textBox3.Text = ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        
    }
}