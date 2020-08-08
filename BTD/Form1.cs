using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using Accord.MachineLearning;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.Rules;
using Accord.IO;
using TreeBuilder;
namespace BTD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        dicomImageHandler dcmhandler = new dicomImageHandler();
        imagehandler imghandler = new imagehandler();
        DataTable tagdt = new DataTable();
        TrainDataHandler tdh = new TrainDataHandler();
        DecisionTree tree= Serializer.Load<DecisionTree>(System.Environment.CurrentDirectory + @"\trees\trees.bin");
        private void folderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderPicker = new FolderBrowserDialog();
            if (folderPicker.ShowDialog() == DialogResult.OK)
            {
                
                listView1.Items.Clear();
                string[] files = Directory.GetFiles(folderPicker.SelectedPath);
                foreach (string file in files)
                {   
                    string fileName = Path.GetFileName(file);
                    ListViewItem item = new ListViewItem(fileName);
                    item.Tag = file;

                    listView1.Items.Add(item);   
                }
                
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            var Selected = listView1.SelectedItems[0];
            //MessageBox.Show(Selected.Tag.ToString());
            //untuk keperluan testing
            //pictureBox1.Image = Bitmap.FromFile(Selected.Tag.ToString());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            var Selecteditem = listView1.SelectedItems;
            if (Selecteditem.Count > 0)
            {
                var Selected = listView1.SelectedItems[0];
                
                var dicomfile = new DicomFile(Selected.Tag.ToString());
                
                pictureBox1.Image = dcmhandler.pixelloader(dicomfile);

                tagdt = dcmhandler.tagreader(dicomfile);
                dataGridView1.DataSource = tagdt;
            }

        }

        private void dICOMFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dicompicker = new OpenFileDialog())
            {
                dicompicker.Filter = "DICOM Files|*.dcm*";
                dicompicker.Title = "Open DICOM Image";

                if (dicompicker.ShowDialog() == DialogResult.OK)
                {  
                    var dicomfile = new DicomFile(dicompicker.FileName);

                    pictureBox1.Image = dcmhandler.pixelloader(dicomfile);
                    dicompicker.Dispose();
                    listView1.Items.Clear();
                 
                    tagdt = dcmhandler.tagreader(dicomfile);
                    dataGridView1.DataSource = tagdt;
                }
            }
            
        }

        Graphics formGraphics;
        bool isDown = false;
        int initialX;
        int initialY;
        Rectangle rect = new Rectangle();

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            initialX = e.X;
            initialY = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (isDown == true)
            {
                pictureBox1.Refresh();
                Pen drwaPen = new Pen(Color.Lime, 1);

                rect.X = Math.Min(e.X, initialX);
                rect.Y = Math.Min(e.Y, initialY);
                rect.Width = Math.Abs(e.X - initialX);
                rect.Height = Math.Abs(e.Y - initialY);

                formGraphics = pictureBox1.CreateGraphics();
                formGraphics.DrawRectangle(drwaPen, rect);

            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }

        private void autoDiagnoseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                {
                    Bitmap img = imghandler.crop(bmp, rect);
                    double[] features = tdh.ExtractFeature(img);
                    string klas=tree.Decide(features).ToString();
                    //string klas = forest.Decide(features).ToString();
                    auto_diagnose ad = new auto_diagnose(img, tagdt,klas);
                    ad.ShowDialog();
                }
            }
            catch
            {
                if (pictureBox1.Image == null)
                {
                    MessageBox.Show("No image, please load image first");
                }
            }
        }
    }
}
