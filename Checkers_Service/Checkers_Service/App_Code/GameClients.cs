using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;

namespace WcfService.App_Code
{
    // this class used to manage clients connection to current playing game
    public class Game
    {
        public int gameID;

        public Dictionary<int, IClientCallBack> clients = new Dictionary<int, IClientCallBack>(); // list of current checkers playing on board

        // number of ready clients for staring game
        public int readyClients = 0;

        public Game(int gameID)
        {
            this.gameID = gameID;
        }

        public void addClient(DTO_Player player,IClientCallBack client)
        {
                if (clients.ContainsKey(player.Id))
                {
                    clients[player.Id] = client;
                }
                else
                {
                    clients.Add(player.Id, client);
                }
        }

        // used for debug propose
        public void LogMessageToFile(string msg)
        {
            System.IO.StreamWriter sw;
            try
            {
                sw = File.AppendText(@"C:\dot_net_final\log_client.txt");
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
                sw.Close();

            }
            catch (Exception) { }

        }

        // checks whenever client is connected and ready to start playing
        public void CheckForGameStart(IClientCallBack client)
        {
            if (clients.Count == Application_Constants.nPlayersForGameStart)
            {
                readyClients = 0;
                foreach (KeyValuePair<int, IClientCallBack> pair in clients)
                {

                    try
                    {
                        pair.Value.CheckClientReady();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        
        // notifies opponent player on game over and winner player info
        public void OnPlayerWon(DTO_Player wonPlayer)
        {
            foreach(KeyValuePair<int, IClientCallBack> pair in clients)
            {
                if (pair.Key != wonPlayer.Id)
                {
                    pair.Value.UpdateClientWithGameWinner(wonPlayer);
                }
            }
        }

        // notifes client both players are connectd and game can be stated
        public void notifyClientsWithStartGame(DTO_Player whiteCheckersPlayer, DTO_Player blackCheckersPlayer)
        {
            foreach (KeyValuePair<int, IClientCallBack> pair in clients)
            {
                pair.Value.UpdateClientGameStartOk(whiteCheckersPlayer,blackCheckersPlayer);
            }
        }

        // checks whenever this client allredy connected this method used due to fact that when client
        // application crashes wcf service dont know about it and thinks client is still alive.
        public bool isClientAllredyConnected(DTO_Player player)
        {
            foreach(KeyValuePair<int , IClientCallBack> pair in clients)
            {
                if (pair.Key == player.Id)
                {
                    try {
                        pair.Value.isConnected();
                        return true;
                    }
                    catch(FaultException)
                    {
                        //System.Net.WebException
                        //System.ServiceModel.ServerTooBusyException
                        return false;
                    }
                    
                }
            }
            return false;
        }

        public void onClientDisconnect(DTO_Player client)
        {
            foreach (KeyValuePair<int, IClientCallBack> pair in clients)
            {
                if (pair.Key != client.Id)
                {
                    pair.Value.NotifyClientOpponentDisconnected();
                }
            }

            clients.Clear();
        }

        // updaes opponent player on checker move
        public void updateClientOnCheckerMove(DTO_Player movingCheckerPlayer,DTO_Checker checker, string eatenCheckerID)
        {
            foreach (KeyValuePair<int, IClientCallBack> pair in clients)
            {
                if (pair.Key!= movingCheckerPlayer.Id)
                {
                    pair.Value.UpdateClientWithCheckerMove(checker);

                    if (eatenCheckerID != null)
                    {
                        pair.Value.UpdateClientRemoveEatenChecker(eatenCheckerID);
                    }
                }
            }
        }
    }
}