using System;
using System.Web.UI;
using WebApplication.ServiceRef;
using System.Net;
using System.IO;
using System.Collections.Generic;
using WebApplication.App_Code;

namespace WebApplication.WebSite
{
    public partial class Register : Page
    {

        protected override void OnInitComplete(EventArgs e)
        {
            // in case user already logged in and somehow got to register page, redirect him.
            if (Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] != null)
            {
                Server.Transfer("../Default.aspx", true);
            }
            // when page refresh not by post back means this first time user enter the page or did refresh mannualy
            // in this case all form clears and no players, so rest the players list which is saved
            // inside session
            else if (!IsPostBack)
            {
                Session["accountPLayers"] = new List<DTO_Player>();
            }
        }


        protected void CreateUser_Click(object sender, EventArgs e)
        {
            List<DTO_Player> accountPLayers = (List<DTO_Player>)Session["accountPLayers"];
            
            // check if players added if no show error message
            if (accountPLayers.Count == 0)
            {
                playersError.Style["display"] = "inline";
                return;

            }
            else if (WebSiteManager.IsEmailExist(Email.Text))
            {
                GeneralError.Text = "Email allready in exists.";
                return;
            }
            else
            {
                // create new user and add to db , in case faild validation errors will be on page.
                try
                {
                    if (IsValid)
                    {
                        // register account to DB
                        // create Hash from user password
                        string hashedPassword = WebSiteManager.CreateHash(Password.Text, WebSiteManager.PASSWORD_HASH_SALT);
                        // create account entitiy
                        DTO_ACCOUNT a = new DTO_ACCOUNT { EMAIL = Email.Text, PASSWORD = hashedPassword, NAME = nickname.Text };
                        // this players not include there ids from db
                        a.players = accountPLayers.ToArray();
                        // add to db
                        WebSiteManager.RegisterAccount(a);
                        // getaccount players with there ids from db
                        a.players = WebSiteManager.GetPlayers(a.EMAIL);
                        // store logged in account and transfer to homepage
                        Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] = new DTO_ACCOUNT { EMAIL = a.EMAIL, PASSWORD = a.PASSWORD, NAME = a.NAME, players = a.players };


                        Server.Transfer("../Default.aspx", true);
                    }
                }
                catch (WebException ex)
                {
                    using (WebResponse response = ex.Response)
                    {
                        var httpResponse = (HttpWebResponse)response;

                        using (Stream data = response.GetResponseStream())
                        {
                            StreamReader sr = new StreamReader(data);
                            GeneralError.Text = sr.ReadToEnd();

                        }
                    }
                }

            }
}
    
        protected void AddPlayers_Click(object sender, EventArgs e)
        {
            firstnameTB.Text = "";
            lastnameTB.Text = "";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addPlayersDialogModel", "$('#addPlayersDialogModel').modal();", true);
            upModal.Update();

        }

        protected void AddPlayer_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(firstnameTB.Text) && !string.IsNullOrWhiteSpace(lastnameTB.Text))
            {
                DTO_Player p = new DTO_Player { FirstName=firstnameTB.Text,LastName=lastnameTB.Text};
                
                List<DTO_Player> accountPLayers = (List<DTO_Player>)Session["accountPLayers"];
                if (accountPLayers == null)
                {
                    accountPLayers = new List<DTO_Player>();
                }
                accountPLayers.Add(p);


                Session["accountPLayers"] = accountPLayers;

                // show the scrollable div of players 
                playersDiv.Style["display"] = "inline";
                // add this player to this div
                playersList.InnerHtml = playersList.InnerHtml + "<li style=\"margin:0; position: relative; left: 0;\" class=\"list-group-item\">" + firstnameTB.Text + " " + lastnameTB.Text + "</li>";
                // close the dialog
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addPlayersDialogModel", "$('#addPlayersDialogModel').modal('toggle');", true);
                // hide error if was 
                playersError.Style["display"] = "none";
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addPlayersDialogModel", "$('#addPlayersDialogModel').modal();", true);
                upModal.Update();
            }
        }
    }
}