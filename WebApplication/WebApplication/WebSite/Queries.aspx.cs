using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.App_Code;
using WebApplication.ServiceRef;

namespace WebApplication.WebSite
{
    public partial class Queries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { 
               loadAllPlayersDDL();
               loadAllGamesDDL();
            }
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            // in case user not logged in and somehow got to game page, redirect him.
            if (Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] == null)
            {
                Server.Transfer("../Default.aspx", true);
                return;
            }

            if (Session["tableData"] != null)
            {
                tableGV.DataSource = Session["tableData"];
                tableGV.DataBind();
                try
                {
                    HideColumns();
                }
                catch (Exception){ }

            }

            hideDropDownPlayersListsFirstItem();
            hideDropDownGamesListsFirstItem();
        }
        private void loadAllPlayersDDL()
        {
            DTO_Player[] players = WebSiteManager.GetAllPlayers();
            List<string> playerNames = new List<string>();
            playerNames.Add("Select Player");
            Session["AllPlayers"] = players;

            foreach (DTO_Player pl in players)
            {
                playerNames.Add(pl.FirstName+" "+pl.LastName);
            }

            PlayerList.DataSource = playerNames;
            PlayerList.DataBind();
        }
        private void loadAllGamesDDL()
        {
            DTO_GamePlay[] games = WebSiteManager.GetAllGames();
            List<string> gameNames = new List<string>();
            gameNames.Add("Select Game");
            Session["AllGames"] = games;

            foreach (DTO_GamePlay gp in games)
            {
                gameNames.Add(gp.gameName);
            }

            GamesList.DataSource = gameNames;
            GamesList.DataBind();
        }
        private void hideDropDownPlayersListsFirstItem()
        {
            try
            {
                PlayerList.Items[0].Attributes["style"] = "display:none";
            }
            catch (Exception) { }
        }
        private void hideDropDownGamesListsFirstItem()
        {
            try
            {
                GamesList.Items[0].Attributes["style"] = "display:none";
            }
            catch (Exception) { }
        }
        private void HideColumns()
        {
            //hide columns wich user shouldent see,used for internal logic
            int colIndex = WebSiteManager.GetColumnIndexByName(tableGV, "type");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref tableGV, colIndex);
            }

            colIndex = WebSiteManager.GetColumnIndexByName(tableGV, "hostPlayerAccountEmail");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref tableGV, colIndex);
            }

            colIndex = WebSiteManager.GetColumnIndexByName(tableGV, "isDeleted");
            if (colIndex != -1)
            {
                WebSiteManager.hideColumnByIndex(ref tableGV, colIndex);
            }
        }
     
        protected void ResultTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.Header)
            {
                // Hiding the Select Button Cell in Header Row.
                e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "none");
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Hiding the Select Button Cells showing for each Data Row. 
                e.Row.Cells[0].Style.Add(HtmlTextWriterStyle.Display, "none");
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
        }
        protected void OnClickShowAllPlayersInformation(object sender, EventArgs e)
        {
            DTO_Player[] players = WebSiteManager.GetAllPlayers();
            Session["tableData"] = players;
        }
        protected void OnClickGetAllGames(object sender, EventArgs e)
        {
            DTO_GamePlay[] games = WebSiteManager.GetAllGames();
            Session["tableData"] = games;
        }
        protected void OnClickGetPlayerNGames(object sender, EventArgs e)
        {
            List<DTO_PLAYER_GAMES> playerNGames = WebSiteManager.GetForEachPlayerNGames();
            Session["tableData"] = playerNGames;
        }
        protected void OnSelectedIndexChangeShowAllPlayerPlayedGames(object sender, EventArgs e)
        {
            DTO_GamePlay[] gamesByID = WebSiteManager.GetGamesByPlayerID(((DTO_Player[])Session["AllPlayers"])[PlayerList.SelectedIndex - 1].Id);
            Session["tableData"] = gamesByID;
            tableGV.DataSource = gamesByID;
            tableGV.DataBind();
            tableGridViewPanel.Update();
            try
            {
                HideColumns();
            }
            catch (Exception) { }
        }
        protected void OnSelectedIndexChangeShowPlayerForGame(object sender, EventArgs e)
        {
            DTO_Player[] playersByGameID = WebSiteManager.GetPlayersByGameID(((DTO_GamePlay[])Session["AllGames"])[GamesList.SelectedIndex - 1].Key);
            Session["tableData"] = playersByGameID;
            tableGV.DataSource = playersByGameID;
            tableGV.DataBind();
            tableGridViewPanel.Update();
            try
            {
                HideColumns();
            }
            catch (Exception) { }
        }
    }

}