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

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ImageOperations.sum_mst = 0;
            List<RGBPixel> dc = ImageOperations.getDistincitColors(ImageMatrix);
            textBox2.Text = dc.Count.ToString();
            Vertex[] mst = ImageOperations.MST(dc);
            Dictionary<int, int> kclusters = ImageOperations.getKClusters(mst,Int32.Parse(numberofclusters.Text), dc);
            Dictionary<int, int[]> representitiveColors = ImageOperations.getClusterRepresentitive(kclusters, dc);
            ClustersDetection.initializer(ImageOperations.alledges);
            int k=ClustersDetection.KClustersDetection();
            textBox4.Text = k.ToString();
            textBox1.Text = ImageOperations.sum_mst.ToString();
            ImageMatrix = Quantization.Quantize(ImageMatrix, representitiveColors, kclusters);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);

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