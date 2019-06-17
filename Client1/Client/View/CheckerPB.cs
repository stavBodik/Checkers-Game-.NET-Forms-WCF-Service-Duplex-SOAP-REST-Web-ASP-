using Client.Model;
using Client.ServiceRef;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.UI
{
    // this class represents single checker on gameboard , has image of checker black or white depends on player side .
    public class CheckerPB : PictureBox
    {

        private Size gameBoardCellSize;
        // Checker position Y: COLUMN X:ROW
        public DTO_Checker checker;

        public CheckerPB(String ID,int type, Size size,Point position,Size gameBoardCellSize)
        {

            checker = new DTO_Checker { ID = ID, checkerType = type };

            if (type == Constants.blackChecker)
            {
                this.Image= (Image)(new Bitmap(Properties.Resources.checker_black, size)); 
            }
            else
            {
                this.Image = (Image)(new Bitmap(Properties.Resources.checker_white, size));
            }

            
            this.Width = size.Width;
            this.Height = size.Height;
            this.Location = position;
            this.BackColor = Color.Transparent;
            this.gameBoardCellSize = gameBoardCellSize;
        }

        public void moveToOldPosition()
        {
            int horizontalMoveOffset = (gameBoardCellSize.Width / 2) - (this.Width / 2);
            int verticalMoveOffset = (gameBoardCellSize.Height / 2) - (this.Height / 2);
            this.Location = new Point(horizontalMoveOffset+(checker.oldPositionOnBord.Y)* gameBoardCellSize.Width, verticalMoveOffset + (checker.oldPositionOnBord.X) * gameBoardCellSize.Height);
        }

        public void FixPosition(Point newPosition)
        {
            Console.WriteLine("FixPosition " + newPosition);

            int horizontalMoveOffset = (gameBoardCellSize.Width / 2) - (this.Width / 2);
            int verticalMoveOffset = (gameBoardCellSize.Height / 2) - (this.Height / 2);
            Point newLocation = new Point(horizontalMoveOffset + (newPosition.Y) * gameBoardCellSize.Width, verticalMoveOffset + (newPosition.X) * gameBoardCellSize.Height);

            setPosition(newLocation);
        }

        delegate void SetPositionCallback(Point newPosition);

        public void setPosition(Point newPosition)
        {
            Console.WriteLine("setPosition start " + newPosition);
            if (this.InvokeRequired)
            {
                Console.WriteLine("setPosition InvokeRequired" + newPosition);

                SetPositionCallback fixPositionCallBack = new SetPositionCallback(setPosition);
                this.Invoke(fixPositionCallBack, new object[] { newPosition });
            }
            else
            {
                Console.WriteLine("setPosition normal" + newPosition);

                this.Location = newPosition;
            }
        }
    }
}
