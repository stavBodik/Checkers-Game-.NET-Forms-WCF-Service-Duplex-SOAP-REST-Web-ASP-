using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using WebApplication.ServiceRef;

namespace WebApplication.App_Code
{
    //this class used to communicate with the server service , 
    // updates and gets information from db trought the service using json  .
    public static class WebSiteManager
    {

        public const string SERVICE_URL = "http://localhost:52032/Service.svc";
        //public const string SERVICE_URL = "http://checkersservice.azurewebsites.net/Service.svc";

        // session constants
        public const string REMEMBER_LOGIN_COOCKIE = "rememberMe";
        public const string LOGGEDIN_ACCOUNT_SESSION = "LoggedInAccount";
        public const string WATING_GAMES_LIST_SESSION = "WATING_GAMES_LIST_SESSION";
        public const string PASSWORD_HASH_SALT = "C@CKER@S";
        public const string PLAYERS_4_DELETE_SESSION = "PLAYERS4DELETE";
        public const string GAMES_4_DELETE_SESSION = "GAMES4DELETE";

        public const int GAME_STATUS_WATING = 0;
        public const int GAME_STATUS_PLAYING = 1;
        public const int GAME_STATUS_END = 2;

        public const string GAME_STATUS_WATING_COLOR = "#c4fdff";
        public const string GAME_STATUS_PLAYING_COLOR = "#f78a05";
        public const string GAME_STATUS_REGISTARED_COLOR = "#55fb58";

        // checks whenever this email found on database used when user login to website
        public static bool IsEmailExist(string email)
        {
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/isEmailExist/{0}", email);
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            return result.Equals("true") ? true : false;
        }

        public static void RegisterAccount(DTO_ACCOUNT a)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DTO_ACCOUNT));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, a);
            string data = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadString(SERVICE_URL + "/Register", "POST", data);
        }

        // updates account information on edit profile,games,players 
        public static void UpdateAccount(DTO_ACCOUNT a)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DTO_ACCOUNT));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, a);
            string data = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadString(SERVICE_URL + "/UpdateAccount", "POST", data);
        }

        // used for debug propose
        public static void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw;
            try
            {
                sw = File.AppendText(@"C:\dot_net_final\website_log.txt");
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
                sw.Close();

            }
            catch (Exception) { }

        }

        public static DTO_ACCOUNT Login(string email,string password)
        {
            
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/WebLogin/{0}/{1}",email,password);
            byte[] data = proxy.DownloadData(serviceURL);
            // when no data means account not found on db (service return null)
            if (data.Length == 0) return null;
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_ACCOUNT));
            return obj.ReadObject(stream) as DTO_ACCOUNT;
        }

        // gets all playres for connected account
        public static DTO_Player[] GetPlayers(string email)
        {
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/WebGetPlayers/{0}", email);
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);

            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_Player[]));
            return obj.ReadObject(stream) as DTO_Player[];
        }

        // gets all players associated with registered accounts in game .
        public static DTO_Player[] GetAllPlayers()
        {
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/GetAllPlayers");
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);

            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_Player[]));
            return obj.ReadObject(stream) as DTO_Player[];
        }

        // Create a hash of the given password and salt.
        public static string CreateHash(string password, string salt)
        {
            // Get a byte array containing the combined password + salt.
            string authDetails = password + salt;
            byte[] authBytes = System.Text.Encoding.ASCII.GetBytes(authDetails);

            // Use MD5 to compute the hash of the byte array, and return the hash as
            // a Base64-encoded string.
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes = md5.ComputeHash(authBytes);
            string hash = Convert.ToBase64String(hashedBytes);

            return hash;
        }

        public static int  AddGame(DTO_GamePlay game)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DTO_GamePlay));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, game);
            string data = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            string res = webClient.UploadString(SERVICE_URL + "/WebAddGame", "POST", data);
            return Int32.Parse(res);
        }

        public static void RemovePlayers(List<int> players4delete)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<int>));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, players4delete);
            string data = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadString(SERVICE_URL + "/RemovePlayers", "POST", data);
        }

        public static void RemoveGames(List<int> games4delete)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<int>));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, games4delete);
            string data = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadString(SERVICE_URL + "/RemoveGames", "POST", data);
        }

        public static void ExistGame(int gameID)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(int));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, gameID);
            string data = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            WebClient webClient = new WebClient();
            webClient.Headers["Content-type"] = "application/json";
            webClient.Encoding = Encoding.UTF8;
            webClient.UploadString(SERVICE_URL + "/ExistGame", "POST", data);
        }

        public static DTO_GamePlay[] GetAllGames()
        {
            WebClient proxy = new WebClient();
            string serviceURL = SERVICE_URL + "/GetAllGames";
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_GamePlay[]));
            return obj.ReadObject(stream) as DTO_GamePlay[];
        }

        public static List<DTO_PLAYER_GAMES> GetForEachPlayerNGames()
        {
            WebClient proxy = new WebClient();
            string serviceURL = SERVICE_URL + "/GetForEachPlayerNGames";
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(List<DTO_PLAYER_GAMES>));
            return obj.ReadObject(stream) as List<DTO_PLAYER_GAMES>;
        }

        public static DTO_GamePlay[] GetGamesByPlayerID(int playerID)
        {
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/GetGamesByPlayerID/{0}", playerID);
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_GamePlay[]));
            return obj.ReadObject(stream) as DTO_GamePlay[];
        }

        // wating games are games that not started yet and wating for opponent player to join thems
        public static DTO_GamePlay[] GetWatingGames()
        {
            WebClient proxy = new WebClient();
            string serviceURL = SERVICE_URL + "/GetWatingGames";
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_GamePlay[]));
            return obj.ReadObject(stream) as DTO_GamePlay[];
        }

        public static int JoinGame(string hostPlayerID, string joiningPlayerID)
        {

            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/JoinGame/{0}/{1}", hostPlayerID, joiningPlayerID);
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            StreamReader reader = new StreamReader(stream);
            string gameID = reader.ReadToEnd();
            return Int32.Parse(gameID);
        }

        public static DTO_Player[] GetPlayersByGameID(int gameid)
        {
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/GetPlayersByGameID/{0}", gameid);
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);

            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_Player[]));
            return obj.ReadObject(stream) as DTO_Player[];
        }

        public static DTO_GamePlay[] GetAllGamesByAccountEmail(string accountEmail)
        {
            WebClient proxy = new WebClient();
            string serviceURL = string.Format(SERVICE_URL + "/GetAllGamesByAccountEmail/{0}", accountEmail);
            byte[] data = proxy.DownloadData(serviceURL);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(DTO_GamePlay[]));
            return obj.ReadObject(stream) as DTO_GamePlay[];
        }


        // this functions used to desgin binded data gride view,  as not all information and order should showed 
        // as they got from object properties .
        public static void renameColumn(ref GridView grid, int index,string newName)
        {
            if (grid.HeaderRow == null) return;

            grid.HeaderRow.Cells[index].Text = newName;
        }

        public static void hideColumnByIndex(ref GridView grid, int index)
        {
            grid.HeaderRow.Cells[index].Visible = false;

            foreach (GridViewRow r in grid.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    r.Cells[index].Visible = false;
                }
            }
        }

        public static int GetColumnIndexByName(GridView grid, string name)
        {
            if (grid.HeaderRow == null) return -1;
            for (int i = 0; i < grid.HeaderRow.Cells.Count; i++)
            {
                if (grid.HeaderRow.Cells[i].Text.ToLower().Trim() == name.ToLower().Trim())
                {
                    return i;
                }
            }
            return -1;
        }
    }


}