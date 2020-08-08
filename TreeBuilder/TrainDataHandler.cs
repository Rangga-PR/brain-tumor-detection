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
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.MachineLearning;

namespace TreeBuilder
{
    class TrainDataHandler
    {
        Median medianfilter = new Median();
        GrayLevelCooccurrenceMatrix glcm = new GrayLevelCooccurrenceMatrix();
        GrayLevelCooccurrenceMatrix glcm45 = new GrayLevelCooccurrenceMatrix();
        GrayLevelCooccurrenceMatrix glcm90 = new GrayLevelCooccurrenceMatrix();
        GrayLevelCooccurrenceMatrix glcm130 = new GrayLevelCooccurrenceMatrix();

        //ResizeBilinear rez = new ResizeBilinear(150,150);
        public double[] ExtractFeature(Bitmap tumor)
        {
            glcm45.Degree=CooccurrenceDegree.Degree45;
            glcm90.Degree = CooccurrenceDegree.Degree90;
            glcm130.Degree = CooccurrenceDegree.Degree135;

            //double[] features = new double[13];
            double[] features = new double[52];
            double[] avgfeatures = new double[13];
            medianfilter.ApplyInPlace(tumor);
            //tumor = rez.Apply(tumor);
            tumor = Grayscale.CommonAlgorithms.BT709.Apply(tumor);
            double[,] matrix= glcm.Compute(tumor);
            double[,] matrix1 = glcm45.Compute(tumor);
            double[,] matrix2 = glcm90.Compute(tumor);
            double[,] matrix3 = glcm130.Compute(tumor);

            HaralickDescriptor descriptor = new HaralickDescriptor(matrix);
            HaralickDescriptor descriptor1 = new HaralickDescriptor(matrix1);
            HaralickDescriptor descriptor2 = new HaralickDescriptor(matrix2);
            HaralickDescriptor descriptor3 = new HaralickDescriptor(matrix3);

            features[0] = descriptor.F01;
            features[1] = descriptor.F02;
            features[2] = descriptor.F03;
            features[3] = descriptor.F04;
            features[4] = descriptor.F05;
            features[5] = descriptor.F06;
            features[6] = descriptor.F07;
            features[7] = descriptor.F08;
            features[8] = descriptor.F09;
            features[9] = descriptor.F10;
            features[10] = descriptor.F11;
            features[11] = descriptor.F12;
            features[12] = descriptor.F13;

            features[13] = descriptor1.F01;
            features[14] = descriptor1.F02;
            features[15] = descriptor1.F03;
            features[16] = descriptor1.F04;
            features[17] = descriptor1.F05;
            features[18] = descriptor1.F06;
            features[19] = descriptor1.F07;
            features[20] = descriptor1.F08;
            features[21] = descriptor1.F09;
            features[22] = descriptor1.F10;
            features[23] = descriptor1.F11;
            features[24] = descriptor1.F12;
            features[25] = descriptor1.F13;

            features[26] = descriptor2.F01;
            features[27] = descriptor2.F02;
            features[28] = descriptor2.F03;
            features[29] = descriptor2.F04;
            features[30] = descriptor2.F05;
            features[31] = descriptor2.F06;
            features[32] = descriptor2.F07;
            features[33] = descriptor2.F08;
            features[34] = descriptor2.F09;
            features[35] = descriptor2.F10;
            features[36] = descriptor2.F11;
            features[37] = descriptor2.F12;
            features[38] = descriptor2.F13;

            features[39] = descriptor3.F01;
            features[40] = descriptor3.F02;
            features[41] = descriptor3.F03;
            features[42] = descriptor3.F04;
            features[43] = descriptor3.F05;
            features[44] = descriptor3.F06;
            features[45] = descriptor3.F07;
            features[46] = descriptor3.F08;
            features[47] = descriptor3.F09;
            features[48] = descriptor3.F10;
            features[49] = descriptor3.F11;
            features[50] = descriptor3.F12;
            features[51] = descriptor3.F13;

            /*
            avgfeatures[0] = (features[0] + features[13] + features[26] + features[39]) / 4;
            avgfeatures[1] = (features[1] + features[14] + features[27] + features[40]) / 4;
            avgfeatures[2] = (features[2] + features[15] + features[28] + features[41]) / 4;
            avgfeatures[3] = (features[3] + features[16] + features[29] + features[42]) / 4;
            avgfeatures[4] = (features[4] + features[17] + features[30] + features[43]) / 4;
            avgfeatures[5] = (features[5] + features[18] + features[31] + features[44]) / 4;
            avgfeatures[6] = (features[6] + features[19] + features[32] + features[45]) / 4;
            avgfeatures[7] = (features[7] + features[20] + features[33] + features[46]) / 4;
            avgfeatures[8] = (features[8] + features[21] + features[34] + features[47]) / 4;
            avgfeatures[9] = (features[9] + features[22] + features[35] + features[48]) / 4;
            avgfeatures[10] = (features[10] + features[23] + features[36] + features[49]) / 4;
            avgfeatures[11] = (features[11] + features[24] + features[37] + features[50]) / 4;
            avgfeatures[12] = (features[12] + features[25] + features[38] + features[51]) / 4;
            */
            return features; 
        }

        public DataTable createdataheaders(DataTable trainingdata)
        {
            //trainingdata.Columns.Add("Patient Name", typeof(string));
            //trainingdata.Columns.Add("Patient ID", typeof(string));
            //trainingdata.Columns.Add("Study ID", typeof(string));
            trainingdata.Columns.Add("Energy", typeof(double));
            trainingdata.Columns.Add("Contrast", typeof(double));
            trainingdata.Columns.Add("Correlation", typeof(double));
            trainingdata.Columns.Add("Variance", typeof(double));
            trainingdata.Columns.Add("Inverse Difference Moment", typeof(double));
            trainingdata.Columns.Add("Sum Average", typeof(double));
            trainingdata.Columns.Add("Sum Variance", typeof(double));
            trainingdata.Columns.Add("Sum Entropy", typeof(double));
            trainingdata.Columns.Add("Entropy", typeof(double));
            trainingdata.Columns.Add("Difference Variance", typeof(double));
            trainingdata.Columns.Add("Difference Enropy", typeof(double));
            trainingdata.Columns.Add("First Information Measure", typeof(double));
            trainingdata.Columns.Add("Second Information Measure", typeof(double));
            
            trainingdata.Columns.Add("Energy45", typeof(double));
            trainingdata.Columns.Add("Contrast45", typeof(double));
            trainingdata.Columns.Add("Correlation45", typeof(double));
            trainingdata.Columns.Add("Variance45", typeof(double));
            trainingdata.Columns.Add("Inverse Difference Moment45", typeof(double));
            trainingdata.Columns.Add("Sum Average45", typeof(double));
            trainingdata.Columns.Add("Sum Variance45", typeof(double));
            trainingdata.Columns.Add("Sum Entropy45", typeof(double));
            trainingdata.Columns.Add("Entropy45", typeof(double));
            trainingdata.Columns.Add("Difference Variance45", typeof(double));
            trainingdata.Columns.Add("Difference Enropy45", typeof(double));
            trainingdata.Columns.Add("First Information Measure45", typeof(double));
            trainingdata.Columns.Add("Second Information Measure45", typeof(double));

            trainingdata.Columns.Add("Energy90", typeof(double));
            trainingdata.Columns.Add("Contrast90", typeof(double));
            trainingdata.Columns.Add("Correlation90", typeof(double));
            trainingdata.Columns.Add("Variance90", typeof(double));
            trainingdata.Columns.Add("Inverse Difference Moment90", typeof(double));
            trainingdata.Columns.Add("Sum Average90", typeof(double));
            trainingdata.Columns.Add("Sum Variance90", typeof(double));
            trainingdata.Columns.Add("Sum Entropy90", typeof(double));
            trainingdata.Columns.Add("Entropy90", typeof(double));
            trainingdata.Columns.Add("Difference Variance90", typeof(double));
            trainingdata.Columns.Add("Difference Enropy90", typeof(double));
            trainingdata.Columns.Add("First Information Measure90", typeof(double));
            trainingdata.Columns.Add("Second Information Measure90", typeof(double));

            trainingdata.Columns.Add("Energy130", typeof(double));
            trainingdata.Columns.Add("Contrast130", typeof(double));
            trainingdata.Columns.Add("Correlation130", typeof(double));
            trainingdata.Columns.Add("Variance130", typeof(double));
            trainingdata.Columns.Add("Inverse Difference Moment130", typeof(double));
            trainingdata.Columns.Add("Sum Average130", typeof(double));
            trainingdata.Columns.Add("Sum Variance130", typeof(double));
            trainingdata.Columns.Add("Sum Entropy130", typeof(double));
            trainingdata.Columns.Add("Entropy130", typeof(double));
            trainingdata.Columns.Add("Difference Variance130", typeof(double));
            trainingdata.Columns.Add("Difference Enropy130", typeof(double));
            trainingdata.Columns.Add("First Information Measure130", typeof(double));
            trainingdata.Columns.Add("Second Information Measure130", typeof(double));
            
            trainingdata.Columns.Add("Tumor", typeof(string));

            return trainingdata;
        }

        public void CreateTrainingData(DicomFile dicomfile,Bitmap img,DataTable data,string kelas)
        {
            dicomfile.Load();
            double[] f = ExtractFeature(img);
            string pn = dicomfile.DataSet.GetAttribute(DicomTags.PatientsName);
            string pnid = dicomfile.DataSet.GetAttribute(DicomTags.PatientId);
            string studid = dicomfile.DataSet.GetAttribute(DicomTags.StudyId);


            //data.Rows.Add(/*pn, pnid,studid,*/f[0], f[1], f[2], f[3], f[4], f[5], f[6], f[7], f[8], f[9], f[10], f[11], f[12],kelas);
            data.Rows.Add(/*pn, pnid,studid,*/f[0], f[1], f[2], f[3], f[4], f[5], f[6], f[7], f[8], f[9], f[10], f[11], f[12], f[13], f[14], f[15], f[16], f[17], f[18], f[19], f[20], f[21], f[22], f[23], f[24], f[25], f[26], f[27], f[28], f[29], f[30], f[31], f[32], f[33], f[34], f[35], f[36], f[37], f[38],f[39], f[40], f[41], f[42], f[43], f[44], f[45], f[46], f[47], f[48], f[49], f[50], f[51], kelas);
        }
    }
}
