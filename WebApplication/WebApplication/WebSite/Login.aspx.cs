using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using WebApplication.App_Code;
using System.Net;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using WebApplication.ServiceRef;

namespace WebApplication.WebSite
{
    public partial class Login : Page
    {
        protected void LogIn(object sender, EventArgs e)
        {
          
       //     if (IsPostBack && Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] != null) return;

            if (IsValid)
            {
                try
                {
                    // first of all check if email exist
                    if (!WebSiteManager.IsEmailExist(Email.Text))
                    {
                        FailureText.Text = "Email not exist";
                        ErrorMessage.Visible = true;
                        return;
                    }
                    else
                    {
                        // generate hash from password for comparing password in db

                        string passwordhashed = WebSiteManager.CreateHash(Password.Text, WebSiteManager.PASSWORD_HASH_SALT); 
                        DTO_ACCOUNT loggedInAccount = WebSiteManager.Login(Email.Text, passwordhashed);


                        if (loggedInAccount != null)
                        {
                            DTO_Player[] players = WebSiteManager.GetPlayers(Email.Text);
                            loggedInAccount.players = players;

                            // is remmber me 
                            if (RememberMe.Checked)
                            {
                                // Create the cookie and set its value to the username and a hash of the logged in account
                                HttpCookie cookie = new HttpCookie(WebSiteManager.REMEMBER_LOGIN_COOCKIE);
                                cookie.Value = loggedInAccount.EMAIL + "," + loggedInAccount.PASSWORD;
                                Response.Cookies.Add(cookie);
                            }

                            Session[WebSiteManager.LOGGEDIN_ACCOUNT_SESSION] = loggedInAccount;
                            Server.Transfer("../Default.aspx", true);
                        }
                        else
                        {
                            FailureText.Text = "Wrong Login Information";
                            ErrorMessage.Visible = true;
                        }
                      

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
                            FailureText.Text = sr.ReadToEnd();
                            ErrorMessage.Visible = true;

                        }
                    }
                }
            }
            
        }
    }
}