using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfService.App_Code
{
    public class Application_Constants
    {
        //player type
        public  const int blackChecker = 0;
        public  const int whiteChecker = 1;

        //game statuses
        public const int nPlayersForGameStart = 2;
        public const int GAME_STATUS_WATING_FOR_PLAYER = 0;
        public const int GAME_STATUS_WATING_FOR_BEGIN = 1;
        public const int GAME_STATUS_END = 2;
        public const int GAME_STATUS_PLAYING = 3;
        public const int GAME_STATUS_DISCONNECTED = 4;

        //game master player id
        public const int GM_PLAYER_ID = 1;

    }
}