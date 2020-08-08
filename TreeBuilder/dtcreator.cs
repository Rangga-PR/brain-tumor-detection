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
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.IO;
using BTD;

namespace TreeBuilder
{
    public partial class dtcreator : Form
    {
        public dtcreator()
        {
            InitializeComponent();
        }
        
        dicomImageHandler dcmhandler = new dicomImageHandler();
        imagehandler imghandler = new imagehandler();
        DataTable dt = new DataTable();
        DicomFile dicomfile = new DicomFile();
        TrainDataHandler tdh = new TrainDataHandler();

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderPicker = new FolderBrowserDialog();
            folderPicker.SelectedPath = @"D:\REMBRANDT\";
            string searchPattern = "*.dcm"; //filter filetype
            if (folderPicker.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
                string[] files = Directory.GetFiles(folderPicker.SelectedPath,searchPattern,SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    ListViewItem item = new ListViewItem(fileName);
                    item.Tag = file;

                    listView1.Items.Add(item);
                }

            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var Selecteditem = listView1.SelectedItems;
            if (Selecteditem.Count > 0)
            {
                var Selected = listView1.SelectedItems[0].Tag;
                dicomfile = new DicomFile(Selected.ToString());

                pictureBox1.Image = dcmhandler.pixelloader(dicomfile);
                //pictureBox2.Image = System.Drawing.Image.FromFile(Selected.ToString()); // for normal image format
                
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
            if (isDown == true) // to avoid unintentional clicks from dropdown menu
            {
                isDown = false;
                using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                {
                    try
                    {
                        Bitmap img = imghandler.crop(bmp, rect);
                        pictureBox2.Image = img;
                    }
                    catch
                    {
                        MessageBox.Show("please select area");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

                var Selected = listView1.SelectedItems[0].Tag;
                dicomfile = new DicomFile(Selected.ToString());

                pictureBox1.Image = dcmhandler.pixelloader(dicomfile);

                Bitmap bm = new Bitmap(pictureBox2.Image);
                tdh.CreateTrainingData(dicomfile,bm, dt, comboBox1.SelectedItem.ToString());
                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
                listBox1.Items.Add("Number = " + dt.Rows.Count);
                // listBox1.Items.Add("Patient ID = " + dt.Rows[dt.Rows.Count-1]["Patient ID"]);
                // listBox1.Items.Add("Patient Name = " + dt.Rows[dt.Rows.Count-1]["Patient Name"]);
                // listBox1.Items.Add("Study ID = " + dt.Rows[dt.Rows.Count-1]["Study ID"]);
                listBox1.Items.Add(" ");
        }

        private void dtcreator_Load(object sender, EventArgs e)
        {
            tdh.createdataheaders(dt);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sdl = new SaveFileDialog())
            {
                sdl.Filter = "Comma Separated Value text|*.txt";

                if (sdl.ShowDialog() == DialogResult.OK)
                {
                    using (CsvWriter writer = new CsvWriter(sdl.FileName))
                    {
                        writer.Write(dt);
                    }
                }
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open data training";
                
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var reader = new CsvReader(dlg.FileName, hasHeaders: true);
                    dt = reader.ToTable();
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sdl = new SaveFileDialog())
            {
                sdl.Filter = "Images|*.jpg ; *.png ; *.bmp";
                sdl.Title = "Save Image";

                if (sdl.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image.Save(sdl.FileName);

                }
            }
        }
    }
}
