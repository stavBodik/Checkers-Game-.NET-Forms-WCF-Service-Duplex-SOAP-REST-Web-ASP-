using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.View
{

    // this class used to show animation on checker move on board ,
    // the animation is flickeing game board cell with random colors while mouse move(right click) holding.
    public class AnimationHolder : PictureBox
    {
        private int animationProgress = 1;

        public Point startPoint { get; set; }

        private Size gameBoardCellSize;

        private Timer animationTimer;


        public int animationStartLocation { get; set; }

        public AnimationHolder(Size gameBoardCellSize)
        {
            animationTimer = new Timer();
            animationTimer.Interval = 1;
            animationTimer.Tick += new EventHandler(animation);
            this.gameBoardCellSize = gameBoardCellSize;
        }

        public void initAnimation(Point startPoint)
        {
            this.startPoint = startPoint;
            animationProgress = 0;
        }
        private void animation(object Sender, EventArgs e)
        {
        animationProgress++;

        if (animationProgress == 60) animationProgress = 40;

        int alpha= 255 % animationProgress;


        Random randomGen = new Random();
        KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
        Color randomColor = Color.FromKnownColor(names[randomGen.Next(names.Length)]);
        Color randomColor1 = Color.FromKnownColor(names[randomGen.Next(names.Length)]);

        Bitmap DrawArea = new Bitmap(Width, Height);

        Image = DrawArea;
        Graphics g = Graphics.FromImage(Image);

        Rectangle rc = new Rectangle(startPoint.X, startPoint.Y-gameBoardCellSize.Height, gameBoardCellSize.Width, gameBoardCellSize.Height);
        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.FromArgb(alpha, randomColor), Color.FromArgb(alpha, randomColor1), 90f);
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.FillRectangle(brush, rc);
        }

        //private void animation(object Sender, EventArgs e)
        //{

        //    if (animationProgress == gameBoardCellSize.Width)
        //    {
        //        animationProgress = 1;
        //        startPoint = new Point(startPoint.X, startPoint.Y);
        //    }
        //    animationProgress++;

        //    Bitmap DrawArea = new Bitmap(Width, Height);
        //    Image = DrawArea;
        //    Color color = ColorTranslator.FromHtml("#0081e1");
        //    int lineSize = gameBoardCellSize.Height;


        //    g = Graphics.FromImage(Image);

        //    int lineYPos = startPoint.Y - gameBoardCellSize.Height;

        //    for (int i = 0; i < 4; i++)
        //    {


        //        Rectangle rc = new Rectangle(startPoint.X, lineYPos, animationProgress, lineSize);

        //        LinearGradientBrush brush = new LinearGradientBrush(rc, Color.FromArgb(alpha, color), Color.FromArgb(0, color), 90f);
        //        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        //        g.FillRectangle(brush, rc);

        //        color = ControlPaint.Light(color, 0.5f);
        //        lineSize = (int)(lineSize * 0.8);
        //        lineYPos += (gameBoardCellSize.Height - lineSize) / 2;

        //    }

        //    g.Dispose();

        //}

        public void startAnimation()
        {
            animationTimer.Enabled=true;
        }

        public void stopAnimation()
        {
            startPoint = new Point (-1,-1);
            animationTimer.Enabled = false;
        }

        
    }
}
