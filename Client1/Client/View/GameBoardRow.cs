using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Client.UI
{
    // this class respresents single row inside gameboard GrideDataView (4 desks black/white colors)
    public class GameBoardRow
    {
        public Image Cell1 { get; set; }
        public Image Cell2 { get; set; }
        public Image Cell3 { get; set; }
        public Image Cell4 { get; set; }

        private Image blackDesk = Properties.Resources.blackDesk;
        private Image whiteDesk = Properties.Resources.whiteDesk;

        public GameBoardRow(int type)
        {
            if (type == Constants.evenRow)
            {
                Cell1 = blackDesk;
                Cell2 = whiteDesk;
                Cell3 = blackDesk;
                Cell4 = whiteDesk;
            }
            else
            {
                Cell1 = whiteDesk;
                Cell2 = blackDesk;
                Cell3 = whiteDesk;
                Cell4 = blackDesk;
            }

        }

       
        
    }
}
