using Hearthrock.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearthrock.Robot
{
    class RockActionContext
    {
        RockAction rockAction;
        int step;

        public RockActionContext(RockAction rockAction)
        {
            this.rockAction = rockAction;
            this.step = 0;
        }

        public void Apply(GameState gameState)
        {
            // Pick source card
            if (this.step == 0)
            {
                //RockInputManager.DisableInput();
                RockInputManager.ClickCard(GetCard(gameState, this.rockAction.Source));

                this.step = 1;
                return;
            }

            if (this.step == 1 && this.rockAction.Targets.Count == 0)
            {
                InputManager.Get().DoNetworkResponse(GetCard(gameState, this.rockAction.Source).GetEntity(), true);
                RockInputManager.DropCard();
                //RockInputManager.EnableInput();

                this.step = 2;
                return;
            }

            // other scenarios
            if (this.rockAction.Targets.Count >= this.step)
            {
                RockInputManager.ClickCard(GetCard(gameState, this.rockAction.Targets[this.step - 1]));
                this.step++;
                return;
            }
            else
            {
                RockInputManager.DropCard();
                //RockInputManager.EnableInput();
                this.step++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsInvalid(GameState gameState)
        {
            if (null == GetCard(gameState, this.rockAction.Source))
            {
                return true;
            }

            foreach (var rockId in this.rockAction.Targets)
            {
                if (null == GetCard(gameState, rockId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
        {
            return (this.step > this.rockAction.Targets.Count + 1);
        }

        public static Card GetCard(GameState gameState, int rockId)
        {
            return GameState.Get().GetEntity(rockId)?.GetCard();
        }
    }
}
