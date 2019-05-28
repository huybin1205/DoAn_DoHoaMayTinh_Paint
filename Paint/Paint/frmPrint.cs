using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Paint
{
    public partial class frmPrint : Form
    {
        private Bitmap _bm;
        public frmPrint()
        {
            InitializeComponent();
            _bm = MiniPaint._bm;
            
        }

        private void frmPrint_Load(object sender, EventArgs e)
        {
            ptbImage.Image = _bm;
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(ptbImage.Width, ptbImage.Height);
            this.DrawToBitmap(_bm, new Rectangle(0, 0, ptbImage.Width, ptbImage.Height));
            e.Graphics.DrawImage(_bm, 0, 0);
            _bm.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += Doc_PrintPage;
            pd.Document = doc;
            if (pd.ShowDialog() == DialogResult.OK)
            {
                doc.Print();
                this.Close();
            }
        }
    }
}
