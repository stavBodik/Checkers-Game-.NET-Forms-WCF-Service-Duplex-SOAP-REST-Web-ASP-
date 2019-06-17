using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.View
{
    // this class used to hold application menu items
    public class ApplicationMenu: MenuStrip
    {
        private ToolStripMenuItem menu;
        public  ToolStripMenuItem loginoutMenuItem;
        public  ToolStripMenuItem playLocalGameMenuItem;
        public  ToolStripMenuItem joinGameMenuItem;
        public  ToolStripMenuItem restorePrevGames;
        public  ToolStripMenuItem aboutMenuItem;
        public  ToolStripMenuItem uiExerciseMenu;
        public  ToolStripMenuItem uiExerciseCheckBox;
        public  ToolStripMenuItem uiExerciseCheckBoxFix;


        public ApplicationMenu()
        {
            menu = new ToolStripMenuItem();
            menu.Text = Constants.menuS;

            this.Items.Add(menu);

            loginoutMenuItem = new ToolStripMenuItem();
            loginoutMenuItem.Text = Constants.loginS;
            menu.DropDownItems.Add(loginoutMenuItem);

            playLocalGameMenuItem = new ToolStripMenuItem();
            playLocalGameMenuItem.Text = Constants.playS;
            playLocalGameMenuItem.Enabled = false;
            menu.DropDownItems.Add(playLocalGameMenuItem);

            joinGameMenuItem = new ToolStripMenuItem();
            joinGameMenuItem.Text =Constants.joingameS;
            joinGameMenuItem.Enabled = false;
            menu.DropDownItems.Add(joinGameMenuItem);

            restorePrevGames = new ToolStripMenuItem();
            restorePrevGames.Text = Constants.restoregameS;
            restorePrevGames.Enabled = false;
            menu.DropDownItems.Add(restorePrevGames);

            aboutMenuItem = new ToolStripMenuItem();
            aboutMenuItem.Text = Constants.aboutS;

            this.Items.Add(aboutMenuItem);

            uiExerciseMenu = new ToolStripMenuItem();
            uiExerciseMenu.Text = Constants.uiExercise;

            uiExerciseCheckBox = new ToolStripMenuItem();
            uiExerciseCheckBox.Text = Constants.uiExercise;
            uiExerciseCheckBox.CheckOnClick = true;
            uiExerciseMenu.DropDownItems.Add(uiExerciseCheckBox);


            uiExerciseCheckBoxFix = new ToolStripMenuItem();
            uiExerciseCheckBoxFix.Text = Constants.uiExercise + "Fix With Thread";
            uiExerciseCheckBoxFix.CheckOnClick = true;
            uiExerciseMenu.DropDownItems.Add(uiExerciseCheckBoxFix);

            this.Items.Add(uiExerciseMenu);

        }
    }
}
