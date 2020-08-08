using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Accord.IO;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Filters;
using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees.Pruning;
using Accord.MachineLearning.DecisionTrees.Rules;
using BTD;

namespace TreeBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataTable dt = new DataTable();

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtcreator dtc = new dtcreator();
            dtc.ShowDialog();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open data training";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var reader = new CsvReader(dlg.FileName, hasHeaders: true);
                    dt = reader.ToTable();
                    dataGridView1.DataSource = dt;

                    // codebook sebagai auto converter
                    var codebook = new Codification(dt);

                    //convert categorical data into numerik
                    DataTable symbols = codebook.Apply(dt);
                    //ekstract feautures data
                    //input = dt.ToJagged<Double>("Energy", "Contrast", "Correlation", "Variance", "Inverse Difference Moment", "Sum Average", "Sum Variance", "Sum Entropy", "Entropy", "Difference Variance", "Difference Enropy", "First Information Measure", "Second Information Measure");

                    input = dt.ToJagged<Double>("Energy", "Contrast", "Correlation", "Variance", "Inverse Difference Moment", "Sum Average", "Sum Variance", "Sum Entropy", "Entropy", "Difference Variance", "Difference Enropy", "First Information Measure", "Second Information Measure", "Energy45", "Contrast45", "Correlation45", "Variance45", "Inverse Difference Moment45", "Sum Average45", "Sum Variance45", "Sum Entropy45", "Entropy45", "Difference Variance45", "Difference Enropy45", "First Information Measure45", "Second Information Measure45", "Energy90", "Contrast90", "Correlation90", "Variance90", "Inverse Difference Moment90", "Sum Average90", "Sum Variance90", "Sum Entropy90", "Entropy90", "Difference Variance90", "Difference Enropy90", "First Information Measure90", "Second Information Measure90", "Energy130", "Contrast130", "Correlation130", "Variance", "Inverse Difference Moment130", "Sum Average130", "Sum Variance130", "Sum Entropy130", "Entropy130", "Difference Variance130", "Difference Enropy130", "First Information Measure130", "Second Information Measure130");

                    //input = dt.ToJagged<Double>("Energy", "Contrast","Inverse Difference Moment", "Entropy");
                    //ekstract ouput data
                    output = symbols.Columns["Tumor"].ToArray<int>();
                }
            }
        }
        DecisionTree pohon=null;
        //RandomForest forest = null;
        Double[][] input = null;
        int[] output = null;

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch stp = new Stopwatch();
            C45Learning teach = new C45Learning();
            
            var cv = CrossValidation.Create(
               k: 10,

               learner: (p) => new C45Learning() {Join=0 }
               ,
               
               // zerooneloss measure
               loss: (actual, expected, p) => new ZeroOneLoss(expected).Loss(actual),
               
               fit: (teachers, x, y, w) => teachers.Learn(x, y, w),

               //spesifikan data input dan output
               x: input, y: output

           );
            try
            {
                // start learning
                stp.Start();
                var result = cv.Learn(input, output);
                pohon = teach.Learn(input, output); // save in global variable
                stp.Stop();
                //double trainingError = result.Training.Mean;
                double validationError = result.Validation.Mean;
                double akurasi = 1 - validationError;
                MessageBox.Show("Build Succeed, Testing Data accuracy = " + akurasi+" || Execution Time = "+stp.Elapsed);
            }
            catch { MessageBox.Show("build failed"); }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sdl = new SaveFileDialog())
            {
                sdl.Filter = "binary file|*.bin";
                //sdl.InitialDirectory=@"D:\Project\BTD\";
                if (sdl.ShowDialog() == DialogResult.OK)
                {
                    Serializer.Save(obj: pohon, path: sdl.FileName); //save tree
                }
            }
        }
    }
}
