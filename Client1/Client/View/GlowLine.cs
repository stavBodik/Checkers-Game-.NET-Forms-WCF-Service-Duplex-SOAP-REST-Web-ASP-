using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.View
{
    // this class not used in the project , written as option for making glowing line animation on checker moves
    class GlowLineHolder : PictureBox
    {
        private Graphics g;
        private Color baseColor;
        public int glowingLineHeight{get;set;}
        public GlowLineHolder(int width,int height,string color)
        {
            Bitmap DrawArea = new Bitmap(width, height);
            Image = DrawArea;
            g = Graphics.FromImage(Image);
            baseColor = ColorTranslator.FromHtml(color);
            this.Width = width;
            this.Height = height;
            glowingLineHeight = 5;
        }


       public void update()
        {
            drawLine(this.Width, this.Height, baseColor);
        }
        public void drawLine(int lineWidth, int linehHeight, Color color)
        {
            while (lineWidth > 2)
            {

                Rectangle rc = new Rectangle(this.Width/2, 0, lineWidth / 2, linehHeight);
                LinearGradientBrush brush = new LinearGradientBrush(rc, Color.FromArgb(255, color), Color.FromArgb(0, color), 0f);
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.FillRectangle(brush, rc);

                rc = new Rectangle((this.Width / 2)- (lineWidth / 2), 0, lineWidth / 2, linehHeight);
                brush = new LinearGradientBrush(rc, Color.FromArgb(0, color), Color.FromArgb(255, color), 0f);
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.FillRectangle(brush, rc);

                color = ControlPaint.Light(color, 0.5f);
                lineWidth = (int)(lineWidth * 0.8);

            }
        }

    }
}
