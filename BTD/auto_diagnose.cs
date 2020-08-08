using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTD
{
    public partial class auto_diagnose : Form
    {

        public auto_diagnose(Bitmap img, DataTable dt,string diag)
        {
            InitializeComponent();
            pictureBox1.Image = img;
            patientinfo = dt;
            if (diag == "0")
            { diag = "ASTROCYTOMA"; }
            else if (diag == "1")
            { diag = "OLIGODENDROGLIOMA"; }
            else if (diag == "2")
            { diag = "GLIOBASTOMA MULTIFORME"; }
            else if (diag == "3")
            { diag = "NORMAL"; }
            label1.Text = "Hasil Diagnosa = " +diag;
        }

        DataTable patientinfo = new DataTable();

        private void auto_diagnose_Load(object sender, EventArgs e)
        {
            Bitmap selectedImage = new Bitmap(pictureBox1.Image);
            
            label2.Text = "Patient ID = " + patientinfo.Rows[0]["Patient ID"];
            label3.Text = "Patient Name = " + patientinfo.Rows[0]["Patient Name"];
            label4.Text = "Patient Age = " + patientinfo.Rows[0]["Patient Age"];
            label5.Text = "Patient Gender = " + patientinfo.Rows[0]["Patient Gender"];
        }
    }
}
