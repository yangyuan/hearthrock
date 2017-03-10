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
    class RockActionContext
    {
        /// <summary>
        /// The RockAction.
        /// </summary>
        RockAction rockAction;

        /// <summary>
        /// The IRockPegasus.
        /// </summary>
        IRockPegasus pegasus;

        /// <summary>
        /// The steps.
        /// </summary>
        int step;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockActionContext" /> class.
        /// </summary>
        public RockActionContext(RockAction rockAction, IRockPegasus pegasus)
        {
            this.rockAction = rockAction;

            if (this.rockAction.Targets == null)
            {
                this.rockAction.Targets = new List<int>();
            }

            this.pegasus = pegasus;
            this.step = 0;
        }

        public string Interpretion()
        {
            var sourceObject = this.pegasus.GetObject(this.rockAction.Source);
            if (this.rockAction.Targets.Count == 0)
            {
                return "Play: " + sourceObject.Name;
            }
            else
            {
                var targetEnities = new List<RockPegasusObject>();
                foreach (var rockId in this.rockAction.Targets)
                {
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
                this.pegasus.ClickObject(this.rockAction.Source);

                this.step = 1;
                return;
            }

            if (this.step == 1 && this.rockAction.Targets.Count == 0)
            {
                this.pegasus.DropObject();

                this.step = 2;
                return;
            }

            // other scenarios
            if (this.rockAction.Targets.Count >= this.step)
            {
                this.pegasus.ClickObject(this.rockAction.Targets[this.step - 1]);
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
            if (null == this.pegasus.GetObject(this.rockAction.Source))
            {
                return true;
            }

            foreach (var rockId in this.rockAction.Targets)
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
            return (this.step > this.rockAction.Targets.Count + 1);
        }
    }
}
