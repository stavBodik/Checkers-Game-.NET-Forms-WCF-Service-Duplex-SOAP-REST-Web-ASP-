using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Client.UI;
using System.Drawing.Imaging;
using Client.Control;
using Client.View;
using Client.Model;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Client.ServiceRef;
using System.Threading;

namespace Client
{
    public partial class GameForm : Form
    {
        private DataGridView GameBoard;  // Game board table 
        private List<GameBoardRow> gameBoardGrid; // each row in DataGridView is constructed from this single row,contains Image cells. 
        private Point MouseDownLocation; // saves the location of mouse down event for calculate moving offset .
        private PictureBox mainPanelPB; // explained on DesignMainPanel method .
        public  AnimationHolder animationHolder; // explained on DesignMainPanel method .
        private Point currentPositionOfSelectedCheckerOnBoard; // the position(row/column) of current selected checker on gameboard gride view 
        public  LoadingLabel loadingLabel; // loading label while wating opponents handshake 
        private Panel gamePanel,loginPanel,startOnlineGamePanel, startLocalGamePanel, restoreGamesPanel;
        public ApplicationMenu applicationMenu;
        private AboutBox aboutBox;
        private LoadingLabel joinGameErrorMessageLabel;
        private DataGridView restoredGamesDGV;
        private ListBox playersLB; // List of logged in account player , used when user choose to play vs pc.
        private GameRestoreAnimator gameRestoreAnimator; // animation class for viewing old games
        private ErrorProvider ep = new ErrorProvider();

        public GameForm()
        {
            InitializeComponent();
        }
        //Intiation of the game form with application menu and login panel
        private void GameForm_Load(object sender, EventArgs e)
        {
            DesignForm();
            LoadMenu();
            loadAndDesignLoginPanel();
        }

        private void loadAndDesignLoginPanel()
        {
            loginPanel = new Panel();
            loginPanel.BackColor = Color.FromArgb(180, Color.White);
            loginPanel.Parent = this;
            loginPanel.Width = (int)(this.Width * 0.5);
            loginPanel.Height = (int)(this.Width * 0.2);
            loginPanel.BorderStyle = BorderStyle.FixedSingle;

            // center login panel inside form
            loginPanel.Top = (this.ClientSize.Height - loginPanel.Height) / 2;
            loginPanel.Left = (this.ClientSize.Width - loginPanel.Width) / 2;


            Label loginLB = new Label();
            loginLB.Width = loginPanel.Width;
            loginLB.Font = new Font(this.Font.FontFamily, 10);
            loginLB.TextAlign = ContentAlignment.MiddleCenter;
            loginLB.Text = Constants.LOGIN;
            loginLB.Parent = loginPanel;

            TextBox email = new TextBox();
            email.Width = (int)(loginPanel.Width * 0.6);
            email.Left = (loginPanel.Width - email.Width) / 2;
            email.Top = loginLB.Bottom+5;
            email.Text = "EMAIL";
            email.Parent = loginPanel;

            TextBox password = new TextBox();
            password.PasswordChar = '*';
            password.Width = (int)(loginPanel.Width * 0.6);
            password.Left = (loginPanel.Width - email.Width) / 2;
            password.Top = email.Top +10+ email.Height;
            password.Text = "PASSWORD";
            password.Parent = loginPanel;


            Button loginBT = new Button();
            loginBT.Width = (int)(loginPanel.Width * 0.5);
            loginBT.Left = (loginPanel.Width - loginBT.Width) / 2;
            loginBT.Top = password.Top + 10 + email.Height;
            loginBT.Text = "Login";
            loginBT.Parent = loginPanel;

            LoadingLabel messageLabel = new LoadingLabel("");
            messageLabel.BackColor = Color.Transparent;
            messageLabel.Width = (int)(loginPanel.Width * 0.5);
            messageLabel.Left = (loginPanel.Width - messageLabel.Width) / 2;
            messageLabel.Top = loginBT.Bottom-3 ;
            messageLabel.Parent = loginPanel;
            messageLabel.ForeColor = Color.Black;

            loginBT.Click += (sender, EventArgs) => { onLoginButtonClick(sender, EventArgs, email, password, messageLabel); };

        }

        // This methods used to load and design different panels in application
        // Usually called after retriving information from server in seperated threads

        delegate void LoadGamesForRestorePanelCallBack(DTO_GamePlay[] games);
        public void LoadAndDesignGamesForRestorePanel(DTO_GamePlay[] games)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    LoadGamesForRestorePanelCallBack cback = new LoadGamesForRestorePanelCallBack(LoadAndDesignGamesForRestorePanel);
                    this.Invoke(cback, new object[] { games });
                }
                else
                {

                    restoreGamesPanel = new Panel();
                    restoreGamesPanel.Width = (int)(this.Width * 0.8);
                    restoreGamesPanel.Height = (int)(this.Width * 0.5);
                    restoreGamesPanel.Top = (this.ClientSize.Height - restoreGamesPanel.Height) / 2;
                    restoreGamesPanel.Left = (this.ClientSize.Width - restoreGamesPanel.Width) / 2;
                    restoreGamesPanel.BackColor = Color.FromArgb(180, Color.White);
                    restoreGamesPanel.Parent = this;
                    restoreGamesPanel.BorderStyle = BorderStyle.FixedSingle;

                    Label header = new Label();
                    header.Width = restoreGamesPanel.Width;
                    header.Font = new Font(this.Font.FontFamily, 12);
                    header.Text = Constants.CHOSE_GAME_RESTORE;
                    header.Parent = restoreGamesPanel;
                    header.BackColor = Color.FromArgb(150, Color.White);



                    restoredGamesDGV = new DataGridView();

                    restoredGamesDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    restoredGamesDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    restoredGamesDGV.DataSource = games;
                    restoredGamesDGV.Parent = restoreGamesPanel;
                    restoredGamesDGV.Columns["hostPlayerID"].Visible = false;
                    restoredGamesDGV.Columns["joinedPlayerID"].Visible = false;
                    restoredGamesDGV.Columns["Status"].Visible = false;
                    restoredGamesDGV.Columns["hostPlayerAccountEmail"].Visible = false;

                    restoredGamesDGV.Columns["gameName"].HeaderText = "Name";
                    restoredGamesDGV.Columns["createDate"].HeaderText = "Date";
                    restoredGamesDGV.Columns["hostPlayerName"].HeaderText = "White Player";
                    restoredGamesDGV.Columns["joinPlayerName"].HeaderText = "Black Player";

                    restoredGamesDGV.Width = restoreGamesPanel.Width;
                    restoredGamesDGV.Height = (int)(restoreGamesPanel.Height * 0.8);
                    restoredGamesDGV.Top = header.Bottom;
                    restoredGamesDGV.ReadOnly = true;
                    foreach (DataGridViewColumn col in restoredGamesDGV.Columns)
                    {
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    Button RestoreGame = new Button();
                    RestoreGame.Text = Constants.RESTORE_GAME;
                    RestoreGame.Top = restoredGamesDGV.Bottom + 10;
                    RestoreGame.Left = (restoreGamesPanel.Width - RestoreGame.Width) / 2;
                    RestoreGame.Parent = restoreGamesPanel;

                    if (games.Length == 0)
                    {
                        RestoreGame.Enabled = false;
                    }

                    RestoreGame.Click += (sender, EventArgs) => { OnRestoreGameClick(sender, EventArgs); };
                }

            }
            catch (Exception)
            {
                // this sepceial case when we exercise closing the form while UI is loading games for restore
                // when we signal form closing , the form is closed so we catch the exepcion here .
            }
        }

        delegate void LoadAndDesignLocalGameStartPanelCallBack(DTO_Player[] players);
        public void LoadAndDesignLocalGameStartPanel(DTO_Player[] players)
        {
            if (this.InvokeRequired)
            {
                LoadAndDesignLocalGameStartPanelCallBack cback = new LoadAndDesignLocalGameStartPanelCallBack(LoadAndDesignLocalGameStartPanel);
                this.Invoke(cback, new object[] { players });
            }
            else
            {
                startOnlineGamePanel.Visible = false;

                startLocalGamePanel = new Panel();
                startLocalGamePanel.Width = (int)(this.Width * 0.5);
                startLocalGamePanel.Height = (int)(this.Width * 0.3);
                startLocalGamePanel.Top = (this.ClientSize.Height - startLocalGamePanel.Height) / 2;
                startLocalGamePanel.Left = (this.ClientSize.Width - startLocalGamePanel.Width) / 2;
                startLocalGamePanel.BackColor = Color.FromArgb(180, Color.White);
                startLocalGamePanel.Parent = this;
                startLocalGamePanel.BorderStyle = BorderStyle.FixedSingle;

                Label header = new Label();
                header.Width = startLocalGamePanel.Width;
                header.Font = new Font(this.Font.FontFamily, 12);
                header.Text = Constants.CHOSE_LOCAL_GAME_PLAYER;
                header.Parent = startLocalGamePanel;
                header.BackColor = Color.FromArgb(150, Color.White);



                playersLB = new ListBox();
                playersLB.Width = (int)(startLocalGamePanel.Width * 0.9);
                playersLB.Height = (int)(startLocalGamePanel.Height * 0.6);
                playersLB.Top = header.Bottom + 10;
                playersLB.Left = (startLocalGamePanel.Width - playersLB.Width) / 2;
                playersLB.Parent = startLocalGamePanel;

                List<string> playerNames = new List<string>();
                foreach (DTO_Player player in players) playerNames.Add(player.FirstName + " " + player.LastName);
                playersLB.DataSource = playerNames;

                Button startGameBT = new Button();
                startGameBT.Text = Constants.playS;
                startGameBT.Top = playersLB.Bottom + 10;
                startGameBT.Left = (startLocalGamePanel.Width - startGameBT.Width) / 2;
                startGameBT.Parent = startLocalGamePanel;

                startGameBT.Click += (sender, EventArgs) => { OnLocalGamePlayClick(sender, EventArgs, players); };


            }
        }

        delegate void LoadAndDesignStartGamePanelCallBack();
        private void LoadAndDesignJoinOnlineGamePanel()
        {


            if (this.InvokeRequired)
            {
                LoadAndDesignStartGamePanelCallBack cback = new LoadAndDesignStartGamePanelCallBack(LoadAndDesignJoinOnlineGamePanel);
                this.Invoke(cback, new object[] { });
            }
            else
            {


                startOnlineGamePanel = new Panel();
                startOnlineGamePanel.Visible = false;
                startOnlineGamePanel.Width = (int)(this.Width * 0.5);
                startOnlineGamePanel.Height = (int)(this.Width * 0.2);
                startOnlineGamePanel.Top = (this.ClientSize.Height - startOnlineGamePanel.Height) / 2;
                startOnlineGamePanel.Left = (this.ClientSize.Width - startOnlineGamePanel.Width) / 2;

                startOnlineGamePanel.BackColor = Color.FromArgb(180, Color.White);
                startOnlineGamePanel.Parent = this;

                startOnlineGamePanel.BorderStyle = BorderStyle.FixedSingle;

                FlowLayoutPanel header = new FlowLayoutPanel();
                header.WrapContents = false;
                header.Width = startOnlineGamePanel.Width;
                header.Height = (int)(startOnlineGamePanel.Height * 0.3);
                header.Parent = startOnlineGamePanel;


                Label welcomeLabel = new Label();
                welcomeLabel.Width = (int)(startOnlineGamePanel.Width * 0.8);
                welcomeLabel.Height = (int)(startOnlineGamePanel.Height * 0.3);
                welcomeLabel.Font = new Font(this.Font.FontFamily, 10);
                welcomeLabel.Text = "Welcome " + GameController.loggedInAccount.NAME + Constants.PLEASE_ENTER_KEY;
                welcomeLabel.Parent = header;
                welcomeLabel.BackColor = Color.Transparent;

                PictureBox backBT = new PictureBox();
                backBT.Width = (int)(startOnlineGamePanel.Width * 0.2);
                backBT.Height = (int)(startOnlineGamePanel.Height);
                backBT.Image = (Image)(new Bitmap(Properties.Resources.backicon, new Size(header.Height, (int)(header.Height * 0.9))));
                backBT.Parent = header;
                backBT.BackColor = Color.Transparent;

                TextBox gameKeyTB = new TextBox();
                gameKeyTB.Width = (int)(startOnlineGamePanel.Width * 0.6);
                gameKeyTB.Left = (startOnlineGamePanel.Width - gameKeyTB.Width) / 2;
                gameKeyTB.Top = header.Bottom + gameKeyTB.Height;
                gameKeyTB.Text = "Game key";
                gameKeyTB.Parent = startOnlineGamePanel;


                Button PlayBT = new Button();
                PlayBT.Width = (int)(startOnlineGamePanel.Width * 0.5);
                PlayBT.Left = (startOnlineGamePanel.Width - PlayBT.Width) / 2;
                PlayBT.Top = gameKeyTB.Top + 10 + gameKeyTB.Height;
                PlayBT.Text = "Play";
                PlayBT.Parent = startOnlineGamePanel;

                joinGameErrorMessageLabel = new LoadingLabel("");
                joinGameErrorMessageLabel.BackColor = Color.Transparent;
                joinGameErrorMessageLabel.Width = startOnlineGamePanel.Width;
                joinGameErrorMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
                joinGameErrorMessageLabel.Left = (startOnlineGamePanel.Width - joinGameErrorMessageLabel.Width) / 2;
                joinGameErrorMessageLabel.Top = PlayBT.Bottom - 3;
                joinGameErrorMessageLabel.Parent = startOnlineGamePanel;
                joinGameErrorMessageLabel.ForeColor = Color.Black;
                startOnlineGamePanel.Visible = true;

                PlayBT.Click += (sender, EventArgs) => { onStartPlayOnlineClick(sender, EventArgs, gameKeyTB, joinGameErrorMessageLabel); };
                backBT.MouseClick += OnClickLoginLogoutMenu;

            }

        }

        //Application menu On click events
        private void OnClickPlayLocalGameMenu(object sender, MouseEventArgs e)
        {
            OnSwitchPanels();
            DataAcsess.OnLocalGameMenuClick();

        }
        private void OnClickRestoreGameMenu(object sender, MouseEventArgs e)
        {

            OnSwitchPanels();

            DataAcsess.GetGamesListForRestore();
        }
        private void OnClickLoginLogoutMenu(object sender, MouseEventArgs e)
        {
            OnSwitchPanels();


            if (applicationMenu.loginoutMenuItem.Text.Equals(Constants.logoutS))
            {
                
                startOnlineGamePanel.Visible = false;
                loginPanel.Visible = true;
                applicationMenu.joinGameMenuItem.Enabled = false;
                applicationMenu.restorePrevGames.Enabled = false;
                applicationMenu.loginoutMenuItem.Text = Constants.loginS;
                GameController.isPlaying = false;
                GameController.isThisPlayerTurn = false;
            }else
            {
                if (startOnlineGamePanel != null)
                {
                    startOnlineGamePanel.Visible = false;
                }
                loginPanel.Visible = true;
            }
            
        }
        private void OnClickPlayOnlineGameMenu(object sender, MouseEventArgs e)
        {
            OnSwitchPanels();

            if (startOnlineGamePanel != null)
            {
                if (gamePanel != null)
                {
                    gamePanel.Visible = false;
                }

                startOnlineGamePanel.Visible = true;
            }
            else
            {
                LoadAndDesignJoinOnlineGamePanel();
            }
        }



        // on click events
        void onLoginButtonClick(Object sender,EventArgs e, TextBox email, TextBox password, LoadingLabel messageLabel)
        {
            Regex emailCheck = new Regex(Constants.EMAIL_REGEX);
            messageLabel.Text = "";

            if (!emailCheck.IsMatch(email.Text))
            {
                ep.SetError(email,Constants.EMAIL_FORMAT);
            }else if (email.Text==String.Empty)
            {
                ep.SetError(email, Constants.EMAIL_EMTPY);
            }else if (password.Text==String.Empty)
            {
                ep.SetError(password, Constants.PASSWORD_EMTPY);
            }else
            {
                messageLabel.ForeColor = Color.Black;
                messageLabel.StartAnimation();
                (sender as Button).Enabled = false;
                ep.SetError(email, "");
                ep.SetError(password, "");
                ep.Clear();

                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => loginThread(email.Text, password.Text,messageLabel, (sender as Button))));  
            }
        }
        void OnRestoreGameClick(Object sender, EventArgs e)
        {
            int selectedRow = restoredGamesDGV.CurrentRow.Index;
            int gameID = Int32.Parse(restoredGamesDGV.Rows[selectedRow].Cells[0].Value.ToString());
            DataAcsess.OnRestoreGame(gameID);
        }



        // on click checkrs on board mouse events 
        private void OnCheckerMouseUp(object sender, MouseEventArgs e)
        {
            CheckerPB clickedCheckerPointer = (CheckerPB)sender;

            if (!GameController.isMouseAllowedToMove)
            {
                GameController.isMouseAllowedToMove = true;
                return;
            }

            // adjust checker image back on mouseup
            if (clickedCheckerPointer.checker.checkerType == Constants.blackChecker)
            {
                clickedCheckerPointer.Image = new Bitmap(Properties.Resources.checker_black, clickedCheckerPointer.Size);
            }
            else
            {
                clickedCheckerPointer.Image = new Bitmap(Properties.Resources.checker_white, clickedCheckerPointer.Size);
            }

            DataGridView.HitTestInfo hti = GameBoard.HitTest(clickedCheckerPointer.Left + clickedCheckerPointer.Width / 2, clickedCheckerPointer.Top + clickedCheckerPointer.Height / 2);

            // if there is option to eat checker and player did not move it ,return
            Point newCheckerPosition = new Point(hti.RowIndex, hti.ColumnIndex);
            Point moveToPositionForEat = GameController.isThereIsOptionToEat(GameController.thisPlayerType);
            // normal move there is not option to eat
            if (moveToPositionForEat.Equals(new Point(-1, -1)))
            {
                bool isCheckerPositionChanged = GameController.OnFinishCheckerMove(ref clickedCheckerPointer, newCheckerPosition);
                animationHolder.stopAnimation();
                updateMainPanelScreenShoot(clickedCheckerPointer);
                if (isCheckerPositionChanged) GameController.isThisPlayerTurn = false;

                if (GameController.gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER && isCheckerPositionChanged)
                {
                    GameController.PlayAsComputer();
                }

            }
            else
            {
                // this case there is option to eat and user did not chosed it .
                if (!newCheckerPosition.Equals(moveToPositionForEat))
                {
                    animationHolder.stopAnimation();
                    clickedCheckerPointer.moveToOldPosition();
                    return;

                }
                else
                {
                    // this case there is option to eat and user did chosed it .
                    bool isCheckerPositionChanged = GameController.OnFinishCheckerMove(ref clickedCheckerPointer, newCheckerPosition);
                    animationHolder.stopAnimation();
                    updateMainPanelScreenShoot(clickedCheckerPointer);
                    if (isCheckerPositionChanged) GameController.isThisPlayerTurn = false;

                    if (GameController.gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER && isCheckerPositionChanged)
                    {
                        GameController.PlayAsComputer();
                    }


                }
            }



        }
        private void OnCheckerMouseMove(object sender, MouseEventArgs e)
        {
            // pointer of moving checker
            CheckerPB clickedCheckerPointer = (CheckerPB)sender;

            if (!GameController.isMouseAllowedToMove)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {

                // calculate moved offset relative to start move position and frame position to screen
                int newCheckerLeft = MousePosition.X - this.Left - GameBoard.Left - clickedCheckerPointer.Width / 2;
                int newCheckerTop = MousePosition.Y - this.Top - GameBoard.Top - clickedCheckerPointer.Height;

                // move the checker by mouse game board limits (the border of gameboard)
                if (newCheckerLeft >= Constants.CheckerMoveOffsetOfBoardLimits && newCheckerLeft <= (GameBoard.Width - clickedCheckerPointer.Width) - Constants.CheckerMoveOffsetOfBoardLimits)
                {
                    clickedCheckerPointer.Left = newCheckerLeft;
                }

                if (newCheckerTop >= Constants.CheckerMoveOffsetOfBoardLimits && newCheckerTop <= (GameBoard.Height - clickedCheckerPointer.Height) - Constants.CheckerMoveOffsetOfBoardLimits)
                {
                    clickedCheckerPointer.Top = newCheckerTop;
                }


                DataGridView.HitTestInfo hti = GameBoard.HitTest(clickedCheckerPointer.Left + clickedCheckerPointer.Width / 2, clickedCheckerPointer.Top + clickedCheckerPointer.Height / 2);
                Point newPositionOfSelectedCheckerOnBoard = new Point(hti.RowIndex, hti.ColumnIndex);

                if (!currentPositionOfSelectedCheckerOnBoard.Equals(newPositionOfSelectedCheckerOnBoard))
                {
                    int gameBoardCellWidth = GameBoard.Columns[0].Width;
                    int gameBoardCellHeight = GameBoard.Rows[0].Height;

                    animationHolder.stopAnimation();
                    animationHolder.initAnimation(new Point(hti.ColumnIndex * gameBoardCellWidth, (hti.RowIndex * gameBoardCellHeight) + gameBoardCellHeight));
                    animationHolder.startAnimation();

                    currentPositionOfSelectedCheckerOnBoard = newPositionOfSelectedCheckerOnBoard;
                }
            }

        }
        private void OnCheckerMouseDown(object sender, MouseEventArgs e)
        {
            // get pointer to clicked checker
            CheckerPB clickedCheckerPointer = (CheckerPB)sender;

            // check if this player turn to play
            if (!GameController.isThisPlayerTurn || GameController.gameStatus != Constants.GAME_STATUS_PLAYING)
            {
                GameController.isMouseAllowedToMove = false;
                return;
            }


            // if this is not my checker type return
            if (clickedCheckerPointer.checker.checkerType != GameController.thisPlayerType)
            {
                GameController.isMouseAllowedToMove = false;
                return;
            }


            if (e.Button == MouseButtons.Left)
            {
                // change brightnace up on moving checkers 
                Image g = (Image)clickedCheckerPointer.Image;
                if (g == null)
                {
                    RepaintCheckerImage(ref clickedCheckerPointer);
                    g = (Image)clickedCheckerPointer.Image;
                }
                Bitmap m = AdjustImage(new Bitmap(g), 1.0f, 1.5f, 1.0f);
                clickedCheckerPointer.Image = m;

                updateMainPanelScreenShoot(clickedCheckerPointer);

                // save start moving mouse location
                MouseDownLocation = e.Location;

                //start animation around selected cell on board
                DataGridView.HitTestInfo hti = GameBoard.HitTest(clickedCheckerPointer.Left + clickedCheckerPointer.Width / 2, clickedCheckerPointer.Top + clickedCheckerPointer.Height / 2);
                int gameBoardCellWidth = GameBoard.Columns[0].Width;
                int gameBoardCellHeight = GameBoard.Rows[0].Height;
                animationHolder.initAnimation(new Point(hti.ColumnIndex * gameBoardCellWidth, (hti.RowIndex * gameBoardCellHeight) + gameBoardCellHeight));
                animationHolder.startAnimation();

                currentPositionOfSelectedCheckerOnBoard = new Point(hti.RowIndex, hti.ColumnIndex);
            }
        }


        // this method callled each time user switching betweing pages in application
        // manage the visability of different panels
        public void OnSwitchPanels()
        {
            if (GameController.gameType == Constants.GAME_TYPE_GAME_RESTORE)
            {
                gameRestoreAnimator.StopAnimation();
            }

            if (GameController.gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER && GameController.gameStatus != Constants.GAME_STATUS_END)
            {
                DataAcsess.UpdateGameStatus(GameController.currentPlayingGameID, Constants.GAME_STATUS_DISCONNECTED);

            }

            if (GameController.loggedInAccountPlayerPlaying != null && GameController.gameType == Constants.GAME_TYPE_PLAY_ONLINE)
            {
                DataAcsess.UnregisterClientOnServer();
            }

            if (startLocalGamePanel != null)
                startLocalGamePanel.Visible = false;

            if (startOnlineGamePanel != null)
                startOnlineGamePanel.Visible = false;

            if (restoreGamesPanel != null)
                restoreGamesPanel.Visible = false;

            if (gamePanel != null)
                gamePanel.Visible = false;

            if (playersLB != null)
                playersLB.Visible = false;

            if (restoredGamesDGV != null)
                restoredGamesDGV.Visible = false;
        }


        public void loginThread(string email,string password, LoadingLabel messageLabel,Button bt)
        {
            DTO_ACCOUNT loggedInAccount = DataAcsess.Login(email, password);

            if (loggedInAccount == null)
            {
                messageLabel.StopAnimation();

                bt.Invoke((MethodInvoker)delegate {
                    bt.Enabled = true;
                });

                messageLabel.setTextNoneUiThread(Constants.WRONG_LOGIN,Color.Red);
            }
            else
            {
                GameController.loggedInAccount = loggedInAccount;
                messageLabel.StopAnimation();
                messageLabel.setTextNoneUiThread("",Color.Black);

                bt.Invoke((MethodInvoker)delegate {
                    loginPanel.Visible = false;
                    bt.Enabled = true;
                });

                applicationMenu.Invoke((MethodInvoker)delegate {
                    applicationMenu.joinGameMenuItem.Enabled = true;
                    applicationMenu.restorePrevGames.Enabled = true;
                    applicationMenu.playLocalGameMenuItem.Enabled = true;
                    applicationMenu.loginoutMenuItem.Text = Constants.logoutS;
                });

                LoadAndDesignJoinOnlineGamePanel();
            }
        }
        private void OnLocalGamePlayClick(Object sender, EventArgs e,DTO_Player[] players)
        {
            GameController.loggedInAccountPlayerPlaying = players[playersLB.SelectedIndex];
            GameController.loggedInAccountPlayerPlaying.Type = Constants.whiteChecker;
            GameController.thisPlayerType = Constants.whiteChecker;
            GameController.gameStatus = Constants.GAME_STATUS_PLAYING;
            GameController.gameType = Constants.GAME_TYPE_GAME_VS_COMPUTER;

            DataAcsess.RegisterNewLocalGamePlay();
        }

        delegate void ShowAndStartLocalGamePlayCallBack();
        public void ShowAndStartLocalGamePlay()
        {
            if (this.InvokeRequired)
            {
                ShowAndStartLocalGamePlayCallBack cback = new ShowAndStartLocalGamePlayCallBack(ShowAndStartLocalGamePlay);
                this.Invoke(cback, new object[] {  });
            }
            else
            {
                startLocalGamePanel.Visible = false;
                GameController.gameType = Constants.GAME_TYPE_GAME_VS_COMPUTER;
                GameController.isPlaying = true;
                GameController.isThisPlayerTurn = true;
                startOnlineGamePanel.Visible = false;
                OnRegisterClientSucceed();
                GameController.opponents[0].Visible = true;
                GameController.opponents[0].setOpponentText(GameController.loggedInAccount.NAME, GameController.loggedInAccountPlayerPlaying.FirstName + " " + GameController.loggedInAccountPlayerPlaying.LastName);
                GameController.opponents[1].Visible = false;
            }
            
        }
        private void LoadMenu()
        {
            applicationMenu = new ApplicationMenu();
            applicationMenu.Dock = DockStyle.Top;
            applicationMenu.Parent = this;

            applicationMenu.loginoutMenuItem.MouseDown += OnClickLoginLogoutMenu;
            applicationMenu.playLocalGameMenuItem.MouseDown += OnClickPlayLocalGameMenu;
            applicationMenu.joinGameMenuItem.MouseDown += OnClickPlayOnlineGameMenu;
            applicationMenu.restorePrevGames.MouseDown += OnClickRestoreGameMenu;

            applicationMenu.aboutMenuItem.MouseDown += showAboutBox;

        }
        void onStartPlayOnlineClick(Object sender, EventArgs e,TextBox gameKeyTB,LoadingLabel messageLabel)
        {
            int gameID;

            try { 
            gameID = Int32.Parse(gameKeyTB.Text);
            }catch(Exception)
            {
                ep.SetError(gameKeyTB,Constants.GAME_KEY_NUMERICAL);
                return;
            }
            ep.SetError(gameKeyTB, "");
            ep.Clear();
            (sender as Button).Enabled = true;
            messageLabel.ForeColor = Color.Black;
            messageLabel.StartAnimation();
            GameController.gameType = Constants.GAME_TYPE_PLAY_ONLINE;
            GameController.gameStatus = Constants.GAME_STATUS_PLAYING;
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => StartOnlinePlayThread(gameID, (sender as Button), messageLabel)));
        }
        private void StartOnlinePlayThread(int gameID,Button playBT,LoadingLabel messageLB)
        {
           

            DTO_Player player = DataAcsess.FindGameAndReturnPlayerInfo(gameID,GameController.loggedInAccount.EMAIL);

            if (player == null)
            {

                playBT.Invoke((MethodInvoker)delegate {
                    playBT.Enabled = true;
                });
                    messageLB.Invoke((MethodInvoker)delegate {
                    messageLB.StopAnimation();
                    messageLB.setTextNoneUiThread("No found game assosiated with your account",Color.Red);
                });

            }
            else
            {
                GameController.loggedInAccountPlayerPlaying = player;
                GameController.thisPlayerType = player.Type;
                GameController.currentPlayingGameID = gameID;

                if (GameController.thisPlayerType == Constants.whiteChecker)
                {
                    GameController.isThisPlayerTurn = true;
                }

                messageLB.StopAnimation();
                messageLB.setTextNoneUiThread("", Color.Black);
                
                if (gamePanel == null)
                {
                    DataAcsess.registerClientOnServer();
                }
                else
                {
                    gamePanel.Invoke((MethodInvoker)delegate
                    {
                        DataAcsess.registerClientOnServer();
                    });
                }

            }
        }
        public void OnRegisterClientFaild()
        {
            joinGameErrorMessageLabel.StopAnimation();
            joinGameErrorMessageLabel.setTextNoneUiThread(Constants.ALLREDY_CONNECTED_TO_GAME,Color.Red);
        }

        delegate void OnRegisterClientSucceedBackCallBack();
        public void OnRegisterClientSucceed()
        {

            if (this.InvokeRequired)
            {
                OnRegisterClientSucceedBackCallBack cback = new OnRegisterClientSucceedBackCallBack(OnRegisterClientSucceed);
                this.Invoke(cback, new object[] { });
            }
            else
            {
                GameController.numberOfeatenCheckers = 0;
                GameController.numberOfeatenCheckersByOpponent = 0;

                joinGameErrorMessageLabel.StopAnimation();
                joinGameErrorMessageLabel.setTextNoneUiThread("", Color.Black);

                if (GameController.gameType==Constants.GAME_TYPE_PLAY_ONLINE) {

                    if (GameController.thisPlayerType == Constants.whiteChecker)
                    {
                        GameController.isThisPlayerTurn = true;
                    }   
                }

                if (gamePanel == null)
                {
                    LoadGamePanel();
                    
                }
                else
                {
                        RestGame();
                }

                if (GameController.gameType == Constants.GAME_TYPE_PLAY_ONLINE)
                {
                    loadingLabel.Visible = true;
                }
                else
                {
                    loadingLabel.Visible = false;
                }
            }
        }

        delegate void OnGameWonCallBack(DTO_Player wonPlayer);
        public void OnGameWon(DTO_Player wonPlayer)
        {
            if (this.InvokeRequired)
            {
                OnGameWonCallBack cback = new OnGameWonCallBack(OnGameWon);
                this.Invoke(cback, new object[] {wonPlayer });
            }
            else
            {
                GameController.gameStatus = Constants.GAME_STATUS_END;
                GameController.isThisPlayerTurn = false;
                GameController.isPlaying = false;
                loadingLabel.StopAnimation();
                loadingLabel.setTextNoneUiThread(Constants.GAME_WON1+ wonPlayer.TEAM_NAME + Constants.GAME_WON2, Color.Black);
                loadingLabel.Visible = true;

                if (GameController.gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER)
                {
                    DataAcsess.UpdateGameStatus(GameController.currentPlayingGameID, Constants.GAME_STATUS_END);
                }
            }
        }

        delegate void RestGameCallBack();
        private void RestGame()
        {
            if (this.InvokeRequired)
            {
                RestGameCallBack cback = new RestGameCallBack(RestGame);
                this.Invoke(cback, new object[] { });
            }else {
                GameController.clearCheckers();
                updateMainPanelScreenShoot(null);
                LoadGameBoardCheckers();
                LoadAndDesignAnimationHolderPanel();
                gamePanel.Visible = true;
                updateMainPanelScreenShoot(null);
                if (GameController.gameType == Constants.GAME_TYPE_PLAY_ONLINE)
                {
                    loadingLabel.Visible = true;
                }
                loadingLabel.setTextNoneUiThread(Constants.waitOpponent, Color.Black);
                loadingLabel.StartAnimation();
                startOnlineGamePanel.Visible = false;
            }
        }

        // when user logged in and starts game , load game panel and register client for gameplay.
        delegate void LoadGamePanelAndRegisterClientCallBack();
        private void LoadGamePanel()
        {

            if (this.InvokeRequired)
            {
                LoadGamePanelAndRegisterClientCallBack cback = new LoadGamePanelAndRegisterClientCallBack(LoadGamePanel);
                this.Invoke(cback, new object[] { });
            }
            else
            {

                if (GameController.gameType == Constants.GAME_TYPE_PLAY_ONLINE)
                {
                    startOnlineGamePanel.Visible = false;
                }
                LoadAndDesignGamePanel();
                DesignGameBoard();
                LoadGameBoardCheckers();
                DesignMainPanel();
                LoadAndDesignAnimationHolderPanel();
                LoadCurrentOpponentPanels();
                LoadLoadingLabel();
            }
        }
        // game panel holds all controls for game play, hidden when no game
        private void LoadAndDesignGamePanel()
        {
            gamePanel = new Panel();
            gamePanel.Parent = this;
            gamePanel.Width = this.Width;
            gamePanel.Height = this.Height;
            gamePanel.BackColor = Color.Transparent;
        }
        // fits form dimentions to screen size 
        private void DesignForm()
        {
            Size screenSize = Screen.PrimaryScreen.WorkingArea.Size;
            this.Width = screenSize.Width / 2;
            this.Height = (int)(screenSize.Height * 0.95);
            this.BackColor = Color.Black;
            this.BackgroundImage = Properties.Resources.bg;
        }
        // This panel is drawed on top of all other controls , it is used for make checker background transparent effect when moving the checkers.
        // This panel is the parent of each checker on the game board , where each checker background is set to transparent.
        // When ever user moves checker , screen shoot of gameboard + checkers is taken and MainPanel holds this image as background.
        private void DesignMainPanel()
        {
            
            mainPanelPB = new PictureBox();
            mainPanelPB.Parent = GameBoard;
            mainPanelPB.Width = GameBoard.Width;
            mainPanelPB.Height = GameBoard.Height;
            mainPanelPB.BackColor = Color.Transparent;

            

        }
        // Set settings to Game Board (DataGrideView)where all game happens and checkers are sits on 
        private void DesignGameBoard()
        {
            GameBoard = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(GameBoard)).BeginInit();
            this.SuspendLayout();

            GameBoard.Parent = gamePanel;

            GameBoard.Width = (int)(this.Width * 0.7);

            // create columens/rows 8*4
            gameBoardGrid = new List<GameBoardRow>();
            GameBoardRow gameBoardOdd = new GameBoardRow(Constants.oddRow);
            GameBoardRow gameBoardEven = new GameBoardRow(Constants.evenRow);

            for (var i = 0; i < Constants.NumberOfGameBoardRows; i++)
            {
                if (i % 2 == 0) gameBoardGrid.Add(gameBoardOdd);
                else gameBoardGrid.Add(gameBoardEven);

            }
            GameBoard.DataSource = gameBoardGrid;

            // hide headers
            GameBoard.ColumnHeadersVisible = false; // You could turn this back on if you wanted, but this hides the headers that would say, "Cell1, Cell2...."
            GameBoard.RowHeadersVisible = false;
            // GameBoard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // fit cells
            GameBoard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // get the height of title
            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
            int titleHeight = screenRectangle.Top - this.Top;

            // fit rows height to gameboard height
            int rowHeight = (int)((this.Height*0.9 - (GameBoard.ColumnHeadersHeight + titleHeight)) / Constants.NumberOfGameBoardRows);
            foreach (DataGridViewRow row in GameBoard.Rows)
            {
                row.Height = rowHeight; //your calculation based on the screen height
                row.ReadOnly = true;
            }
            GameBoard.Height = rowHeight * Constants.NumberOfGameBoardRows;

            // hide scrollbars
            GameBoard.ScrollBars = ScrollBars.None;
            // disable resizing rows/cols
            GameBoard.AllowUserToResizeRows = false;
            GameBoard.AllowUserToResizeColumns = false;

            // hide cells border cols
            GameBoard.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            GameBoard.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;


            // hide cells border rows
            GameBoard.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            GameBoard.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;

            // center gameboard inside form
              GameBoard.Top = (int)(this.Height*0.1);
            //GameBoard.Top = (int)(this.ClientSize.Height*0.1);
            GameBoard.Left = (this.ClientSize.Width - GameBoard.Width) / 2;


        }
        private void LoadCurrentOpponentPanels()
        {
            Size opponentSize = new Size((int)(this.Width * 0.12), (int)(this.Width * 0.1));

            OpponentPB whiteOponent = new OpponentPB(Constants.whiteChecker, opponentSize);
            whiteOponent.Parent = gamePanel;
            whiteOponent.Left = GameBoard.Right + (int)(whiteOponent.Width * 0.1);
            whiteOponent.Top = GameBoard.Bottom - whiteOponent.Height;
            whiteOponent.Visible = false;
            GameController.opponents[0] = whiteOponent;

            OpponentPB blackOponent = new OpponentPB(Constants.blackChecker, opponentSize);
            blackOponent.Parent = gamePanel;
            blackOponent.Left = GameBoard.Left - (int)(blackOponent.Width * 1.1);
            blackOponent.Top = GameBoard.Top;
            blackOponent.Visible = false;
            GameController.opponents[1] = blackOponent;

        }
        private void LoadLoadingLabel()
        {
            loadingLabel = new LoadingLabel(Constants.waitOpponent);
            loadingLabel.Left = GameBoard.Left;
            loadingLabel.Top = applicationMenu.Height+5;
            loadingLabel.Font = new Font(this.Font.FontFamily, 16);
            loadingLabel.Width = GameBoard.Width;
            loadingLabel.Height = (int)(this.Height * 0.05);
            loadingLabel.Parent = gamePanel;
            loadingLabel.StartAnimation();
            loadingLabel.Visible = false;
        }
        // create and add cheackers to gameboard,top board player checkers are white bottom player checkers are black.
        private void LoadGameBoardCheckers()
        {
            int cellWidth = GameBoard.Columns[0].Width;
            int cellHeight = GameBoard.Rows[0].Height;
            Size gameBoardCellSize = new Size(cellWidth, cellHeight);
            Size checkerSize = new Size((int)(cellHeight*0.8), (int)(cellHeight * 0.8));

            // create checkers for top player (first 8 checkers at top of the desk)
            int checkerPositionX = (cellWidth / 2 - checkerSize.Width / 2)+ cellWidth;
            int checkerPositionY = cellHeight / 2 - checkerSize.Height / 2;
            createPlayerCheckers(Constants.blackChecker, gameBoardCellSize, checkerSize, new Point(checkerPositionX, checkerPositionY));

            // create checkers for bottom player (last 8 checkers at bottom of the desk)
            checkerPositionY = cellHeight / 2 - checkerSize.Height / 2+(Constants.NumberOfGameBoardRows-2)* (cellHeight);
            createPlayerCheckers(Constants.whiteChecker, gameBoardCellSize, checkerSize, new Point(checkerPositionX, checkerPositionY));
        }
        private void createPlayerCheckers(int type,Size cellSize,Size checkerSize,Point startOffset)
        {
            for (int i = 0; i < Constants.NumberOfGameBoardCols; i++)
            {
                Point chekerPosition = new Point(startOffset.X, startOffset.Y);
                CheckerPB ch = createChecker(type+"_"+i, type, checkerSize, chekerPosition, cellSize);
              
                
                GameController.checkers.Add(ch.checker.ID,ch);
                startOffset.X += cellSize.Width*2;

                if (i == 1)
                {
                    startOffset.X = cellSize.Width / 2 - checkerSize.Width / 2;
                    startOffset.Y += cellSize.Height;
                }
            }
        }
        private CheckerPB createChecker(String ID,int type, Size size,Point position,Size gameBoardCellSize)
        {
            CheckerPB ch = new CheckerPB(ID, type, size, position, gameBoardCellSize);
            DataGridView.HitTestInfo hti = GameBoard.HitTest(position.X, position.Y);
            ch.checker.oldPositionOnBord = new Point(hti.RowIndex, hti.ColumnIndex);
            ch.Parent = GameBoard;
            ch.MouseDown += OnCheckerMouseDown;
            ch.MouseMove += OnCheckerMouseMove;
            ch.MouseUp += OnCheckerMouseUp;
            return ch;
        }
        private void RepaintCheckerImage(ref CheckerPB checker)
        {
            int cellWidth = GameBoard.Columns[0].Width;
            int cellHeight = GameBoard.Rows[0].Height;
            Size gameBoardCellSize = new Size(cellWidth, cellHeight);
            Size checkerSize = new Size((int)(cellHeight * 0.8), (int)(cellHeight * 0.8));

            if (checker.checker.checkerType == Constants.whiteChecker)
            {
                checker.Image = (Image)(new Bitmap(Properties.Resources.checker_white, checkerSize));
            }
            else
            {
                checker.Image = (Image)(new Bitmap(Properties.Resources.checker_black, checkerSize));
            }
        }
        // used to change image colors for example when user selects checker the checker image is brighter .
        public Bitmap AdjustImage(Bitmap originalImage, float brightness, float contrast, float gamma)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width,originalImage.Height);

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
            new float[] {contrast, 0, 0, 0, 0}, // scale red
            new float[] {0, contrast, 0, 0, 0}, // scale green
            new float[] {0, 0, contrast, 0, 0}, // scale blue
            new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
            new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, imageAttributes);

            return adjustedImage;
        }
        private void LoadAndDesignAnimationHolderPanel()
        {
            if (animationHolder == null) { 
            animationHolder = new AnimationHolder(new Size(GameBoard.Columns[0].Width, GameBoard.Rows[0].Height));
            animationHolder.Width = mainPanelPB.Width;
            animationHolder.Height = mainPanelPB.Height;
            animationHolder.Parent = mainPanelPB;
            }

            foreach (KeyValuePair<string, CheckerPB> pair in GameController.checkers)
            {
                pair.Value.Parent = animationHolder;
            }
        }

        delegate void updateMainPanelScreenShootCallback(CheckerPB clickedChecker);
        // on start move checker or eat other cheacher , update the mainpanel that holds screenShoot of game for transparent effect 
        // explained on DesignMainPanel();
        public void updateMainPanelScreenShoot(CheckerPB clickedChecker)
        {


            if (GameBoard.InvokeRequired)
            {
                updateMainPanelScreenShootCallback fixPositionCallBack = new updateMainPanelScreenShootCallback(updateMainPanelScreenShoot);
                this.Invoke(fixPositionCallBack, new object[] { clickedChecker });
            }
            else
            {

                // remove old mainPanel image
                if (mainPanelPB.Image != null)
                {
                    mainPanelPB.Image.Dispose();
                    mainPanelPB.Image = null;
                }


                if (animationHolder.Image != null)
                {
                    animationHolder.Image.Dispose();
                    animationHolder.Image = null;
                }


                Bitmap checkerImageTemp =null;
                if (clickedChecker != null) { 
                // save moving checker image as temp 
                checkerImageTemp = new Bitmap(clickedChecker.Image);
                
                // remove this moving checker image
                clickedChecker.Image.Dispose();
                clickedChecker.Image = null;
                }

                //take screenshoot of all board w/o the moving checker.
                Bitmap bmp = new Bitmap(GameBoard.Width, GameBoard.Height);
                GameBoard.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                mainPanelPB.Image = bmp;
                mainPanelPB.BringToFront();

                // return this moving checker his image
                if (clickedChecker != null) { 
                clickedChecker.Image = checkerImageTemp;
                clickedChecker.BringToFront();
                }
            }

        }
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            CheckState exerciseCheckboxState = applicationMenu.uiExerciseCheckBox.CheckState;
            CheckState exerciseFixCheckboxState = applicationMenu.uiExerciseCheckBoxFix.CheckState;

            if (exerciseCheckboxState == CheckState.Checked && exerciseFixCheckboxState == CheckState.Checked)
            {
                DataAcsess.autoReset.WaitOne();
            }

            if (GameController.loggedInAccountPlayerPlaying != null && GameController.gameType == Constants.GAME_TYPE_PLAY_ONLINE)
            {
                DataAcsess.UnregisterClientOnServer();
            }

        }

        delegate void ShowRestoredGameCallback(DTO_RESTORED_GAME restoredGame);
        public void ShowRestoredGame(DTO_RESTORED_GAME restoredGame)
        {
            if (this.InvokeRequired)
            {
                ShowRestoredGameCallback fixPositionCallBack = new ShowRestoredGameCallback(ShowRestoredGame);
                this.Invoke(fixPositionCallBack, new object[] { restoredGame });
            }else
            {
                restoreGamesPanel.Visible = false;
                if (loadingLabel != null) { 
                loadingLabel.Visible = false;
                }
                OnRegisterClientSucceed();
                LoadAndDesignAnimationHolderPanel();
                gameRestoreAnimator = new GameRestoreAnimator(restoredGame);
                LoadCurrentOpponentPanels();
                GameController.opponents[0].setOpponentText(restoredGame.players[0].TEAM_NAME, restoredGame.players[0].FirstName + " " + restoredGame.players[0].LastName);
                GameController.opponents[1].setOpponentText(restoredGame.players[1].TEAM_NAME, restoredGame.players[1].FirstName + " " + restoredGame.players[1].LastName);
                GameController.opponents[0].Visible = true;
                GameController.opponents[1].Visible = false;
                gameRestoreAnimator.StartAnimation();
            }
        }

        // returns single cell size inside game board table
        public Point getCellSize()
        {
            int gameBoardCellWidth = GameBoard.Columns[0].Width;
            int gameBoardCellHeight = GameBoard.Rows[0].Height;
            return new Point(gameBoardCellWidth, gameBoardCellHeight);
        }
        public void showAboutBox(object sender, MouseEventArgs e)
        {
            aboutBox = new AboutBox();
            aboutBox.Show();
        }
    }
}
