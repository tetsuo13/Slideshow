using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

// http://www.codeproject.com/Articles/20464/A-Simple-Label-Control-with-Border-Effect
namespace Slideshow
{
    class InfoLabel : Control
    {
        private Color color = Color.Black;
        private int opacity = 100;
        private int shadowDepth = 4;
        private GraphicsPath path;
        private GraphicsPath shadowPath;
        private SolidBrush foreColor;
        private SolidBrush shadowColor;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ExStyle |= 0x00000020;
                return parms;
            }
        }

        public InfoLabel()
        {
            path = new GraphicsPath();
            shadowPath = new GraphicsPath();
            foreColor = new SolidBrush(Color.Tan);
            shadowColor = new SolidBrush(Color.FromArgb(opacity, 11, 11, 11));
            SetStyle(ControlStyles.Opaque, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do nothing.
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x000F)
            {
                DrawText();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Text.Length == 0)
            {
                return;
            }

            DrawText();
        }

        private void DrawText()
        {
            using (Graphics graphics = CreateGraphics())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                SizeF drawSize = graphics.MeasureString(this.Text, this.Font, new PointF(),
                    StringFormat.GenericTypographic);

                PointF point = new PointF
                {
                    X = this.Width - (this.Padding.Right + drawSize.Width) - 10,
                    Y = (this.Height - drawSize.Height) / 2
                };

                PointF shadowPoint = new PointF
                {
                    X = point.X + shadowDepth,
                    Y = point.Y + shadowDepth
                };

                float fontSize = graphics.DpiY * this.Font.SizeInPoints / 72;

                FillString(graphics, shadowPath, shadowPoint, shadowColor, fontSize);
                FillString(graphics, path, point, foreColor, fontSize);
            }
        }

        private void FillString(Graphics graphics, GraphicsPath path, PointF point, SolidBrush brush,
            float fontSize)
        {
            path.Reset();
            path.AddString(this.Text, this.Font.FontFamily, (int)this.Font.Style, fontSize, point,
                StringFormat.GenericTypographic);
            graphics.FillPath(brush, path);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (foreColor != null)
                {
                    foreColor.Dispose();
                }
                if (path != null)
                {
                    path.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
