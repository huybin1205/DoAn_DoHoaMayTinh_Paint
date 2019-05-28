using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class MiniPaint : Form
    {
        public enum ShapeMode
        {
            LINE,
            DRAWTRIANGLE,
            DRAWSQUARE,
            DRAWCIRCLE,
            DRAWELLIPSE,
            DRAWRECTANGLE,
            FILLTRIANGLE,
            FILLSQUARE,
            FILLCIRCLE,
            FILLELLIPSE,
            FILLRECTANGLE,
            TEXT,
            SELECT,
            ERASER
        }

        private Point _p1;
        private Point _p2;
        private bool _isDown;
        public static Bitmap _bm;
        private Graphics _grs;
        private Color _currColor;
        private int _currPenSize;
        private ShapeMode _currShapeMode;
        private List<Bitmap> dsBitmap;
        private int vt;
        private bool _isSelected;
        private Font _font;
        public static string text = "";
        private string filename = "";

        public MiniPaint()
        {
            InitializeComponent();
        }

        private void MiniPaint_Load(object sender, EventArgs e)
        {
            this.MouseDown += MiniPaint_MouseDown;
            this.MouseMove += MiniPaint_MouseMove;
            this.MouseUp += MiniPaint_MouseUp;
            this.Paint += MiniPaint_Paint;

            _bm = new Bitmap(this.Width, this.Height);
            _grs = Graphics.FromImage(_bm);
            _currColor = Color.Black;
            _currShapeMode = ShapeMode.LINE;

            //Giảm hiện tượng rung cho form
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            //Thêm vào item cho comboBox
            InitComboBoxSize();

            dsBitmap = new List<Bitmap>();
            vt = 0;
            _font = new Font("Arial", 13);
        }

        private void InitComboBoxSize()
        {
            for (int i = 1; i <= 10; i++)
            {
                cbSize.Items.Add(i);
            }
            cbSize.SelectedIndex = 0;
        }

        private void MiniPaint_Paint(object sender, PaintEventArgs e)
        {
            if (_isDown)
            {
                Pen pen;
                SolidBrush sb;
                int dx = _p2.X - _p1.X;
                int dy = _p2.Y - _p1.Y;
                switch (_currShapeMode)
                {
                    case ShapeMode.LINE:
                        pen = new Pen(_currColor, (float)_currPenSize);
                        e.Graphics.DrawLine(pen, _p1, _p2);
                        break;
                    case ShapeMode.DRAWTRIANGLE:
                        pen = new Pen(_currColor, (float)_currPenSize);
                        e.Graphics.DrawLine(pen, _p1, _p2);
                        Point pTemp = new Point(_p1.X, _p2.Y);
                        e.Graphics.DrawLine(pen, _p1, pTemp);
                        e.Graphics.DrawLine(pen, _p2, pTemp);
                        break;
                    case ShapeMode.DRAWRECTANGLE:
                        pen = new Pen(_currColor, (float)_currPenSize);
                        e.Graphics.DrawRectangle(pen, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dy));
                        break;
                    case ShapeMode.DRAWSQUARE:
                        pen = new Pen(_currColor, (float)_currPenSize);
                        e.Graphics.DrawRectangle(pen, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dx));
                        break;
                    case ShapeMode.DRAWCIRCLE:
                        pen = new Pen(_currColor, (float)_currPenSize);
                        e.Graphics.DrawEllipse(pen, _p1.X, _p1.Y, dx, dx);
                        break;
                    case ShapeMode.DRAWELLIPSE:
                        pen = new Pen(_currColor, (float)_currPenSize);
                        e.Graphics.DrawEllipse(pen, _p1.X, _p1.Y, dx, dy);
                        break;
                    case ShapeMode.FILLSQUARE:
                        sb = new SolidBrush(_currColor);
                        e.Graphics.FillRectangle(sb, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dx));
                        break;
                    case ShapeMode.FILLCIRCLE:
                        sb = new SolidBrush(_currColor);
                        e.Graphics.FillEllipse(sb, _p1.X, _p1.Y, dx, dx);
                        break;
                    case ShapeMode.FILLELLIPSE:
                        sb = new SolidBrush(_currColor);
                        e.Graphics.FillEllipse(sb, _p1.X, _p1.Y, dx, dy);
                        break;
                    case ShapeMode.FILLRECTANGLE:
                        sb = new SolidBrush(_currColor);
                        e.Graphics.FillRectangle(sb, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dy));
                        break;
                    case ShapeMode.TEXT:
                        sb = new SolidBrush(_currColor);
                        e.Graphics.DrawString(text, _font, sb, _p1.X, _p1.Y);
                        break;
                    case ShapeMode.ERASER:
                        pen = new Pen(this.BackColor, (float)_currPenSize);
                        e.Graphics.DrawLine(pen, _p1, _p2);
                        break;
                    default:
                        MessageBox.Show("Lỗi");
                        break;
                }
            }
        }

        private void MiniPaint_MouseUp(object sender, MouseEventArgs e)
        {
            XuLyVe();
            XuLyUndoRedo();
        }

        private void XuLyUndoRedo()
        {
            if (_currShapeMode != ShapeMode.ERASER)
            {
                dsBitmap.Add((Bitmap)_bm.Clone());
                vt = dsBitmap.Count - 1;
                ListViewItem lvi = new ListViewItem(_currShapeMode.ToString());
                lvi.SubItems.Add(_p1.X + "");
                lvi.SubItems.Add(_p1.Y + "");
                lvi.SubItems.Add(_p2.X + "");
                lvi.SubItems.Add(_p2.Y + "");
                lvShape.Items.Add(lvi);
            }
        }

        private void XuLyVe()
        {
            _isDown = false;
            Pen pen;
            SolidBrush sb;
            int dx = _p2.X - _p1.X;
            int dy = _p2.Y - _p1.Y;
            switch (_currShapeMode)
            {
                case ShapeMode.LINE:
                    pen = new Pen(_currColor, (float)_currPenSize);
                    _grs.DrawLine(pen, _p1, _p2);
                    break;
                case ShapeMode.DRAWTRIANGLE:
                    pen = new Pen(_currColor, (float)_currPenSize);
                    _grs.DrawLine(pen, _p1, _p2);
                    Point pTemp = new Point(_p1.X, _p2.Y);
                    _grs.DrawLine(pen, _p1, pTemp);
                    _grs.DrawLine(pen, _p2, pTemp);
                    break;
                case ShapeMode.DRAWRECTANGLE:
                    pen = new Pen(_currColor, (float)_currPenSize);
                    _grs.DrawRectangle(pen, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dy));
                    break;
                case ShapeMode.DRAWSQUARE:
                    pen = new Pen(_currColor, (float)_currPenSize);
                    _grs.DrawRectangle(pen, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dx));
                    break;
                case ShapeMode.DRAWCIRCLE:
                    pen = new Pen(_currColor, (float)_currPenSize);
                    _grs.DrawEllipse(pen, _p1.X, _p1.Y, dx, dx);
                    break;
                case ShapeMode.DRAWELLIPSE:
                    pen = new Pen(_currColor, (float)_currPenSize);
                    _grs.DrawEllipse(pen, _p1.X, _p1.Y, dx, dy);
                    break;
                case ShapeMode.FILLSQUARE:
                    sb = new SolidBrush(_currColor);
                    _grs.FillRectangle(sb, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dx));
                    break;
                case ShapeMode.FILLCIRCLE:
                    sb = new SolidBrush(_currColor);
                    _grs.FillEllipse(sb, _p1.X, _p1.Y, dx, dx);
                    break;
                case ShapeMode.FILLELLIPSE:
                    sb = new SolidBrush(_currColor);
                    _grs.FillEllipse(sb, _p1.X, _p1.Y, dx, dy);
                    break;
                case ShapeMode.FILLRECTANGLE:
                    sb = new SolidBrush(_currColor);
                    _grs.FillRectangle(sb, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dy));
                    break;
                case ShapeMode.TEXT:
                    sb = new SolidBrush(_currColor);
                    _grs.DrawString(text, _font, sb, _p1.X, _p1.Y);
                    break;
                case ShapeMode.ERASER:
                    pen = new Pen(this.BackColor, (float) _currPenSize);
                    _grs.DrawLine(pen, _p1, _p2);
                    break;
                default:
                    MessageBox.Show("Lỗi");
                    break;
            }
            this.BackgroundImage = (Bitmap)_bm.Clone();
        }

        private void MiniPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                _p2 = new Point(e.Location.X, e.Location.Y);
                this.Refresh();
            }
            lblLocation.Text = e.X.ToString() + "," + e.Y.ToString();
        }

        private void MiniPaint_MouseDown(object sender, MouseEventArgs e)
        {
            if (vt < dsBitmap.Count - 1)
            {
                for (int i = vt; i < dsBitmap.Count - 1; i++)
                {
                    dsBitmap.RemoveAt(i);
                }
                _grs = Graphics.FromImage(dsBitmap[vt]);
                this.BackgroundImage = (Bitmap)dsBitmap[vt].Clone();
            }
            _isDown = true;
            _p1 = new Point(e.Location.X, e.Location.Y);
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            _currShapeMode = ShapeMode.LINE;
        }

        private void btnTriangle_Click(object sender, EventArgs e)
        {
            _currShapeMode = ShapeMode.DRAWTRIANGLE;
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            frmText frm = new frmText();
            frm.ShowDialog();
            _currShapeMode = ShapeMode.TEXT;
        }

        private void colorPickerDropDown1_SelectedColorChanged(object sender, EventArgs e)
        {
            _currColor = colorPickerDropDown1.SelectedColor;
        }

        private void cbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currPenSize = int.Parse(cbSize.Text);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            _currShapeMode = ShapeMode.SELECT;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            dsBitmap.Clear();
            lvShape.Items.Clear();
            _bm = new Bitmap(this.Width, this.Height);
            _grs = Graphics.FromImage(_bm);
            this.Refresh();
            this.BackgroundImage = (Bitmap)_bm.Clone();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            //Cài đặt bộ lọc đuôi ảnh
            ofd.Filter = "Image files (*.jpg,*.jpeg,*.png,*.bmp,*.tiff)|*jpg;*jpeg;*.png;*.bmp;*.tiff";
            //Cài đặt bộ chọn nhiều
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //Lấy ảnh bằng đường dẫn
                Image img = Image.FromFile(ofd.FileName);
                filename = ofd.FileName;
                //Tạo mới lại Bitmap bằng cỡ ảnh vừa lấy
                _bm = new Bitmap(img.Width, img.Height);
                //Tạo lại đối tượng Graphics cho Bitmap
                _grs = Graphics.FromImage(_bm);
                //Tạo đối tượng HCN bằng với  Bitmap
                Rectangle rec = new Rectangle(0, 0, _bm.Width, _bm.Height);
                //Vẽ ảnh lên Bitmap có cỡ bằng với HCN vừa tạo
                _grs.DrawImage(img, rec);
                this.Refresh();
                this.BackgroundImage = (Bitmap)_bm.Clone();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            //Cài đặt bộ lọc đuôi ảnh
            sfd.Filter = "Bmp (*.bmp)|*.bmp|Jpeg (*.jpeg)|*.jpeg|Jpg (*.jpg)|*.jpg|Png (*.png)|*.png";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _bm.Save(sfd.FileName);
            }
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {

        }

        private void buttonItem20_Click(object sender, EventArgs e)
        {

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            _bm = new Bitmap(_bm, _bm.Width * 100 / 125, _bm.Height * 100 / 125);
            _grs = Graphics.FromImage(_bm);
            this.Refresh();
            this.BackgroundImage = (Bitmap)_bm.Clone();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            _bm = new Bitmap(_bm, _bm.Width * 125 / 100, _bm.Height * 125 / 100);
            _grs = Graphics.FromImage(_bm);
            this.Refresh();
            this.BackgroundImage = (Bitmap)_bm.Clone();
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
            if (btnFill.Checked)
                _currShapeMode = ShapeMode.FILLCIRCLE;
            else
                _currShapeMode = ShapeMode.DRAWCIRCLE;
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            if (btnFill.Checked)
                _currShapeMode = ShapeMode.FILLRECTANGLE;
            else
                _currShapeMode = ShapeMode.DRAWRECTANGLE;
        }

        private void btnEclipse_Click(object sender, EventArgs e)
        {
            if (btnFill.Checked)
                _currShapeMode = ShapeMode.FILLELLIPSE;
            else
                _currShapeMode = ShapeMode.DRAWELLIPSE;
        }

        private void btnSquare_Click(object sender, EventArgs e)
        {
            if (btnFill.Checked)
                _currShapeMode = ShapeMode.FILLSQUARE;
            else
                _currShapeMode = ShapeMode.DRAWSQUARE;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            if(btnFill.Checked == true)
                btnFill.Checked = false;
            else
                btnFill.Checked = true;

        }

        private int selected = 0;
        private void lvShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvShape.SelectedItems.Count > 0)
            {
                Pen pen;
                SolidBrush sb;
                Graphics gr = this.CreateGraphics();
                if (selected != lvShape.Items.IndexOf(lvShape.SelectedItems[0]))
                {
                    this.Refresh();
                    pen = new Pen(_currColor, _currPenSize);
                    sb = new SolidBrush(_currColor);
                    for (int i = 0; i < lvShape.Items.Count - 1; i++)
                    {
                        if (selected == lvShape.Items[i].Index)
                        {
                            XuLySelect(pen, sb, gr, lvShape.Items[i].Text);
                        }
                    }
                }
                int x1 = int.Parse(lvShape.SelectedItems[0].SubItems[1].Text);
                int y1 = int.Parse(lvShape.SelectedItems[0].SubItems[2].Text);
                int x2 = int.Parse(lvShape.SelectedItems[0].SubItems[3].Text);
                int y2 = int.Parse(lvShape.SelectedItems[0].SubItems[4].Text);
                _p1 = new Point(x1, y1);
                _p2 = new Point(x2, y2);
                selected = lvShape.Items.IndexOf(lvShape.SelectedItems[0]);
                pen = new Pen(Color.Red, 2f);
                sb = new SolidBrush(Color.Red);
                XuLySelect(pen, sb, gr, lvShape.SelectedItems[0].Text);
            }

            
        }

        private void XuLySelect(Pen pen, SolidBrush sb, Graphics _gr, string shapeMode)
        {
            int dx = _p2.X - _p1.X;
            int dy = _p2.Y - _p1.Y;
            switch (shapeMode)
            {
                case "LINE":
                    _gr.DrawLine(pen, _p1, _p2);
                    break;
                case "DRAWTRIANGLE":
                    _gr.DrawLine(pen, _p1, _p2);
                    Point pTemp = new Point(_p1.X, _p2.Y);
                    _gr.DrawLine(pen, _p1, pTemp);
                    _gr.DrawLine(pen, _p2, pTemp);
                    break;
                case "DRAWSQUARE":
                    _gr.DrawRectangle(pen, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dx));
                    break;
                case "DRAWCIRCLE":
                    _gr.DrawEllipse(pen, _p1.X, _p1.Y, dx, dx);
                    break;
                case "DRAWELLIPSE":
                    _gr.DrawEllipse(pen, _p1.X, _p1.Y, dx, dy);
                    break;
                case "DRAWRECTANGLE":
                    _gr.DrawRectangle(pen, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dy));
                    break;
                case "FILLRECTANGLE":
                    _gr.FillRectangle(sb, dx > 0 ? _p1.X : _p2.X, dy > 0 ? _p1.Y : _p2.Y, Math.Abs(dx), Math.Abs(dy));
                    break;
                case "FILLELLIPSE":
                    _gr.FillEllipse(sb, _p1.X, _p1.Y, dx, dy);
                    break;
                default:
                    MessageBox.Show("Lỗi");
                    break;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            frmPrint frm = new frmPrint();
            frm.ShowDialog();
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            _currShapeMode = ShapeMode.ERASER;
        }
    }
}
