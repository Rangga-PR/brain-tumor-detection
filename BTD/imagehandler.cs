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

namespace BTD
{
    class imagehandler
    {
        public Bitmap crop(Bitmap image,Rectangle rect)
        {
            Bitmap crbmp = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(crbmp);
            g.DrawImage(image, -rect.X, -rect.Y);

            return crbmp;
        }

    }
}
