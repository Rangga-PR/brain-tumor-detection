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

namespace BTD
{
    class dicomImageHandler
    {
        int imgWidth;
        int imgHeight;
        Bitmap imgbmp;
        DataTable tags=new DataTable();

        public Bitmap pixelloader(DicomFile dicomfile)
        {
            LocalSopDataSource DatosImagen = new LocalSopDataSource(dicomfile);

            ImageSop imageSop = new ImageSop(DatosImagen);

            IPresentationImage imagen_a_mostrar = PresentationImageFactory.Create(imageSop.Frames[1]);

            imgWidth = imageSop.Frames[1].Columns;

            imgHeight = imageSop.Frames[1].Rows;

            imgbmp = imagen_a_mostrar.DrawToBitmap(imgWidth, imgHeight);

            return imgbmp;
        }

        private string dateparser(string dates)
        {
            string output = "";
            try
            {
                DateTime dtp = DateTime.ParseExact(dates, "yyyyMMdd", null);

                output = dtp.Day + "-" + dtp.Month + "-" + dtp.Year.ToString();
            }
            catch
            {
                output = "";
            }
            return output;
        }

        public DataTable tagreader(DicomFile dicomfile)
        {
            dicomfile.Load();
            tags.Columns.Clear();
            tags.Rows.Clear();
            tags.Columns.Add("Patient Name", typeof(string));
            tags.Columns.Add("Patient ID", typeof(string));
            tags.Columns.Add("Patient Birth Date", typeof(string));
            tags.Columns.Add("Patient Gender", typeof(string));
            tags.Columns.Add("Patient Age", typeof(string));
            tags.Columns.Add("Patient Weight", typeof(string));
            tags.Columns.Add("Patient Height", typeof(string));
            tags.Columns.Add("Study Date", typeof(string));
            tags.Columns.Add("Study ID", typeof(string));
            tags.Columns.Add("Study Modality", typeof(string));

            string pn = dicomfile.DataSet.GetAttribute(DicomTags.PatientsName);
            string pnid = dicomfile.DataSet.GetAttribute(DicomTags.PatientId);
            string pnbd = dateparser(dicomfile.DataSet.GetAttribute(DicomTags.PatientsBirthDate));
            string pngen = dicomfile.DataSet.GetAttribute(DicomTags.PatientsSex);
            string pnage = dicomfile.DataSet.GetAttribute(DicomTags.PatientsAge);
            string pnw = dicomfile.DataSet.GetAttribute(DicomTags.PatientsWeight);
            string pnh = dicomfile.DataSet.GetAttribute(DicomTags.PatientsSize);

            string studt = dateparser(dicomfile.DataSet.GetAttribute(DicomTags.StudyDate));
            string studid = dicomfile.DataSet.GetAttribute(DicomTags.StudyId);
            string studmod = dicomfile.DataSet.GetAttribute(DicomTags.Modality);

            tags.Rows.Add(pn, pnid, pnbd, pngen, pnage, pnw, pnh, studt, studid, studmod);

            return tags;

        }
    }
}
