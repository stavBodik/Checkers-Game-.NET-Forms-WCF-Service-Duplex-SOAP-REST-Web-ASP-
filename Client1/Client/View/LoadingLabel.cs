using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.View
{
    // this class used to show loading status when client connected and wating for other player to connect
    public class LoadingLabel:Label
    {

        private Timer animationTimer;
        private int animationProgress = 0;
        private string loadingMessage { get; set; }
        public LoadingLabel(string loadingMessage)
        {
            this.ForeColor = Color.Black;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.BackColor = Color.FromArgb(100, Color.White);
            this.loadingMessage = loadingMessage;
            this.Text = loadingMessage;
            animationTimer = new Timer();
            animationTimer.Interval = 100;
            animationTimer.Tick += new EventHandler(Animation);
        }


        delegate void setTextNoneUiThreadCallBack(string text,Color color);

        public void setTextNoneUiThread(string text,Color color)
        {
            if (this.InvokeRequired)
            {
                setTextNoneUiThreadCallBack cback = new setTextNoneUiThreadCallBack(setTextNoneUiThread);
                this.Invoke(cback, new object[] { text,color });
            }
            else
            {
                this.ForeColor = color;
                this.Text = text;
            }
        }

        private void Animation(object Sender, EventArgs e)
        {


            animationProgress++;
            if (animationProgress == 20)
            {
                this.Text = loadingMessage;
                animationProgress = 0;
            }

            this.Text = this.Text + ".";
        }

        public void StartAnimation()
        {
            animationTimer.Enabled = true;
        }

        delegate void StopAnimationCallBack(string text);

        public void StopAnimation()
        {
            animationTimer.Enabled = false;
        }

    }
}
