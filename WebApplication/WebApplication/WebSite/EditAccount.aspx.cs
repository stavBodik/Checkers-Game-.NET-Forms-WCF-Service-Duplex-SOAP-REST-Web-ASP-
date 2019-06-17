using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.App_Code;
using WebApplication.ServiceRef;

namespace WebApplication.WebSite
{
    public partial class EditAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // in case user not logged in and somehow got to game page, redirect him.
            if (Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] == null)
            {
                Server.Transfer("../Default.aspx", true);
                return;
            }

            if (!IsPostBack)
            {
                loadAccountPlayersLB(WebSiteManager.GetPlayers(((DTO_ACCOUNT)Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]).EMAIL).ToList());
                loadAccountGamesLB(WebSiteManager.GetAllGamesByAccountEmail(((DTO_ACCOUNT)Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]).EMAIL).ToList());
                Session[WebSiteManager.PLAYERS_4_DELETE_SESSION] = new List<int>();
                Session[WebSiteManager.GAMES_4_DELETE_SESSION] = new List<int>();
            }

        }
        private void loadAccountPlayersLB(List<DTO_Player> players)
        {
            Session["AllAccountPlayers"] = players;

            List<string> playerNames = new List<string>();
            foreach (DTO_Player pl in players)
            {
                playerNames.Add(pl.FirstName + " " + pl.LastName);
            }

            PlayersLV.DataSource = playerNames;
            PlayersLV.DataBind();
        }

        private void loadAccountGamesLB(List<DTO_GamePlay> games)
        {
            Session["AllAccountGames"] = games;

            List<string> gamesInfo = new List<string>();
            foreach (DTO_GamePlay gp in games)
            {
                string gameVS = gp.joinPlayerName;
                if (gameVS==null) gameVS = "None";
                gamesInfo.Add(gp.gameName+" "+gp.hostPlayerName+" VS "+ gameVS+" At "+gp.createDate);
            }

            GamesLV.DataSource = gamesInfo;
            GamesLV.DataBind();
        }

        protected void OnRemovePlayer(object sender, EventArgs e)
        {
            List<DTO_Player> players = (List<DTO_Player>)Session["AllAccountPlayers"];
            List<int> players4delete = (List<int>)Session[WebSiteManager.PLAYERS_4_DELETE_SESSION];

            if (PlayersLV.SelectedIndex<0) return;
            players4delete.Add(players[PlayersLV.SelectedIndex].Id);

            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION]);
            // find logged in account player and delete it from session memory
            loggedInAccount.players = loggedInAccount.players.Where(p => p.Id != players[PlayersLV.SelectedIndex].Id).ToArray();
            Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] = loggedInAccount;


            players.RemoveAt(PlayersLV.SelectedIndex);
            loadAccountPlayersLB(players);
            Session[WebSiteManager.PLAYERS_4_DELETE_SESSION] = players4delete;

           
        }

        protected void OnRemoveGame(object sender, EventArgs e)
        {
            List<DTO_GamePlay> games = (List<DTO_GamePlay>)Session["AllAccountGames"];
            List<int> games4delete = (List<int>)Session[WebSiteManager.GAMES_4_DELETE_SESSION];

            if (GamesLV.SelectedIndex<0) return;
            games4delete.Add(games[GamesLV.SelectedIndex].Key);
            games.RemoveAt(GamesLV.SelectedIndex);
            loadAccountGamesLB(games);
            Session[WebSiteManager.GAMES_4_DELETE_SESSION] = games4delete;

        }

        protected void OnSaveClick(object sender, EventArgs e)
        {
            DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION];
            List<DTO_Player> players = (List<DTO_Player>)Session["AllAccountPlayers"];

            if (players.Count() == 0)
            {
                ErrorMessage.Text = "You Must leav at least one player,please refresh the page and start over";
                return;

            }

            string oldPassword = WebSiteManager.CreateHash(OldPassword.Text, WebSiteManager.PASSWORD_HASH_SALT);
            if (IsValid && oldPassword.Equals(loggedInAccount.PASSWORD))
            {
                loggedInAccount.EMAIL = Email.Text;
                loggedInAccount.PASSWORD = WebSiteManager.CreateHash(NewPassword.Text, WebSiteManager.PASSWORD_HASH_SALT);
                loggedInAccount.NAME = nickname.Text;
                Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] = loggedInAccount;
                WebSiteManager.UpdateAccount(new DTO_ACCOUNT { EMAIL=loggedInAccount.EMAIL,NAME=loggedInAccount.NAME,PASSWORD=loggedInAccount.PASSWORD});
              
                // Update cockie with new password if changed
                // Create the cookie and set its value to the username and a hash of the logged in account
                HttpCookie cookie = new HttpCookie(WebSiteManager.REMEMBER_LOGIN_COOCKIE);
                cookie.Value = loggedInAccount.EMAIL + "," + loggedInAccount.PASSWORD;
                Response.Cookies.Add(cookie);


                WebSiteManager.RemovePlayers(((List<int>)Session[WebSiteManager.PLAYERS_4_DELETE_SESSION]));
                WebSiteManager.RemoveGames(((List<int>)Session[WebSiteManager.GAMES_4_DELETE_SESSION]));

            }
            else
            {
                ErrorMessage.Text = "Old Password not match";
            }

        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (!IsPostBack) { 
                DTO_ACCOUNT loggedInAccount = (DTO_ACCOUNT)Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION];
                Email.Text = loggedInAccount.EMAIL;
                nickname.Text = loggedInAccount.NAME;
            }

            OldPassword.Text = "";
            NewPassword.Text = "";
        }

      
    }
}