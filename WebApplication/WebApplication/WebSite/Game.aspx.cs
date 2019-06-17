using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.App_Code;
using WebApplication.ServiceRef;

namespace WebApplication.WebSite
{
    public partial class Game : Page
    {
        protected override void OnInitComplete(EventArgs e)
        {
            // in case user not logged in and somehow got to game page, redirect him.
            if (Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] == null)
            {
                Server.Transfer("../Default.aspx", true);
                return;
            }

            if (!IsPostBack)
            {
                loadLoggedInUserPlayers();
            }

            // load the wating games from db and bind to girdview table
            loadWatingGames();
            // first time page loades we color the tabel for player at index 0
            ColorGamesTableRows(0);
            gamesGridViewPanel.Update();
            HideColumns();
            RenameColumns();
        }

        //renames columns inside binded table with data to more readable text for user
        private void RenameColumns()
        {
            //hide columns wich user shouldent see,used for internal logic
            int colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "createDate");
            if (colIndex != -1)
            {
                WebSiteManager.renameColumn(ref gamesGV,colIndex,"Create Date");
            }

            colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "gameName");
            if (colIndex != -1)
            {
                WebSiteManager.renameColumn(ref gamesGV, colIndex, "Game Name");
            }

            colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "hostPlayerName");
            if (colIndex != -1)
            {
                WebSiteManager.renameColumn(ref gamesGV, colIndex, "Hosting Player");
            }

            colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "joinPlayerName");
            if (colIndex != -1)
            {
                WebSiteManager.renameColumn(ref gamesGV, colIndex, "Joined Player");
            }
        }

        //hides columns wich user shouldent see,used for internal logic
        private void HideColumns()
        {
            int colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "hostPlayerAccountEmail");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref gamesGV, colIndex);
            }

            colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "hostPlayerID");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref gamesGV, colIndex);
            }

            colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "joinedPlayerID");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref gamesGV, colIndex);
            }

            colIndex = WebSiteManager.GetColumnIndexByName(gamesGV, "Status");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref gamesGV, colIndex);
            }
        }
        protected void GamePlayes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                // Hiding the Select Button Cell in Header Row.
                e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Hiding the Select Button Cells showing for each Data Row. 
                e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "none");

                // Attaching one onclick event for the entire row, so that it will
                // fire SelectedIndexChanged, while we click anywhere on the row.
                e.Row.Attributes["onclick"] =
                  ClientScript.GetPostBackClientHyperlink(this.gamesGV, "Select$" + e.Row.RowIndex);
            }

            // rename table headers and align center
            //if (e.Row.RowType == DataControlRowType.Header)
            //{
            //    e.Row.Cells[2].Text = "Started At ";
            //    e.Row.Cells[3].Text = "Game Name";
            //    e.Row.Cells[6].Text = "Hosting Player";

            //}
      
        }

        public void ColorGamesTableRows(int selectedIndexPlayerForJoinID)
        {

            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION];
            int selectedPlayerForJoinID = loggedInAccount.players[selectedIndexPlayerForJoinID].Id;

            foreach (GridViewRow r in gamesGV.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    // IDS From row in table
                    int hostingGamePlayerID = ((DTO_GamePlay[])Session[WebSiteManager.WATING_GAMES_LIST_SESSION])[r.RowIndex].hostPlayerID;
                    Nullable<int> joinedGamePlayerID = ((DTO_GamePlay[])Session[WebSiteManager.WATING_GAMES_LIST_SESSION])[r.RowIndex].joinedPlayerID;

                    // avoid comparing int to null (selectedPlayerForJoinID == joinedGamePlayerID)
                    if (joinedGamePlayerID == null) { joinedGamePlayerID = -1; }

                    if (hostingGamePlayerID!= -1 && joinedGamePlayerID!=-1)
                    {
                        // this case means the game started
                        r.BackColor = ColorTranslator.FromHtml(WebSiteManager.GAME_STATUS_PLAYING_COLOR);
                    }
                    else if(selectedPlayerForJoinID == hostingGamePlayerID || selectedPlayerForJoinID == joinedGamePlayerID)
                    {
                        r.BackColor = ColorTranslator.FromHtml(WebSiteManager.GAME_STATUS_REGISTARED_COLOR);
                    }
                    else
                    {
                        
                        r.BackColor = ColorTranslator.FromHtml(WebSiteManager.GAME_STATUS_WATING_COLOR);
                    }
                }
            }
        }

        public bool isPlayerExistInArrayByID(DTO_Player[] players,int playerID)
        {
            foreach(DTO_Player player in players)
            {
                if (player.Id == playerID)
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow r in gamesGV.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    r.Attributes["onmouseover"] = "this.style.color='green';this.style.cursor='pointer';";
                    r.ToolTip = "Click to select row";
                    r.Attributes["onclick"] = this.Page.ClientScript.GetPostBackClientHyperlink(this.gamesGV, "Select$" + r.RowIndex, true);

                    //try
                    //{
                        if (r.RowIndex != gamesGV.SelectedIndex)
                        {
                            r.Attributes["onmouseout"] = "this.style.color='black';";
                        }
                    //}
                    //catch (Exception) { }


                }
            }

            base.Render(writer);
        }

        protected void OnGameSelected(object sender, EventArgs e)
        {

            // clear error if was
            GeneralError.Text = "";
            UpdatePanelGeneralError.Update();

            foreach (GridViewRow row in gamesGV.Rows)
            {
                if (row.RowIndex == gamesGV.SelectedIndex)
                {
                    row.ForeColor = Color.Green;
                }
                else
                {
                    row.ForeColor = Color.Black;
                }
            }

        }
        private void loadWatingGames()
        {
            DTO_GamePlay[] watingGames = WebSiteManager.GetWatingGames();

            // store wating games
            Session[WebSiteManager.WATING_GAMES_LIST_SESSION] = watingGames;

            gamesGV.DataSource = watingGames;
            gamesGV.DataBind();
        }

        // get current logged in account player names for and load them into drop drop-down-list 
        private void loadLoggedInUserPlayers()
        {
            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]);
            DTO_Player[] loggedInAccountPlayers = loggedInAccount.players;

            List<ListItem> loggedInAccountPlayersNames = new List<ListItem>();

            for (int i = 0; i < loggedInAccountPlayers.Length; i++)
            {
                loggedInAccountPlayersNames.Add(new ListItem(loggedInAccountPlayers[i].FirstName + " " + loggedInAccountPlayers[i].LastName));
            }

            PlayerList.DataSource = loggedInAccountPlayersNames;
            PlayerList.DataBind();

            PlayerList2.DataSource = loggedInAccountPlayersNames;
            PlayerList2.DataBind();

        }

        protected void onJoiningPlayerSelectedIndexChanges(object sender,EventArgs e)
        {
            ColorGamesTableRows(PlayerList2.SelectedIndex);
        }

        protected void OnStartGameClick(object sender, EventArgs e)
        {

            if (isPlayerHostingOrJoinedGame(((DTO_ACCOUNT)Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]).players[PlayerList.SelectedIndex].Id))
            {
                GeneralErrorStartGame.ForeColor = Color.Red;
                GeneralErrorStartGame.Text = "You Allready registared to game";
                return;
            }

            try
            {
                DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]);
                DTO_Player hostGamePlayer = loggedInAccount.players[PlayerList.SelectedIndex];
                DTO_GamePlay game = new DTO_GamePlay {gameName=GameNameTB.Text,hostPlayerID=hostGamePlayer.Id,createDate=DateTime.Now.ToLocalTime() };
                int gameKey = WebSiteManager.AddGame(game);
                
                // load the wating games from db and bind to girdview table
                loadWatingGames();
                // first time page loades we color the tabel for player at index 0
                ColorGamesTableRows(PlayerList2.SelectedIndex);
                gamesGridViewPanel.Update();

                GeneralErrorStartGame.ForeColor = Color.Green;
                GeneralErrorStartGame.Text = "Game successfully registared, Key : " + gameKey;
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(data);
                    }
                }
            }
        }

        protected void OnJoinGameClick(object sender, EventArgs e)
        {
            // rest start game messages if exist
            GeneralErrorStartGame.Text = "";
            // set join game messages color
            GeneralError.ForeColor = Color.Red;

            // get joining player id
            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]);
            DTO_Player joiningPlayer = loggedInAccount.players[PlayerList2.SelectedIndex];

            DTO_GamePlay selectedGameInTable;
            // check if there is game selected
            if (gamesGV.SelectedRow == null)
            {
                GeneralError.Text = "Please Select Game";
                return;
            }else
            {
                selectedGameInTable = ((DTO_GamePlay[])Session[WebSiteManager.WATING_GAMES_LIST_SESSION])[gamesGV.SelectedRow.RowIndex];
            }


            //validate that user not hosting game allready or not joined to other game
            if (isPlayerHostingOrJoinedGame(joiningPlayer.Id))
            {
                GeneralError.Text = "You allready registarted to game";
            }
            // check if game is full 
            else if (selectedGameInTable.joinedPlayerID!=null)
            {
                GeneralError.Text = "The Game is full";
            }
            // check that selected game host player is not joining player (we wont join to ourself game)
            else if (joiningPlayer.Id== selectedGameInTable.hostPlayerID)
            {
                GeneralError.Text = "Cannot join to own game";
            }
            // check that joinng player not playing vs own group of players
            else if (joiningPlayer.AccountEmail == selectedGameInTable.hostPlayerAccountEmail)
            {
                GeneralError.Text = "This game statred by your team members,Cannot play vs your own team";
            }
            else
            { 
            // updated database games table with joined player to this game
            int gameKey = WebSiteManager.JoinGame(selectedGameInTable.hostPlayerID.ToString(), joiningPlayer.Id.ToString());

            loadWatingGames();
            // first time page loades we color the tabel for player at index 0
            ColorGamesTableRows(PlayerList2.SelectedIndex);
            gamesGridViewPanel.Update();

            GeneralError.ForeColor = Color.Green;
            GeneralError.Text = "Successfully joined game, Key : " + gameKey;
            }
        }

        private bool isPlayerHostingOrJoinedGame(int playerID)
        {
            foreach (DTO_GamePlay gp in (DTO_GamePlay[])(Session[WebSiteManager.WATING_GAMES_LIST_SESSION]))
            {
                if(gp.hostPlayerID==playerID || gp.joinedPlayerID == playerID)
                {
                    return true;
                }
            }

            return false;
        }

        protected void OnCloseGameClick(object sender, EventArgs e)
        {

            //rest start game message if exists
            GeneralErrorStartGame.Text = "";

            DTO_GamePlay selectedGame = ((DTO_GamePlay[])Session[WebSiteManager.WATING_GAMES_LIST_SESSION])[gamesGV.SelectedRow.RowIndex];
            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]);
            DTO_Player joiningPlayer = loggedInAccount.players[PlayerList2.SelectedIndex];


            if (selectedGame.hostPlayerID != joiningPlayer.Id)
            {
                GeneralError.ForeColor = Color.Red;
                GeneralError.Text = "You are not the owner of this game";
            }
            else { 
                int gameToDeleteID = selectedGame.Key;
                WebSiteManager.RemoveGames(new List<int> { gameToDeleteID });

                loadWatingGames();
                // first time page loades we color the tabel for player at index 0
                ColorGamesTableRows(PlayerList2.SelectedIndex);
                // gamesGridViewPanel.Update();

                
            }
        }


        protected void OnExistGameClick(object sender, EventArgs e)
        {
            //rest start game message if exists
            GeneralErrorStartGame.Text = "";

            DTO_GamePlay selectedGame = ((DTO_GamePlay[])Session[WebSiteManager.WATING_GAMES_LIST_SESSION])[gamesGV.SelectedRow.RowIndex];
            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]);
            DTO_Player joiningPlayer = loggedInAccount.players[PlayerList2.SelectedIndex];


            if (selectedGame.hostPlayerID == joiningPlayer.Id)
            {
                GeneralError.ForeColor = Color.Red;
                GeneralError.Text = "You are owner of this game,You allowed to close the game";
            }
            else if (selectedGame.joinedPlayerID!= joiningPlayer.Id)
            {
                GeneralError.ForeColor = Color.Red;
                GeneralError.Text = "You are not player in this game";
            }
            else
            {
                WebSiteManager.ExistGame(selectedGame.Key);
                loadWatingGames();
                // first time page loades we color the tabel for player at index 0
                ColorGamesTableRows(PlayerList2.SelectedIndex);
                gamesGridViewPanel.Update();
            }
        }

    }
}