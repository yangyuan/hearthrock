// <copyright file="RockActionContext.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System.Collections.Generic;

    using Hearthrock.Contracts;
    using Hearthrock.Pegasus;

    /// <summary>
    /// Context for RockAction
    /// </summary>
    public class RockActionContext
    {
        /// <summary>
        /// The RockAction.
        /// </summary>
        private List<int> rockAction;

        /// <summary>
        /// The IRockPegasus.
        /// </summary>
        private IRockPegasus pegasus;

        /// <summary>
        /// The steps.
        /// </summary>
        private int step;

        public string Interpretion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RockActionContext" /> class.
        /// </summary>
        /// <param name="rockAction">The rockAction</param>
        /// <param name="pegasus">The IRockPegasus.</param>
        public RockActionContext(List<int> rockAction, IRockPegasus pegasus)
        {

            if (rockAction == null)
            {
                this.rockAction = new List<int>();
            }
            else
            {
                this.rockAction = rockAction;
            }

            this.pegasus = pegasus;
            this.step = 0;
            this.Interpretion = this.GetInterpretion();
        }

        public string GetInterpretion()
        {
            if (this.rockAction.Count == 0)
            {
                return string.Empty;
            }

            var sourceObject = this.pegasus.GetObject(this.rockAction[0]);
            if (this.rockAction.Count == 1)
            {
                return "Play: " + sourceObject.Name;
            }
            else
            {
                var targetEnities = new List<RockPegasusObject>();

                for (int i=1; i< this.rockAction.Count; i++)
                {
                    var rockId = this.rockAction[i];
                    targetEnities.Add(this.pegasus.GetObject(rockId));
                }
             

                string ret = "Attack: " + sourceObject.Name + " ";
                foreach (var targetEnity in targetEnities)
                {
                    ret += " > " + targetEnity.Name;
                }

                return ret;
            }
        }

        public void Apply()
        {
            // Pick source card
            if (this.step == 0)
            {
                this.pegasus.ClickObject(this.rockAction[0]);

                this.step = 1;
                return;
            }

            if (this.step == 1 && this.rockAction.Count == 1)
            {
                this.pegasus.DropObject();

                this.step = 2;
                return;
            }

            // other scenarios
            if (this.rockAction.Count > this.step)
            {
                this.pegasus.ClickObject(this.rockAction[this.step]);
                this.step++;
                return;
            }
            else
            {
                this.pegasus.DropObject();
                this.step++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsInvalid()
        {
            if (this.rockAction.Count == 0)
            {
                return true;
            }

            foreach (var rockId in this.rockAction)
            {
                if (null == this.pegasus.GetObject(rockId))
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
            return (this.step > this.rockAction.Count);
        }


        public void ApplyAll()
        {
            foreach (int cardId in this.rockAction)
            {
                var card = this.pegasus.GetObject(cardId);
                if (card.CardId == "GAME_005") continue;
                this.pegasus.ClickObject(card.RockId);
            }

            this.step = this.rockAction.Count + 1;
        }
    }
}
