using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Constants
    { 
        // game board settings
        public const int NumberOfGameBoardRows = 8;
        public const int NumberOfGameBoardCols = 4;
        public const int CheckerMoveOffsetOfBoardLimits = 5;
        public const int UI_EXERCISE_DELAY_TIME = 3000;
        
        // restore game steps speed animation
        public const int restoreAnimationSpeed = 500;

        //checker type
        public const int blackChecker = 0;
        public const int whiteChecker = 1;

        // gameboard row type (starts with white cell or black cell)
        public const int evenRow = 0;
        public const int oddRow = 1;


        //game rules
        public const int eatOtherCheckerJumpSize = 2;
        public const int maxJumpSize = 3;

        // strings
        public const string LOGIN = "Account login";
        public const string waitOpponent=    "Wating for opponent ";
        public const string EMAIL_FORMAT =   "Please input correct email";
        public const string EMAIL_EMTPY =    "Please input email";
        public const string PASSWORD_EMTPY = "Please input email";
        public const string WRONG_LOGIN =    "Wrong login information";
        public const string EMAIL_REGEX = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string OPPONENT_DISCONNECTED = "Connection Problem,Please Start new game.";
        public const string ALLREDY_CONNECTED_TO_GAME = "You Allready connected to this game";
        public const string GAME_WON1 = " Team : ";
        public const string GAME_WON2 = " Won The Game !";
        public const string CHOSE_GAME_RESTORE = "Please choose game for restore :";
        public const string CHOSE_LOCAL_GAME_PLAYER = "Please choose player :";

        public const string RESTORE_GAME = "Restore game";
        public const string GAME_KEY_NUMERICAL = "Game key must be an number";
        public const string PLEASE_ENTER_KEY = "\nPlease type game key for start playing online : ";
        public const string COMPUTER_TEAM_NAME = "Checkers";

        // login hashing password salt
        public const string PASSWORD_HASH_SALT = "C@CKER@S";

        // menu strings
        public const string menuS = "Menu";
        public const string loginS = "Login";
        public const string logoutS = "Logout";
        public const string aboutS = "About";
        public const string joingameS = "Play Online Game";
        public const string restoregameS = "Restore previous game";
        public const string playS = "Play";
        public const string uiExercise = "UI Thread Exercise";


        

        // game statuses
        public const int GAME_STATUS_WATING_FOR_PLAYER = 0;
        public const int GAME_STATUS_WATING_FOR_BEGIN = 1;
        public const int GAME_STATUS_END = 2;
        public const int GAME_STATUS_PLAYING = 3;
        public const int GAME_STATUS_DISCONNECTED = 4;
        public const int GAME_STATUS_NONE =-1;


        //current game type
        public const int GAME_TYPE_GAME_VS_COMPUTER = 1;
        public const int GAME_TYPE_GAME_RESTORE = 2;
        public const int GAME_TYPE_PLAY_ONLINE = 3;

        //game master player id
        public const int GM_PLAYER_ID = 1;
    }
}
