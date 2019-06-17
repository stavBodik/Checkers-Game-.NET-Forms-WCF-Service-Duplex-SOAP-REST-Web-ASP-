using Client.Model;
using Client.ServiceRef;
using Client.UI;
using Client.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Client.Control
{
    // this class is used to control game play by checkers game rules.
    // also used to play as computer whenever user choose to play locally.
    public static class GameController
    {
        //Dictionary of all checkers found on board (checkerID,checkerPB)
        public static Dictionary<string, CheckerPB> checkers = new Dictionary<string, CheckerPB>(); // list of current checkers playing on board

        public static OpponentPB[] opponents = new OpponentPB[2];
        public static DTO_Player loggedInAccountPlayerPlaying;
        public static bool isPlaying = false;
        public static DTO_ACCOUNT loggedInAccount;
        public static bool isThisPlayerTurn=false;
        public static bool isMouseAllowedToMove = true;
        public static int numberOfeatenCheckers = 0;
        public static int numberOfeatenCheckersByOpponent = 0;
        public static int currentPlayingGameID = -1;
        public static int gameType = Constants.GAME_TYPE_PLAY_ONLINE;
        public static int gameStatus=Constants.GAME_STATUS_NONE;
        

        public static Nullable<int> thisPlayerType = Constants.whiteChecker;

        // This function is called each mouse up event when checker is selected
        // Used to deside and detect game moves such as game over(win/lose)/checker normal moves etc .
        public static bool OnFinishCheckerMove(ref CheckerPB checkerPB,Point newCheckerPosition)
        {

            // this special case when playing vs computer and player won pc , we dont take last computer move
            if (gameStatus == Constants.GAME_STATUS_END && gameType==Constants.GAME_TYPE_GAME_VS_COMPUTER) return false;

            bool isCheckerPositionChanged = true;

            int jumpSize = Math.Abs(newCheckerPosition.Y - checkerPB.checker.oldPositionOnBord.Y);

            if (!isCheckerCanMove(checkerPB, newCheckerPosition))
            {
                checkerPB.moveToOldPosition();
                isCheckerPositionChanged = false;
            }
            else if (jumpSize==Constants.eatOtherCheckerJumpSize)
            {
                CheckerPB eatenChecker = FindEetenChecker(checkerPB,newCheckerPosition);
                if(eatenChecker == null || eatenChecker.checker.checkerType == checkerPB.checker.checkerType)
                {
                    checkerPB.moveToOldPosition();
                    isCheckerPositionChanged= false;
                }
                else
                {
                    //EATE CHECKER
                    RemoveEatenChecker(eatenChecker.checker.ID);
                    UpdateCheckerPosition(checkerPB, newCheckerPosition);
                    DataAcsess.UpdateServerWithCheckerMove(checkerPB.checker, eatenChecker.checker.ID);
                    updateThisClientOppenentVisibility();
                    if (isThisPlayerTurn) { 
                        numberOfeatenCheckers++;
                    }else
                    {
                        numberOfeatenCheckersByOpponent++;
                    }
                }
            }
            else
            {
                UpdateCheckerPosition(checkerPB, newCheckerPosition);
                DataAcsess.UpdateServerWithCheckerMove(checkerPB.checker,null);
                updateThisClientOppenentVisibility();

            }

            if (gameType!=Constants.GAME_TYPE_GAME_RESTORE &&  IsWonGame(newCheckerPosition))
            {
                gameStatus = Constants.GAME_STATUS_END;
                GameForm gameForm = (GameForm)Application.OpenForms["GameForm"];

                if (gameType == Constants.GAME_TYPE_PLAY_ONLINE) {
                    gameForm.OnGameWon(loggedInAccountPlayerPlaying);
                }else
                {
                    if (isThisPlayerTurn) { 
                        gameForm.OnGameWon(new DTO_Player {TEAM_NAME=loggedInAccount.NAME});
                    }else
                    {
                        gameForm.OnGameWon(new DTO_Player { TEAM_NAME = Constants.COMPUTER_TEAM_NAME });
                    }
                }

                DataAcsess.UpdateServerPlayerWon(loggedInAccountPlayerPlaying);
            }

            if ((gameType==Constants.GAME_TYPE_PLAY_ONLINE || gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER))
            {
                 DataAcsess.AddGameMovement(currentPlayingGameID,new DTO_MOVEMENT { Checker_ID= checkerPB.checker.ID,Row=newCheckerPosition.X,Col=newCheckerPosition.Y });
            }

            return isCheckerPositionChanged;
        }
        // checks if one of the players won the game
        private static bool IsWonGame(Point newCheckerPosition)
        {
            if (gameType==Constants.GAME_TYPE_GAME_RESTORE) return false;

            if (   IsPlayerBlocked(thisPlayerType == Constants.whiteChecker ? Constants.blackChecker : Constants.whiteChecker)
                || IsEatenAllOpponnetCheckers()
                || IsGotToOpponnetFirstRow(newCheckerPosition))
            {
                return true;
            }
             return false;
            
        }

        // for current player checks if there is option to eat opponent checker
        // if there is, returns the position player must move else returns -1,-1 
        public static Point isCheckerCanEat(CheckerPB optionalCheckerForMove, Nullable<int> playerType)
        {
            Point currentCheckerPos = optionalCheckerForMove.checker.oldPositionOnBord;

            if (playerType == Constants.whiteChecker)
            {
                // for white checker check 2 directions
                Point positionForCheck1 = new Point(currentCheckerPos.X - 2, currentCheckerPos.Y - 2);
                Point positionForCheck2 = new Point(currentCheckerPos.X - 2, currentCheckerPos.Y + 2);

                CheckerPB eatenCheckerOp1 = null;
                CheckerPB eatenCheckerOp2 = null;

                if (IsCellEmpty(optionalCheckerForMove, positionForCheck1))
                {
                    eatenCheckerOp1 = FindEetenChecker(optionalCheckerForMove, positionForCheck1);
                }

                if (eatenCheckerOp1 == null && IsCellEmpty(optionalCheckerForMove, positionForCheck2))
                {
                    eatenCheckerOp2 = FindEetenChecker(optionalCheckerForMove, positionForCheck2);
                }

                if (eatenCheckerOp1 != null)
                {
                    return new Point(currentCheckerPos.X - 2, currentCheckerPos.Y - 2);
                }
                else if (eatenCheckerOp2 != null)
                {
                    return new Point(currentCheckerPos.X - 2, currentCheckerPos.Y + 2);
                }
            }
            else
            {
                Point positionForCheck1 = new Point(currentCheckerPos.X + 2, currentCheckerPos.Y - 2);
                Point positionForCheck2 = new Point(currentCheckerPos.X + 2, currentCheckerPos.Y + 2);

                CheckerPB eatenCheckerOp1 = null;
                CheckerPB eatenCheckerOp2 = null;

                if (IsCellEmpty(optionalCheckerForMove, positionForCheck1))
                {
                    eatenCheckerOp1 = FindEetenChecker(optionalCheckerForMove, positionForCheck1);
                }

                if (eatenCheckerOp1 == null && IsCellEmpty(optionalCheckerForMove, positionForCheck2))
                {
                    eatenCheckerOp2 = FindEetenChecker(optionalCheckerForMove, positionForCheck2);
                }

                if (eatenCheckerOp1 != null)
                {
                    return new Point(currentCheckerPos.X + 2, currentCheckerPos.Y - 2);
                }
                else if (eatenCheckerOp2 != null)
                {
                    return new Point(currentCheckerPos.X + 2, currentCheckerPos.Y + 2);
                }
            }

            return new Point(-1, -1);
        }
        // sub method to 'isCheckerCanEat' method
        public static Point isThereIsOptionToEat(Nullable<int> playerType)
        {
            //iterate over all checkers of this player type and check if they can eat
            foreach (KeyValuePair<string, CheckerPB> pair in checkers)
            {

                if (pair.Value.checker.checkerType == playerType)
                {
                    Point moveToForEat = isCheckerCanEat(pair.Value, playerType);

                    if (!moveToForEat.Equals(new Point(-1, -1)))
                    {
                        return moveToForEat;
                    }
                }
            }

            return new Point(-1, -1);
        }
        // rest checkers list on game restart .
        public static void clearCheckers()
        {
            foreach (KeyValuePair<string, CheckerPB> pair in checkers)
            {
                pair.Value.Image.Dispose();
                pair.Value.Image = null;
                pair.Value.Dispose();
            }
            checkers.Clear();
        }

        // checks whether the player cannot make any movement , in this case games ends and the blocker is the winner .
        public static bool IsPlayerBlocked(int playerType)
        {
            bool isBlocked = true;
            foreach (KeyValuePair<string, CheckerPB> pair in checkers)
            {
                if (pair.Value.checker.checkerType == Constants.whiteChecker)
                {
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X - 1, pair.Value.checker.oldPositionOnBord.Y - 1))) isBlocked = false;
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X - 1, pair.Value.checker.oldPositionOnBord.Y + 1))) isBlocked = false;
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X - 2, pair.Value.checker.oldPositionOnBord.Y - 2))) isBlocked = false;
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X - 2, pair.Value.checker.oldPositionOnBord.Y + 2))) isBlocked = false;
                }
                else
                {
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X + 1, pair.Value.checker.oldPositionOnBord.Y - 1))) isBlocked = false;
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X + 1, pair.Value.checker.oldPositionOnBord.Y + 1))) isBlocked = false;
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X + 2, pair.Value.checker.oldPositionOnBord.Y - 2))) isBlocked = false;
                    if (isCheckerCanMove(pair.Value, new Point(pair.Value.checker.oldPositionOnBord.X + 2, pair.Value.checker.oldPositionOnBord.Y + 2))) isBlocked = false;
                }
            }

            return isBlocked;
        }

        // updates the current player turn panels visabilty .
        public static void UpdateOpponentsVisabilityOnGameRestore(int movingCheckerType)
        {
            if (movingCheckerType == Constants.whiteChecker)
            {
                opponents[0].Visible = false;
                opponents[1].Visible = true;
            }
            else
            {
                opponents[0].Visible = true;
                opponents[1].Visible = false;
            }
        }

        // This method generates game movement when user plays vs computer
        // The generation of movement is random but smart whenever random movement can eat user checker it dose it .
        public static void PlayAsComputer()
        {
            // check if there is option to eat 
            foreach (KeyValuePair<string, CheckerPB> pair in checkers)
            {
                if (pair.Value.checker.checkerType == Constants.blackChecker)
                {
                    CheckerPB optionalCheckerForMove = pair.Value;
                    Point moveToForEet = isCheckerCanEat(optionalCheckerForMove, Constants.blackChecker);

                    if (!moveToForEet.Equals(new Point(-1, -1)))
                    {
                        OnFinishCheckerMove(ref optionalCheckerForMove, moveToForEet);
                        isThisPlayerTurn = true;
                        return;
                    }

                }
            }


            List<CheckerPB> optionalCheckersMoves = new List<CheckerPB>();
            List<Point> optionalMoves = new List<Point>();

            // find options for simple move 
            foreach (KeyValuePair<string, CheckerPB> pair in checkers)
            {
                if (pair.Value.checker.checkerType == Constants.blackChecker)
                {
                    CheckerPB optionalCheckerForMove = pair.Value;

                    Point moveOption1 = new Point(optionalCheckerForMove.checker.oldPositionOnBord.X + 1, optionalCheckerForMove.checker.oldPositionOnBord.Y - 1);
                    Point moveOption2 = new Point(optionalCheckerForMove.checker.oldPositionOnBord.X + 1, optionalCheckerForMove.checker.oldPositionOnBord.Y + 1);

                    if (isCheckerCanMove(optionalCheckerForMove, moveOption1))
                    {
                        optionalCheckersMoves.Add(optionalCheckerForMove);
                        optionalMoves.Add(moveOption1);
                    }

                    if (isCheckerCanMove(optionalCheckerForMove, moveOption2))
                    {
                        optionalCheckersMoves.Add(optionalCheckerForMove);
                        optionalMoves.Add(moveOption2);

                    }
                }
            }

            //chose random move
            int numberOfOptions = optionalMoves.Count;
            Random rnd = new Random();
            int randomOptionIndex = rnd.Next(0, numberOfOptions - 1);

            CheckerPB checkerForMove = optionalCheckersMoves[randomOptionIndex];
            OnFinishCheckerMove(ref checkerForMove, optionalMoves[randomOptionIndex]);
            isThisPlayerTurn = true;
        }

        private static bool IsGotToOpponnetFirstRow(Point newCheckerPosition)
        {
            if (gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER && newCheckerPosition.X == Constants.NumberOfGameBoardRows - 1)
            {
                return true;
            }
            else if (thisPlayerType == Constants.whiteChecker && newCheckerPosition.X == 0)
            {
                return true;
            }else if(thisPlayerType == Constants.blackChecker && newCheckerPosition.X == Constants.NumberOfGameBoardRows-1)
            {
                return true;
            }

            return false;
        }

        private static bool IsEatenAllOpponnetCheckers()
        {
            bool result=false;

            if (gameType == Constants.GAME_TYPE_PLAY_ONLINE) { 
                result= numberOfeatenCheckers  == Constants.NumberOfGameBoardCols ? true : false;
            }else if(gameType == Constants.GAME_TYPE_GAME_VS_COMPUTER)
            {
                if(numberOfeatenCheckers == Constants.NumberOfGameBoardCols || numberOfeatenCheckersByOpponent == Constants.NumberOfGameBoardCols)
                {
                    result = true;
                }
            }


            return result;
        }

        private static bool isCheckerCanMove(CheckerPB checkerPB, Point newCheckerPosition)
        {
            int moveAngel = CalculateMoveAngel(checkerPB.checker.oldPositionOnBord, newCheckerPosition);
            Boolean isCheckerMovedBack = IsCheckerMovedBack(checkerPB, newCheckerPosition);
            Boolean isCellEmpty = IsCellEmpty(checkerPB, newCheckerPosition);
            int jumpSize = Math.Abs(newCheckerPosition.Y - checkerPB.checker.oldPositionOnBord.Y);

            //critical conditions for not moving
            if (!isCellEmpty || isCheckerMovedBack || (Math.Abs(moveAngel) != 135 && Math.Abs(moveAngel) != 45) || jumpSize >= Constants.maxJumpSize)
            {
                return false;
            }


            return true;
            
        }
        
        // detects whenever checker eaten other checker
        public static CheckerPB FindEetenChecker(CheckerPB checker, Point newCheckerPosition)
        {
            int moveAngel = CalculateMoveAngel(checker.checker.oldPositionOnBord, newCheckerPosition);
            CheckerPB eatenChecker = null;
            Point moveDirection = new Point(Math.Sign((float)(Math.Cos(ToRadians(moveAngel)))), Math.Sign((float)Math.Sin(ToRadians(moveAngel))));

            Point possiblePosition;

            if (checker.checker.checkerType == Constants.blackChecker)
            {

                if (moveDirection.X > 0)
                {
                    possiblePosition = new Point((newCheckerPosition.X - 1), (newCheckerPosition.Y -1));
                }
                else
                {
                    possiblePosition = new Point((newCheckerPosition.X - 1), (newCheckerPosition.Y + 1));

                }

                foreach (KeyValuePair<string, CheckerPB> pair in checkers)
                {
                    if ((pair.Value.checker.oldPositionOnBord == possiblePosition) && (pair.Value.checker.checkerType != checker.checker.checkerType))
                    {
                        CheckerPB ch = pair.Value;
                        return ch;
                    }
                } 
            }
            else
            {
                if (moveDirection.X > 0)
                {
                    possiblePosition = new Point((newCheckerPosition.X + 1), (newCheckerPosition.Y - 1));
                }
                else
                {
                    possiblePosition = new Point((newCheckerPosition.X + 1), (newCheckerPosition.Y + 1));

                }

                foreach (KeyValuePair<string, CheckerPB> pair in checkers)
                {
                    if ((pair.Value.checker.oldPositionOnBord == possiblePosition) && (pair.Value.checker.checkerType!= checker.checker.checkerType))
                    {
                        CheckerPB ch = pair.Value;
                        DTO_Checker c = ch.checker;
                        return ch;
                    }
                }

            }

            return eatenChecker;

        }

        private static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        private static Boolean IsCellEmpty(CheckerPB checker,Point newCheckerPosition)
        {
            // if the cell for check is out of board bounds , do not check 
            // act like !IsCellEmpty ,we wont jump out of board bounds

            if (newCheckerPosition.Y == -1 || newCheckerPosition.Y == Constants.NumberOfGameBoardCols || newCheckerPosition.X == -1 || newCheckerPosition.X == Constants.NumberOfGameBoardRows)
            {
                return false;
            }

            foreach (KeyValuePair<string, CheckerPB> pair in checkers)
            {
                if (pair.Value.checker.oldPositionOnBord.Equals(newCheckerPosition))
                {
                    return false;
                }
            }

            return true;
        }
        // detects whenever checker move was back which is not allowed in game
        private static Boolean IsCheckerMovedBack(CheckerPB checker, Point newCheckerPosition)
        {
            if (checker.checker.checkerType == Constants.blackChecker)
            {
                if ((newCheckerPosition.X - checker.checker.oldPositionOnBord.X) > 0) return false;
                else return true;
            }
            else
            {
                if ((newCheckerPosition.X - checker.checker.oldPositionOnBord.X) < 0) return false;
                else return true;
            }

        }

        // the moving angel of checker from old point to new point
        private static int CalculateMoveAngel(Point p1,Point p2)
        {
            return (int)(Math.Atan2(p2.X - p1.X, p2.Y - p1.Y) * 180.0 / Math.PI);
        }
        public static void UpdateCheckerPosition(CheckerPB ch,Point updatedPosition)
        {

            ch.checker.oldPositionOnBord = updatedPosition;
            ch.FixPosition(updatedPosition);

            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];
            form1Ref.updateMainPanelScreenShoot(ch);
        }
        delegate void RemoveEatenCheckerCallBack(string eatenCheckerID);
        public static void RemoveEatenChecker(string eatenCheckerID)
        {
            if (checkers[eatenCheckerID].InvokeRequired)
            {
                RemoveEatenCheckerCallBack cback = new RemoveEatenCheckerCallBack(RemoveEatenChecker);
                checkers[eatenCheckerID].Invoke(cback, new object[] { eatenCheckerID });
            }
            else
            {

                checkers[eatenCheckerID].Image.Dispose();
                checkers[eatenCheckerID].Image = null;
                checkers[eatenCheckerID].Dispose();
                checkers.Remove(eatenCheckerID);
                GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];
                form1Ref.updateMainPanelScreenShoot(null);

            }
        }

        delegate void updateCurrentOpponentVisibilityCallBack(DTO_Player player);
        public static void updateCurrentOpponentVisibility(DTO_Player player)
        {
            if (GameController.opponents[0].InvokeRequired)
            {
                updateCurrentOpponentVisibilityCallBack cback = new updateCurrentOpponentVisibilityCallBack(updateCurrentOpponentVisibility);
                GameController.opponents[0].Invoke(cback, new object[] { player });
            }
            else
            {
                if (GameController.thisPlayerType == Constants.whiteChecker)
                {
                    GameController.opponents[0].Visible = true;
                    GameController.opponents[1].Visible = false;
                }
                else
                {
                    GameController.opponents[1].Visible = true;
                    GameController.opponents[0].Visible = false;
                }
            }

           
        }

        public static void updateThisClientOppenentVisibility()
        {
            if (gameType == Constants.GAME_TYPE_GAME_RESTORE) return;
            
            if (thisPlayerType == Constants.whiteChecker && isThisPlayerTurn)
            {
                opponents[0].Visible = false;
                opponents[1].Visible = true;
            }
            else
            {
                opponents[1].Visible = false;
                opponents[0].Visible = true;
            }
        }

        delegate void OnOpponentDissconnectedCallBack();
        // when one of the players dissconneted from server (game crash,connection problem etc...)
        public static void OnOpponentDissconnected()
        {
            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];

            if (form1Ref.loadingLabel.InvokeRequired)
            {
                OnOpponentDissconnectedCallBack cback = new OnOpponentDissconnectedCallBack(OnOpponentDissconnected);
                form1Ref.loadingLabel.Invoke(cback, new object[] { });
            }else
            {
                isPlaying = false;
                isThisPlayerTurn = false;
                opponents[0].Visible = false;
                opponents[1].Visible = false;
                form1Ref.loadingLabel.StopAnimation();
                form1Ref.loadingLabel.Visible = true;
                form1Ref.loadingLabel.setTextNoneUiThread(Constants.OPPONENT_DISCONNECTED,Color.Red);
                loggedInAccountPlayerPlaying = null;
            }
        }

        delegate void showOpponnetsOnGameStartCallBack();
        public static void showOpponnetsOnGameStart()
        {
            if (opponents[0].InvokeRequired)
            {
                showOpponnetsOnGameStartCallBack cback = new showOpponnetsOnGameStartCallBack(showOpponnetsOnGameStart);
                opponents[0].Invoke(cback, new object[] {});
            }
            else
            {
                opponents[0].Visible = true;
                opponents[1].Visible = false;
                GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];
                form1Ref.loadingLabel.StopAnimation();
                form1Ref.loadingLabel.Visible = false;
            }
        }
      
    }
}
