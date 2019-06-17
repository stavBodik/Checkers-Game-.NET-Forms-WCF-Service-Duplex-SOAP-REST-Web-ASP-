using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.View
{
    // this class used to show current player turn in game includes checker color, team name,player name
    public class OpponentPB : Panel
    {
        public Label opponentNameLB;
        public OpponentPB(int type,Size size)
        {

            double labelPresendHeight = 0.8;
            this.Width = size.Width;
            this.Height = (int)(size.Width*(1+ labelPresendHeight));
            this.BackColor = Color.Transparent;


            opponentNameLB = new Label();
            opponentNameLB.TextAlign = ContentAlignment.MiddleCenter;
            opponentNameLB.Text = "OPPONENT \n";
            opponentNameLB.ForeColor = Color.White;
            this.Controls.Add(opponentNameLB);
            opponentNameLB.Width = this.Width;
            opponentNameLB.Height = (int)(size.Width * labelPresendHeight);

            PictureBox pb = new PictureBox();
            Size imageSize = new Size((int)(size.Width), (int)(size.Width));
            pb.Size = imageSize;
            pb.Parent = this;
            pb.Top = this.Height- imageSize.Height;
            pb.Left = this.Width / 2 - pb.Width / 2;

            if (type == Constants.blackChecker)
            {
                pb.Image = (Image)(new Bitmap(Properties.Resources.checker_black, imageSize));
            }
            else
            {
                pb.Image = (Image)(new Bitmap(Properties.Resources.checker_white, imageSize));
            }
        }


        public void setOpponentText(string team,string name)
        {
            opponentNameLB.Invoke((MethodInvoker)delegate {
                opponentNameLB.Text = team + "\n" + name;
            });
        }
    }
}
