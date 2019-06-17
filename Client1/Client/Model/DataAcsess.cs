using Client.Control;
using Client.ServiceRef;
using Client.View;
using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;

namespace Client.Model
{
    // This class is used as Data Access Object ,
    // Communicates with application service and updates DB(through the service)

    public static class DataAcsess
    {
        private static DuplexServiceClient duplexServiceClient = new DuplexServiceClient(new System.ServiceModel.InstanceContext(new CallBack()));
        private static SoapServiceClient soapServiceClient = new SoapServiceClient();
        public static AutoResetEvent autoReset = new AutoResetEvent(false);

        // updates game statuses declered in Constants.cs 
        public static void UpdateGameStatus(int gameID,int status)
        {
            if (GameController.gameType == Constants.GAME_TYPE_PLAY_ONLINE || GameController.gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER)
            {
                duplexServiceClient.UpdateGameStatus(gameID, status);
            }
        }
        // registation of new local game of player vs computer for storing game movments.
        public static void RegisterNewLocalGamePlay()
        {
            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];

            Thread thread = new Thread(delegate ()
            {
                DTO_GamePlay gamePlay = new DTO_GamePlay { hostPlayerID = GameController.loggedInAccountPlayerPlaying.Id,joinedPlayerID=Constants.GM_PLAYER_ID, createDate = DateTime.Now.ToLocalTime(), gameName = GameController.loggedInAccount.NAME + " VS Checkers" };
                GameController.currentPlayingGameID = duplexServiceClient.ClientAddGame(gamePlay);
                form1Ref.ShowAndStartLocalGamePlay();

            });

            thread.Start();
        }

        public static void GetGamesListForRestore()
        {
            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];
            DTO_GamePlay[] games=null;
            CheckState exerciseCheckboxState = form1Ref.applicationMenu.uiExerciseCheckBox.CheckState;
            CheckState exerciseFixCheckboxState = form1Ref.applicationMenu.uiExerciseCheckBoxFix.CheckState;

            if (exerciseCheckboxState == CheckState.Unchecked)
            {
                Thread thread = new Thread(delegate ()
                {
                    LoadRestoredGames(ref games);
                    form1Ref.LoadAndDesignGamesForRestorePanel(games);
                });thread.Start();
            }
            else if(exerciseCheckboxState == CheckState.Checked && exerciseFixCheckboxState== CheckState.Unchecked)
            {
                // BUGGY CASE
                int milliseconds = Constants.UI_EXERCISE_DELAY_TIME;
                Thread.Sleep(milliseconds);
                LoadRestoredGames(ref games);
                form1Ref.LoadAndDesignGamesForRestorePanel(games);
            }else
            {
                // FIX BUGGY CASE
                Thread thread = new Thread(delegate ()
                {
                    int milliseconds = Constants.UI_EXERCISE_DELAY_TIME;
                    Thread.Sleep(milliseconds);

                    LoadRestoredGames(ref games);
                    // signal wating handle inside form closing 
                    DataAcsess.autoReset.Set();
                    //UI thread 
                    form1Ref.LoadAndDesignGamesForRestorePanel(games);
                });

                
                thread.Start();

            }
        }

        public static void LoadRestoredGames(ref DTO_GamePlay[] games)
        {
            games= soapServiceClient.GetPreviewsGamesForRestore(GameController.loggedInAccount.EMAIL);
            foreach (DTO_GamePlay gp in games)
            {
                if (gp.joinedPlayerID == null)
                {
                    gp.joinPlayerName = "Checkers";
                }
            }
        }

        // updates server on game won (the server updates the opponent player)
        public static void UpdateServerPlayerWon(DTO_Player player)
        {
            if (GameController.gameType != Constants.GAME_TYPE_PLAY_ONLINE) return;
            duplexServiceClient.UpdateServerPlayerWon(GameController.currentPlayingGameID, player);
            if (GameController.loggedInAccountPlayerPlaying != null)
            {
                UnregisterClientOnServer();
            }
        }

        public static void UnregisterClientOnServer()
        {
            if (GameController.gameType != Constants.GAME_TYPE_PLAY_ONLINE) return;
            duplexServiceClient.UnregisterClient(GameController.currentPlayingGameID,GameController.loggedInAccountPlayerPlaying, GameController.gameStatus==Constants.GAME_STATUS_END ? true:false);
            GameController.loggedInAccountPlayerPlaying = null;
        }

        public static void registerClientOnServer()
        {
            if (GameController.gameType != Constants.GAME_TYPE_PLAY_ONLINE) return;
            Thread thread = new Thread(delegate ()
            {
                duplexServiceClient.RegisterClient(GameController.currentPlayingGameID,GameController.loggedInAccountPlayerPlaying);
            });

            thread.Start();
        }
        
        // updates server on checker move (the server updates the opponent player with this player moves)
        public static void UpdateServerWithCheckerMove(DTO_Checker checker,string eatenCheckerID)
        {
            if (GameController.gameType != Constants.GAME_TYPE_PLAY_ONLINE)
            {
                return;
            }

            Thread thread = new Thread(delegate ()
            {
              duplexServiceClient.UpdateServerWithCheckerMove(GameController.currentPlayingGameID, GameController.loggedInAccountPlayerPlaying, checker,eatenCheckerID);
            });

            thread.Start();
        }

        public static void OnLocalGameMenuClick()
        {
            GameForm gameFormRef = (GameForm)Application.OpenForms["GameForm"];

            Thread thread = new Thread(delegate ()
            {
                DTO_Player[] players = duplexServiceClient.ClientGetPlayers(GameController.loggedInAccount.EMAIL);
                gameFormRef.LoadAndDesignLocalGameStartPanel(players);
            });

            thread.Start();

        }

        public static DTO_ACCOUNT Login(string email,string password)
        {
            string passwordHashed = CreateHash(password, Constants.PASSWORD_HASH_SALT);
            DTO_ACCOUNT account = duplexServiceClient.ClientLogin(email, passwordHashed);
            if (account == null) return null;
            else
            {
                account.players = duplexServiceClient.ClientGetPlayers(account.EMAIL);
                return account;
            }
        }

        // notify server that this client redy to play and connected
        public static void NotifyReadyToPlay()
        {
            if (GameController.gameType != Constants.GAME_TYPE_PLAY_ONLINE) return;
            duplexServiceClient.NotifyReadyToPlay(GameController.currentPlayingGameID,GameController.loggedInAccountPlayerPlaying);
        }
        
        // Create a hash of the given password and salt for user login authentication.
        private static string CreateHash(string password, string salt)
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

        // on game start find game associated with connected account
        public static DTO_Player FindGameAndReturnPlayerInfo(int gameID,string accountEmail)
        {
               return duplexServiceClient.FindGameAndReturnPlayerInfo(gameID, accountEmail);
        }
        // adds game movment for future restoring of game
        public static void AddGameMovement(int gameID,DTO_MOVEMENT movement)
        {
            Thread thread = new Thread(delegate ()
            {
                soapServiceClient.AddGameMovement(gameID, movement);
            });

            thread.Start();
        }

        public static void OnRestoreGame(int gameID)
        {
            GameForm GameFormReference = (GameForm)Application.OpenForms["GameForm"];

            Thread thread = new Thread(delegate ()
                {
                    DTO_RESTORED_GAME restoredGame = duplexServiceClient.GetRestoredGame(gameID);
                    GameController.gameType = Constants.GAME_TYPE_GAME_RESTORE;
                    GameFormReference.ShowRestoredGame(restoredGame);

                });

                thread.Start();
        }
    }

    // callbacks from server to client
    public class CallBack : IDuplexServiceCallback
    {

        delegate void UpdateClientWithGameWinnerCallBack(DTO_Player wonPlayer);

        // called from server whenever game over and there is winner
        public void UpdateClientWithGameWinner(DTO_Player wonPlayer)
        {
            GameForm gameForm = (GameForm)Application.OpenForms["GameForm"];
            gameForm.OnGameWon(wonPlayer);
        }

        public void UpdateClientWithCheckerMove(DTO_Checker ch)
        {
            GameController.UpdateCheckerPosition(GameController.checkers[ch.ID], ch.oldPositionOnBord);
            GameController.updateCurrentOpponentVisibility(GameController.loggedInAccountPlayerPlaying);
            GameController.isThisPlayerTurn = true;
        }

        // called from server whenever opponent player eaten this client checker
        public void UpdateClientRemoveEatenChecker(string eatenCheckerID)
        {
            GameController.RemoveEatenChecker(eatenCheckerID);
        }

        // called from server whenever both client called NotifyReadyToPlay method
        public void UpdateClientGameStartOk(DTO_Player whiteCheckersPlayer, DTO_Player blackCheckersPlayer)
        {
            GameController.opponents[0].setOpponentText(whiteCheckersPlayer.TEAM_NAME, whiteCheckersPlayer.FirstName+" "+ whiteCheckersPlayer.LastName);
            GameController.opponents[1].setOpponentText(blackCheckersPlayer.TEAM_NAME, blackCheckersPlayer.FirstName + " " + blackCheckersPlayer.LastName);
            GameController.isPlaying = true;
            GameController.showOpponnetsOnGameStart();

            if (GameController.thisPlayerType == Constants.whiteChecker)
            {
                GameController.loggedInAccountPlayerPlaying.TEAM_NAME = whiteCheckersPlayer.TEAM_NAME;
            }
            else
            {
                GameController.loggedInAccountPlayerPlaying.TEAM_NAME = blackCheckersPlayer.TEAM_NAME;
            }
        }

        // notifies client registation status
        public void NotifyRegistarClientStatus(bool isRegisterSucceed)
        {
            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];

             if (isRegisterSucceed) { 
                form1Ref.OnRegisterClientSucceed();
            }else
            {
                form1Ref.OnRegisterClientFaild();
            }
        }

        public void CheckClientReady()
        {
            DataAcsess.NotifyReadyToPlay();
        }

        // This function dose nothing,used for check from server if client is connected
        // in case service succeed call this function means client is connected.

        public void isConnected()
        {
        }

        public void NotifyClientOpponentDisconnected()
        {
            GameController.OnOpponentDissconnected();
        }
    }
}
