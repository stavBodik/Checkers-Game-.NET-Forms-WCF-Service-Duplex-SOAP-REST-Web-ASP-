using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using WcfService.App_Code;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.UI.WebControls;
using System.Data.Linq;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]

public class Service : IDuplexService, IWebService,ISoapService
{
    // list of current games playing
    public static Dictionary<int, Game> games = new Dictionary<int, Game>();
    // db contects
    public DBDataContext db = new DBDataContext();


    // methods documentation is found in Service interfaces class (Iservice.cs file)

    public void UpdateAccount(DTO_ACCOUNT account)
    {
        var accountForUpdate = from acc in db.Accounts
                               where acc.EMAIL == account.EMAIL
                               select acc;

        foreach(Account acc in accountForUpdate)
        {
            acc.EMAIL = account.EMAIL;
            acc.PASSWORD = account.PASSWORD;
            acc.NAME = account.NAME;
        }

        db.SubmitChanges();
    }

    public void UpdateGameStatus(int gameID, int status)
    {
        //find game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID
                    select gp;


        //get game
        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
            if(gpFromDB.Status!= status) {

                try
                {
                    gpFromDB.Status = status;
                    db.SubmitChanges();
                }
                catch (ChangeConflictException)
                {
                    foreach (ObjectChangeConflict objchangeconf in db.ChangeConflicts)
                    {
                        objchangeconf.Resolve(RefreshMode.OverwriteCurrentValues);
                    }
                }
            }
        }


    }

    public DTO_RESTORED_GAME GetRestoredGame(int gameID)
    {
        // get this game movments
        DTO_MOVEMENT[] movements =  (from movement in db.Movements
                                 where movement.Game_ID==gameID
                                 select new DTO_MOVEMENT { Id = movement.Id, Checker_ID = movement.Checker_ID, Col = movement.Col, Game_ID = movement.Game_ID, Row = movement.Row }).ToArray();

        //find game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID
                    select gp;

        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
        }

        //get this game players
        DTO_Player p1 = (from pl in db.Players
                         where pl.Id == gpFromDB.Player1.Id
                         select new DTO_Player { Id = pl.Id, FirstName = pl.FirstName,LastName=pl.LastName }).FirstOrDefault();


        DTO_Player p2 = (from pl in db.Players
                         where pl.Id == gpFromDB.Player.Id
                         select new DTO_Player { Id = pl.Id, FirstName = pl.FirstName, LastName = pl.LastName }).FirstOrDefault();

        //find team names for this players
        string q1 = (from ac in db.Accounts
                     where ac.EMAIL == gpFromDB.Player1.AccountEmail
                     select ac.NAME).FirstOrDefault();

        string q2 = (from ac in db.Accounts
                     where ac.EMAIL == gpFromDB.Player.AccountEmail
                     select ac.NAME).FirstOrDefault();

        p1.TEAM_NAME = q1;
        p2.TEAM_NAME = q2;



        return new DTO_RESTORED_GAME {gameStatus= gpFromDB.Status,movements= movements,players=new DTO_Player[] {p1,p2 } };
    }

    public void AddGameMovement(int gameID, DTO_MOVEMENT movement)
    {
        //get game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID
                    select gp;


        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
        }

        movement.Game_ID = gpFromDB.GAME_ID;


        db.Movements.InsertOnSubmit(new Movement {Id=movement.Id,Checker_ID=movement.Checker_ID, Col=movement.Col,Game_ID=movement.Game_ID,Row=movement.Row });
        db.SubmitChanges();
    }

    public void UpdateServerWithCheckerMove(int gameID,DTO_Player player,DTO_Checker checker,string eatenCheckerID)
    {

        //get game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID
                    select gp;

        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query){gpFromDB = gamePlay;}

        games[gpFromDB.GAME_ID].updateClientOnCheckerMove(player,checker,eatenCheckerID);

    }

    public void RegisterAccount(DTO_ACCOUNT account)
    {
        var playersEntitiySet = new EntitySet<Player>();
        List<Player> playersList = new List<Player>();
        foreach(DTO_Player dtoplayer in account.players)
        {
            playersList.Add(new Player {Id=dtoplayer.Id,FirstName=dtoplayer.FirstName,LastName=dtoplayer.LastName });
        }

        playersEntitiySet.AddRange(playersList);
        db.Accounts.InsertOnSubmit(new Account {EMAIL=account.EMAIL,NAME=account.NAME,PASSWORD=account.PASSWORD,Players= playersEntitiySet});
        db.SubmitChanges();
    }

    public void UpdateServerPlayerWon(int gameID,DTO_Player wonPlayer)
    {
        //find game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID
                    select gp;


        //get game from db and change its status
        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
            try
            {
                gpFromDB.Status = Application_Constants.GAME_STATUS_END;
                db.SubmitChanges();
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict objchangeconf in db.ChangeConflicts)
                {
                    objchangeconf.Resolve(RefreshMode.OverwriteCurrentValues);
                }
            }

        }


        //notify other player that game ended and he lose 
        games[gpFromDB.GAME_ID].OnPlayerWon(wonPlayer);




    }

    public void RegisterClient(int gameID,DTO_Player player)
    {
        IClientCallBack client = OperationContext.Current.GetCallbackChannel<IClientCallBack>();
        bool isRegisterSucceed = true;

        //find game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID== gameID
                    select gp;


        //get game
        GamePlay gpFromDB=null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
        }

        //check if this game allready exist in games list, if exist its just add to it the clien
        // if not create game

        if (games.ContainsKey(gpFromDB.GAME_ID))
        {
            ////check if this client allready registared
            //if (games[gpFromDB.GAME_ID].isClientAllredyConnected(player))
            //{
            //    isRegisterSucceed= false;
            //    client.NotifyRegistarClientStatus(isRegisterSucceed);
            //    return;
            //}
            //else
            //{
                games[gpFromDB.GAME_ID].addClient(player, client);
          //  }
        }
        else
        {
            Game g = new Game(gpFromDB.GAME_ID);
            g.addClient(player, client);
            games.Add(gpFromDB.GAME_ID,g);
        }

        games[gpFromDB.GAME_ID].CheckForGameStart(client);
        client.NotifyRegistarClientStatus(isRegisterSucceed);

    }

    public void NotifyReadyToPlay(int gameID,DTO_Player player)
    {
        //find game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID && gp.Status==Application_Constants.GAME_STATUS_WATING_FOR_BEGIN
                    select gp;

        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
        }

        games[gpFromDB.GAME_ID].readyClients++;


        if (games[gpFromDB.GAME_ID].readyClients == Application_Constants.nPlayersForGameStart)
        {

            //find team names for this players
            string q1 = (from ac in db.Accounts
                     where ac.EMAIL == gpFromDB.Player1.AccountEmail
                     select ac.NAME).FirstOrDefault();

            string q2 = (from ac in db.Accounts
                     where ac.EMAIL == gpFromDB.Player.AccountEmail
                     select ac.NAME).FirstOrDefault();



            games[gpFromDB.GAME_ID].notifyClientsWithStartGame(new DTO_Player {TEAM_NAME=q1, Id = gpFromDB.Player1.Id, FirstName = gpFromDB.Player1.FirstName, LastName = gpFromDB.Player1.LastName }, new DTO_Player { TEAM_NAME = q2, Id = gpFromDB.Player.Id, FirstName = gpFromDB.Player.FirstName, LastName = gpFromDB.Player.LastName });
            //change status of gameplay on db
                try { 
                    gpFromDB.Status = Application_Constants.GAME_STATUS_PLAYING;
                    db.SubmitChanges();
                }catch(ChangeConflictException)
                {
                    foreach (ObjectChangeConflict objchangeconf in db.ChangeConflicts)
                    {
                        objchangeconf.Resolve(RefreshMode.OverwriteCurrentValues);
                    }
                }
        }

    }

    public void UnregisterClient(int gameID,DTO_Player player,bool isGameOver)
    {

        //get game for this player
        var query = from gp in db.GamePlays
                    where gp.GAME_ID==gameID
                    select gp;


        GamePlay gpFromDB = null;
        foreach (GamePlay gamePlay in query)
        {
            gpFromDB = gamePlay;
        }

        if (!games.ContainsKey(gpFromDB.GAME_ID)) return;

        //in case one of the clients disconnected from game isGameOver equals false
        // means client disconnected in middle of game, we notify other client with game over
        // and remove him from the clients list 
        if (!isGameOver)
        {
            games[gpFromDB.GAME_ID].onClientDisconnect(player);
            try
            {
                gpFromDB.Status = Application_Constants.GAME_STATUS_DISCONNECTED;
                db.SubmitChanges();
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict objchangeconf in db.ChangeConflicts)
                {
                    objchangeconf.Resolve(RefreshMode.KeepCurrentValues);
                }
            }

        }
        else
        {

            try
            {
                gpFromDB.Status = Application_Constants.GAME_STATUS_END;
                db.SubmitChanges();
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict objchangeconf in db.ChangeConflicts)
                {
                    objchangeconf.Resolve(RefreshMode.OverwriteCurrentValues);
                }
            }
        }

        // finally remove this game from games list
        games.Remove(gpFromDB.GAME_ID);
    }

    public void LogMessageToFile(string msg)
    {
        System.IO.StreamWriter sw;
        try
        {
            sw = File.AppendText(@"C:\dot_net_final\log.txt");
            string logLine = System.String.Format(
                "{0:G}: {1}.", System.DateTime.Now, msg);
            sw.WriteLine(logLine);
            sw.Close();

        }
        catch (Exception) { }
        
    }

    public bool IsEmailExist(string email)
    {
        return db.Accounts.Any(u => u.EMAIL == email);
    }

    public DTO_ACCOUNT WebLogin(string email, string password)
    {
        return Login(email,password);
    }

    public DTO_ACCOUNT ClientLogin(string email, string password)
    {
        return Login(email, password);
    }

    public DTO_ACCOUNT Login(string email, string password)
    {
        var query = (from p in db.Accounts
                     where (p.EMAIL.Equals(email) && p.PASSWORD.Equals(password))
                     select new { EMAIL = p.EMAIL, PASSWORD = p.PASSWORD, NAME = p.NAME, Players = p.Players }).FirstOrDefault();
        System.Diagnostics.Trace.TraceError("2If you're seeing this, something bad happened");

        return query == null ? null : new DTO_ACCOUNT { EMAIL = query.EMAIL, PASSWORD = query.PASSWORD, NAME = query.NAME };
    }

    public DTO_Player[] WebGetPlayers(string email)
    {
        return GetPlayers(email);
    }

    public DTO_Player[] ClientGetPlayers(string email)
    {
        return GetPlayers(email);
    }

    private DTO_Player[] GetPlayers(string email)
    {
        DTO_Player[] query = (
                           from acc in db.Accounts
                           where acc.EMAIL.Equals(email)
                           from p in acc.Players
                           where p.isDeleted == null
                           select new DTO_Player
                           {
                               FirstName = p.FirstName,
                               LastName = p.LastName,
                               Id = p.Id,
                               AccountEmail = p.AccountEmail
                           }
                           ).ToArray();

        return query;
    }

    public DTO_Player[] GetAllPlayers()
    {
        DTO_Player[] query = (
                            from acc in db.Accounts
                            from p in acc.Players
                            where p.isDeleted==null
                            select new DTO_Player
                            {
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                Id = p.Id,
                                AccountEmail = p.AccountEmail,
                                TEAM_NAME = acc.NAME
                            }
                            ).ToArray();

        return query;
    }

    public int WebAddGame(DTO_GamePlay game)
    {
        return AddGame(game);
    }

    public int ClientAddGame(DTO_GamePlay game)
    {
        return AddGame(game);
    }

    public int AddGame(DTO_GamePlay game)
    {
        GamePlay gp = new GamePlay { Host_Player_ID = game.hostPlayerID,Join_Player_ID=game.joinedPlayerID, Start_Date = game.createDate, Status = Application_Constants.GAME_STATUS_WATING_FOR_PLAYER, Name = game.gameName };
        db.GamePlays.InsertOnSubmit(gp);
        db.SubmitChanges();
        return gp.GAME_ID;
    }

    public DTO_GamePlay[] GetWatingGames()
    {
        var query = (from gm in db.GamePlays
                     join p in db.Players on gm.Host_Player_ID equals p.Id
                     where (p.isDeleted==null && gm.Status == Application_Constants.GAME_STATUS_WATING_FOR_PLAYER || gm.Status == Application_Constants.GAME_STATUS_WATING_FOR_BEGIN || gm.Status == Application_Constants.GAME_STATUS_PLAYING) && (gm.Join_Player_ID == null || gm.Join_Player_ID != 1)
                     select new DTO_GamePlay { Key = gm.GAME_ID, gameName = gm.Name, createDate = gm.Start_Date, hostPlayerID = gm.Host_Player_ID, hostPlayerAccountEmail = p.AccountEmail, joinedPlayerID = gm.Join_Player_ID, hostPlayerName = (p.FirstName + " " + p.LastName) }).ToArray();

        return query;
    }

    public DTO_GamePlay[] GetAllGames()
    {
        var query = (from gameplay in db.GamePlays
                     select new DTO_GamePlay
                     {
                         Key = gameplay.GAME_ID,
                         gameName = gameplay.Name,
                         createDate = gameplay.Start_Date,
                         hostPlayerID = gameplay.Host_Player_ID,
                         hostPlayerAccountEmail = gameplay.Player1.AccountEmail,
                         joinedPlayerID = gameplay.Join_Player_ID,
                         hostPlayerName = (gameplay.Player1.FirstName + " " + gameplay.Player1.LastName),
                         joinPlayerName = (gameplay.Player.FirstName + " " + gameplay.Player.LastName),
                         Status = gameplay.Status
                     }).ToArray();

        return query;      
    }

    public int JoinGame(string hostPlayerID, string joiningPlayerID)
    {
        int nhostPlayerID = Int32.Parse(hostPlayerID);
        int njoiningPlayerID = Int32.Parse(joiningPlayerID);

        int gameID =0;
        var watingGamePlay = from wg in db.GamePlays
                             where wg.Host_Player_ID == nhostPlayerID && wg.Status == 0
                             select wg;

        foreach (GamePlay gp in watingGamePlay)
        {
            gameID = gp.GAME_ID;
            gp.Status = Application_Constants.GAME_STATUS_WATING_FOR_BEGIN;
            gp.Join_Player_ID = njoiningPlayerID;
        }

        db.SubmitChanges();

        return gameID;
    }

    public void ExistGame(int gameID)
    {
        var watingGamePlay = from wg in db.GamePlays
                             where wg.GAME_ID == gameID
                             select wg;

        foreach (GamePlay gp in watingGamePlay)
        {
            try
            {
                gp.Status = Application_Constants.GAME_STATUS_WATING_FOR_PLAYER;
                db.SubmitChanges();
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict objchangeconf in db.ChangeConflicts)
                {
                    objchangeconf.Resolve(RefreshMode.OverwriteCurrentValues);
                }
            }


            gp.Join_Player_ID = null;
        }

        db.SubmitChanges();
    }

    public DTO_Player FindGameAndReturnPlayerInfo(int gameID,string accountEmail)
    {
        // find game
        var gameFound = (from gm in db.GamePlays
                         where gm.GAME_ID == gameID && gm.Status==Application_Constants.GAME_STATUS_WATING_FOR_BEGIN
                         select new DTO_GamePlay { Key = gm.GAME_ID, gameName = gm.Name, createDate = gm.Start_Date, hostPlayerID = gm.Host_Player_ID, joinedPlayerID = gm.Join_Player_ID }).FirstOrDefault();

        if (gameFound == null) return null;


        //check if one of players in this account participate in this game
        var query = from a in db.Accounts
                    where (a.Players.Any(p => p.Id == gameFound.hostPlayerID) || a.Players.Any(p => p.Id == gameFound.joinedPlayerID) ) && a.EMAIL.Equals(accountEmail)
                    select a;
        if (!query.Any()) return null;

        // get player
        DTO_Player player = (from p in db.Players
                     where (p.Id == gameFound.hostPlayerID || p.Id == gameFound.joinedPlayerID) && p.AccountEmail.Equals(accountEmail)
                     select new DTO_Player { Id = p.Id,FirstName=p.FirstName,LastName=p.LastName }).FirstOrDefault();

        // is this player hosts(white checkers) game or joined(black checkers) to game
        if (gameFound.hostPlayerID == player.Id)
        {
            player.Type = Application_Constants.whiteChecker;
        }else
        {
            player.Type = Application_Constants.blackChecker;
        }

        return player;
    }

    public DTO_GamePlay[] GetGamesByPlayerID(string playerID)
    {

        var query = (from gameplay in db.GamePlays
                     where gameplay.Host_Player_ID==Int32.Parse(playerID) || gameplay.Join_Player_ID== Int32.Parse(playerID)
                     select new DTO_GamePlay
                     {
                         Key = gameplay.GAME_ID,
                         gameName = gameplay.Name,
                         createDate = gameplay.Start_Date,
                         hostPlayerID = gameplay.Host_Player_ID,
                         hostPlayerAccountEmail = gameplay.Player1.AccountEmail,
                         joinedPlayerID = gameplay.Join_Player_ID,
                         hostPlayerName = (gameplay.Player1.FirstName + " " + gameplay.Player1.LastName),
                         joinPlayerName = (gameplay.Player.FirstName + " " + gameplay.Player.LastName),
                         Status = gameplay.Status
                     }).ToArray();

        return query;
    }

    public DTO_GamePlay[] GetPreviewsGamesForRestore(string accountEmail)
    {
        var query = (from gameplay in db.GamePlays
                     join move in db.Movements on gameplay.GAME_ID equals move.Game_ID
                     where (gameplay.Status==Application_Constants.GAME_STATUS_END || gameplay.Status == Application_Constants.GAME_STATUS_DISCONNECTED) && (gameplay.Player.AccountEmail.Equals(accountEmail) || gameplay.Player1.AccountEmail.Equals(accountEmail))
                     select new DTO_GamePlay { Key = gameplay.GAME_ID,
                                               gameName = gameplay.Name,
                                               createDate = gameplay.Start_Date,
                                               hostPlayerID = gameplay.Host_Player_ID,
                                               hostPlayerAccountEmail = gameplay.Player1.AccountEmail,
                                               joinedPlayerID = gameplay.Join_Player_ID,
                                               hostPlayerName = (gameplay.Player1.FirstName + " " + gameplay.Player1.LastName),
                                               joinPlayerName= (gameplay.Player.FirstName + " " + gameplay.Player.LastName)
                                               
                     }).Distinct().ToArray();
        return query;
    }

    public List<DTO_Player> GetPlayersByGameID(string gmID)
    {
        int gameID = Int32.Parse(gmID);

        var gamePlay = from gp in db.GamePlays
                      where gp.GAME_ID == gameID
                      select gp;

        List<DTO_Player> players = new List<DTO_Player>();
        string playerAccountEmail, accName;

        foreach (GamePlay gp in gamePlay)
        {
            if (gp.Player != null)
            {
                 playerAccountEmail = gp.Player.AccountEmail;
                 accName = (from a in db.Accounts where a.EMAIL == playerAccountEmail select new DTO_ACCOUNT { NAME = a.NAME }).FirstOrDefault().NAME;

                DTO_Player p1 = new DTO_Player
                {
                    FirstName = gp.Player.FirstName,
                    LastName = gp.Player.LastName,
                    Id = gp.Player.Id,
                    AccountEmail = gp.Player.AccountEmail,
                    TEAM_NAME = accName
                };

                players.Add(p1);
            }

            if (gp.Player1 != null) {

                playerAccountEmail = gp.Player1.AccountEmail;
                accName = (from a in db.Accounts where a.EMAIL == playerAccountEmail select new DTO_ACCOUNT { NAME = a.NAME }).FirstOrDefault().NAME;

                DTO_Player p2 = new DTO_Player
                {
                    FirstName = gp.Player1.FirstName,
                    LastName = gp.Player1.LastName,
                    Id = gp.Player1.Id,
                    AccountEmail = gp.Player1.AccountEmail,
                    TEAM_NAME = accName
                };

                players.Add(p2);
            }
        }


        return players;
    }

    public List<DTO_PLAYER_GAMES> GetForEachPlayerNGames()
    {
        DTO_Player[] allPlayers = GetAllPlayers();
        List<DTO_PLAYER_GAMES> result = new List<DTO_PLAYER_GAMES>();

        foreach (DTO_Player pll in allPlayers)
        {
            result.Add(new DTO_PLAYER_GAMES { ID=pll.Id, PlayerName =( pll.FirstName+" "+pll.LastName),Games= db.GamePlays.Count(a => a.Host_Player_ID == pll.Id) });
        }
        return result; 
    }

    public DTO_GamePlay[] GetAllGamesByAccountEmail(string accountEmail)
    {
        var query = (from gameplay in db.GamePlays
                     where (gameplay.Player.AccountEmail.Equals(accountEmail) || gameplay.Player1.AccountEmail.Equals(accountEmail))
                     select new DTO_GamePlay
                     {
                         Key = gameplay.GAME_ID,
                         gameName = gameplay.Name,
                         createDate = gameplay.Start_Date,
                         hostPlayerID = gameplay.Host_Player_ID,
                         hostPlayerAccountEmail = gameplay.Player1.AccountEmail,
                         joinedPlayerID = gameplay.Join_Player_ID,
                         hostPlayerName = (gameplay.Player1.FirstName + " " + gameplay.Player1.LastName),
                         joinPlayerName = (gameplay.Player.FirstName + " " + gameplay.Player.LastName)

                     }).ToArray();
        return query;
    }

    public void RemovePlayers(List<int> playersID)
    { 
        foreach (int pID in playersID)
        {
            var query = from player in db.Players where player.Id == pID select player;
            foreach (Player player in query) { player.isDeleted = 1; }
            db.SubmitChanges();
        }
    }

    public void RemoveGames(List<int> gamesID)
    {
        foreach (int gameID in gamesID)
        {
            db.GamePlays.DeleteOnSubmit(db.GamePlays.Where(gp => gp.GAME_ID == gameID).Select(gp => gp).Single());
            db.SubmitChanges();
        }
    }
}