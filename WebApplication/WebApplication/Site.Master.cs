using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using System.Web.UI.HtmlControls;
using WebApplication.App_Code;
using WebApplication.ServiceRef;
using System.IO;

namespace WebApplication
{
    public partial class SiteMaster : MasterPage
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            // check if remmber me cockie is set, if is set get logged in account
            if (Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] == null && Request.Cookies[WebSiteManager.REMEMBER_LOGIN_COOCKIE] != null && Request.Cookies[WebSiteManager.REMEMBER_LOGIN_COOCKIE].Value.Length>0)
            {
                var accountLoginInfo = Request.Cookies[WebSiteManager.REMEMBER_LOGIN_COOCKIE].Value;

                string[] values = accountLoginInfo.Split(',');
                // Retrieve the username and hash from the split values.
                string email = values[0];
                string password = values[1];
                // get logged in account from db
                DTO_ACCOUNT loggedInAccount = WebSiteManager.Login(email, password);
                // get logged in account players from db
                DTO_Player[] players = WebSiteManager.GetPlayers(email);
                loggedInAccount.players = players;
                // save logged in account to current session
                Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] = loggedInAccount;

            }

          
              SetLoggedInBar();
            
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
          
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
           
        }


        protected void SetLoggedInBar()
        {
           if(Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] != null)
            { 
                // right side bar
                loggedin_bar.Visible = true;
                loggedout_bar.Visible = false;

                // leftsidebar
                loggedin_bar1.Visible = true;
                loggedout_bar1.Visible = false;
            }
            else
            {
                // right side bar
                loggedin_bar.Visible = false;
                loggedout_bar.Visible = true;

                //left side bar
                loggedin_bar1.Visible = false;
                loggedout_bar1.Visible = true;

            }
        }

        protected void LogOut(object sender, EventArgs e)
        {
            // clear logged in user info
            Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] = null;
           
            // cleare remmeber me coccike
            if (Request.Cookies[WebSiteManager.REMEMBER_LOGIN_COOCKIE] != null)
            {
                Request.Cookies[WebSiteManager.REMEMBER_LOGIN_COOCKIE].Value = "";
                Response.Cookies[WebSiteManager.REMEMBER_LOGIN_COOCKIE].Expires = DateTime.Now.AddDays(-1);
            }

            SetLoggedInBar();

            try { 
            Server.Transfer("../Default.aspx");
            }
            catch (Exception)
            {
                //this case is whenever user is on default page and press on logout
                // we cant transfer to same page
            }


        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {

        }
    }

}