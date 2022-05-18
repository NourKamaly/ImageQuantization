using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] InputImageMatrix;
        public static string openfilepath;

        private void btnOpen_Click(object sender, EventArgs e)
        {
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ColorsConstruction colorsconstruction = new ColorsConstruction();
            Clustering clusterting = new Clustering();
            Quantization quantization = new Quantization();
            List<RGBPixel> distinctcolors = colorsconstruction.getDistincitColors(InputImageMatrix);
            textBox2.Text = distinctcolors.Count.ToString();
            Vertex[] mst = colorsconstruction.mininmumSpanningTree(distinctcolors);
            textBox1.Text = colorsconstruction.sum_mst.ToString();
           
            Dictionary<int, int> kclusters = clusterting.getKClusters(mst, Int32.Parse(numberofclusters.Text), distinctcolors);
            Dictionary<int, int[]> representitiveColors = clusterting.getClusterRepresentitive(kclusters, distinctcolors);
            
            RGBPixel[,] QuantizedMatrix = quantization.Quantize(InputImageMatrix, representitiveColors, kclusters, colorsconstruction.MapColor);
            ImageOperations.DisplayImage(QuantizedMatrix, pictureBox2);
            ClustersDetection clustersdetection = new ClustersDetection();
            clustersdetection.initializer(clusterting.alledges);
            int k = clustersdetection.KClustersDetection();
            textBox4.Text = k.ToString();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            textBox3.Text = ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds;
        }
        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void numberofclusters_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}