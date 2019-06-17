using Client.Control;
using Client.ServiceRef;
using Client.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.View
{
    // this class used to show restored game moves animation.
    public class GameRestoreAnimator
    {
        private DTO_RESTORED_GAME restoredGame;
        private Timer animationTimer;
        private int animationProgress = 0;
        public GameRestoreAnimator(DTO_RESTORED_GAME restoredGame)
        {
            this.restoredGame = restoredGame;
            animationTimer = new Timer();
            animationTimer.Interval = Constants.restoreAnimationSpeed;
            animationTimer.Tick += new EventHandler(Animation);
        }

        private void Animation(object Sender, EventArgs e)
        {
            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];
            DTO_MOVEMENT movement = restoredGame.movements[animationProgress];
            CheckerPB checker = GameController.checkers[movement.Checker_ID];
            
            form1Ref.animationHolder.initAnimation(new Point(movement.Col * form1Ref.getCellSize().X, (movement.Row * form1Ref.getCellSize().Y) + form1Ref.getCellSize().Y));
            form1Ref.animationHolder.startAnimation();

            GameController.OnFinishCheckerMove(ref checker,new Point(movement.Row,movement.Col));
            GameController.UpdateOpponentsVisabilityOnGameRestore(checker.checker.checkerType);
            animationProgress++;

            if (animationProgress == restoredGame.movements.Length)
            {
                StopAnimation();
                form1Ref.animationHolder.stopAnimation();

               // if (restoredGame.gameStatus == Constants.GAME_STATUS_END) {

                    if (checker.checker.checkerType == Constants.whiteChecker)
                    {
                        form1Ref.OnGameWon(restoredGame.players[0]);
                    }
                    else
                    {
                        form1Ref.OnGameWon(restoredGame.players[1]);
                    }
             //   }
            }
        }

        public void StartAnimation()
        {
            animationProgress = 0;
            animationTimer.Enabled = true;
        }

        public void StopAnimation()
        {
            GameForm form1Ref = (GameForm)Application.OpenForms["GameForm"];
            form1Ref.animationHolder.stopAnimation();
            animationTimer.Enabled = false;
            GameController.gameType = Constants.GAME_TYPE_PLAY_ONLINE;
            GameController.opponents[0].Visible = false;
            GameController.opponents[1].Visible = false;

        }

    }
}
