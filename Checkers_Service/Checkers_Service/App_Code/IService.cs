using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using WcfService.App_Code;

[ServiceContract(CallbackContract = typeof(IClientCallBack))]
public interface IDuplexService
{
    // updates game statuses declered in Constants.cs 
    [OperationContract(IsOneWay = true)]
    void UpdateGameStatus(int gameID,int status);

    // returnes to client restored game for viewing his movements
    [OperationContract]
    DTO_RESTORED_GAME GetRestoredGame(int gameID);

    // updates server on checker move (the server updates the opponent player with this player moves)
    [OperationContract(IsOneWay = true)]
    void UpdateServerWithCheckerMove(int gameID,DTO_Player Player, DTO_Checker obj,string eatenCheckerID);

    // registration of client on game connect
    [OperationContract(IsOneWay = true)]
    void RegisterClient(int gameID, DTO_Player player);

    // on game start find game associated with connected account
    [OperationContract]
    DTO_Player FindGameAndReturnPlayerInfo(int gameID, string accountEmail);

    // unregister of client on game over
    [OperationContract(IsOneWay = true)]
    void UnregisterClient(int gameID,DTO_Player player, bool isGameOver);

    // notifies server whenever client is redy to play
    [OperationContract(IsOneWay = true)]
    void NotifyReadyToPlay(int gameID,DTO_Player player);

    // updates server on game won (the server updates the opponent player)
    [OperationContract(IsOneWay = true)]
    void UpdateServerPlayerWon(int gameID,DTO_Player wonPlayer);

    [OperationContract]
    DTO_ACCOUNT ClientLogin(string email, string password);

    [OperationContract]
    DTO_Player[] ClientGetPlayers(string email);

    [OperationContract]
    int ClientAddGame(DTO_GamePlay game);
}

[ServiceContract]
public interface ISoapService
{
    // adds game movment for future restoring of game
    [OperationContract]
    void AddGameMovement(int gameID, DTO_MOVEMENT movement);

    [OperationContract]
    DTO_GamePlay[] GetPreviewsGamesForRestore(string accountEmail);
}

[ServiceContract]
public interface IWebService
{
    // checks whenever this email found on database used when user login to website
    [OperationContract]
    [WebGet(UriTemplate = "/IsEmailExist/{email}", ResponseFormat = WebMessageFormat.Json)]
    bool IsEmailExist(string email);

    [OperationContract]
    [WebInvoke(UriTemplate = "/Register", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
    void RegisterAccount(DTO_ACCOUNT account);

    // updates account information on edit profile,games,players 
    [OperationContract]
    [WebInvoke(UriTemplate = "/UpdateAccount", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
    void UpdateAccount(DTO_ACCOUNT account);

    [OperationContract]
    [WebGet(UriTemplate = "/WebLogin/{email}/{password}", ResponseFormat = WebMessageFormat.Json)]
    DTO_ACCOUNT WebLogin(string email,string password);

    [OperationContract]
    [WebGet(UriTemplate = "/WebGetPlayers/{email}", ResponseFormat = WebMessageFormat.Json)]
    DTO_Player[] WebGetPlayers(string email);

    // addes new games to db
    [OperationContract]
    [WebInvoke(UriTemplate = "/WebAddGame", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
    int WebAddGame(DTO_GamePlay game);

    [OperationContract]
    [WebInvoke(UriTemplate = "/ExistGame", RequestFormat = WebMessageFormat.Json)]
    void ExistGame(int gameID);

    // wating games are games that not started yet and wating for opponent player to join thems
    [OperationContract]
    [WebGet(UriTemplate = "/GetWatingGames", ResponseFormat = WebMessageFormat.Json)]
    DTO_GamePlay[] GetWatingGames();

    [OperationContract]
    [WebGet(UriTemplate = "/JoinGame/{hostPlayerID}/{joiningPlayerID}", ResponseFormat = WebMessageFormat.Json)]
    int JoinGame(string hostPlayerID,string joiningPlayerID);

    [OperationContract]
    [WebGet(UriTemplate = "/GetAllPlayers", ResponseFormat = WebMessageFormat.Json)]
    DTO_Player[] GetAllPlayers();

    [OperationContract]
    [WebGet(UriTemplate = "/GetAllGames", ResponseFormat = WebMessageFormat.Json)]
    DTO_GamePlay[] GetAllGames();

    [OperationContract]
    [WebGet(UriTemplate = "/GetGamesByPlayerID/{playerID}", ResponseFormat = WebMessageFormat.Json)]
    DTO_GamePlay[] GetGamesByPlayerID(string playerID);

    [OperationContract]
    [WebGet(UriTemplate = "/GetPlayersByGameID/{gmID}", ResponseFormat = WebMessageFormat.Json)]
    List<DTO_Player> GetPlayersByGameID(string gmID);

    [OperationContract]
    [WebGet(UriTemplate = "/GetForEachPlayerNGames", ResponseFormat = WebMessageFormat.Json)]
    List<DTO_PLAYER_GAMES> GetForEachPlayerNGames();

    [OperationContract]
    [WebGet(UriTemplate = "/GetAllGamesByAccountEmail/{accountEmail}", ResponseFormat = WebMessageFormat.Json)]
    DTO_GamePlay[] GetAllGamesByAccountEmail(string accountEmail);

    [OperationContract]
    [WebInvoke(UriTemplate = "/RemovePlayers", RequestFormat = WebMessageFormat.Json)]
    void RemovePlayers(List<int> playersID);

    [OperationContract]
    [WebInvoke(UriTemplate = "/RemoveGames", RequestFormat = WebMessageFormat.Json)]
    void RemoveGames(List<int> gamesID);

}
public interface IClientCallBack
{
    // called from server whenever game over and there is winner
    [OperationContract(IsOneWay = true)]
    void UpdateClientWithGameWinner(DTO_Player wonPlayer);

    // called from server whenever opponent player eaten this client checker
    [OperationContract(IsOneWay = true)]
    void UpdateClientWithCheckerMove(DTO_Checker checker);

    [OperationContract(IsOneWay = true)]
    void UpdateClientRemoveEatenChecker(string eatenCheckerID);

    // called from server whenever both client called NotifyReadyToPlay method
    [OperationContract(IsOneWay = true)]
    void UpdateClientGameStartOk(DTO_Player whiteCheckersPlayer, DTO_Player blackCheckersPlayer);

    [OperationContract(IsOneWay = true)]
    void CheckClientReady();

    [OperationContract(IsOneWay = true)]
    void NotifyClientOpponentDisconnected();

    // This function dose nothing on client side,used for check from server if client is connected
    // in case service succeed call this function means client is connected.
    [OperationContract(IsOneWay = true)]
    void isConnected();

    // notifies client registation status
    [OperationContract(IsOneWay = true)]
    void NotifyRegistarClientStatus(bool isRegisterSucceed);

}









