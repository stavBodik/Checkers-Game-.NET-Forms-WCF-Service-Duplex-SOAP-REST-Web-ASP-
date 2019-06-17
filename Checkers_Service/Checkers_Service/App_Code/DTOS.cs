using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WcfService.App_Code;

namespace WcfService.App_Code
{
    [DataContract]
    public class DTO_MOVEMENT
    {
        [DataMember]
        public int Id;

        [DataMember]
        public string Checker_ID;

        [DataMember]
        public int Row;

        [DataMember]
        public int Col;

        [DataMember]
        public int Game_ID;

    }


    [DataContract]
    public class DTO_GamePlay
    {
        [DataMember]
        public int Key { get; set; }

        [DataMember]
        public string gameName {get;set;}

        [DataMember]
        public DateTime createDate { get; set; }

        [DataMember]
        public string hostPlayerName { get; set; }

        [DataMember]
        public int hostPlayerID { get; set; }

        [DataMember]
        public string hostPlayerAccountEmail { get; set; }

        [DataMember]
        public System.Nullable<int> joinedPlayerID { get; set; }

        [DataMember]
        public string joinPlayerName { get; set; }

        [DataMember]
        public Nullable<int> Status { get; set; }
    }

    public class DTO_Player 
    {
        [DataMember]
        public int Id;

        [DataMember]
        public string AccountEmail;

        [DataMember]
        public string FirstName;

        [DataMember]
        public string LastName;

        [DataMember]
        public System.Nullable<int> Type;

        [DataMember]
        public System.Nullable<int> isDeleted;

        [DataMember]
        public string TEAM_NAME;
    }

    [DataContract]
    public class DTO_ACCOUNT
    {
        [DataMember]
        public string EMAIL;

        [DataMember]
        public string PASSWORD;

        [DataMember]
        public string NAME;

        [DataMember]
        public DTO_Player[] players;

    }

    [DataContract]
    public class DTO_RESTORED_GAME
    {
        [DataMember]
        public DTO_MOVEMENT[] movements;

        [DataMember]
        public DTO_Player[] players;

        [DataMember]
        public Nullable<int> gameStatus;
    }

    public class DTO_PLAYER_GAMES
    {
        public int ID;
        public string PlayerName;
        public int Games;

    }

    [DataContract]
    public class DTO_Checker
    {
        [DataMember]
        public Point oldPositionOnBord { get; set; }

        [DataMember]
        public int checkerType { get; set; }

        [DataMember]
        public String ID { get; set; }

    }
}

