// <copyright file="RockEngineAction.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System.Collections.Generic;

    using Hearthrock.Contracts;
    using Hearthrock.Hooks;
    using Hearthrock.Pegasus;

    /// <summary>
    /// Action for RockEngine
    /// </summary>
    public class RockEngineAction
    {
        /// <summary>
        /// The RockAction.
        /// </summary>
        private List<int> actions;

        /// <summary>
        /// The IRockPegasus.
        /// </summary>
        private IRockPegasus pegasus;

        /// <summary>
        /// The current step index.
        /// </summary>
        private int step;

        /// <summary>
        /// The current step index.
        /// </summary>
        private int slot;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockEngineAction" /> class.
        /// </summary>
        /// <param name="pegasus">The IRockPegasus.</param>
        /// <param name="action">The RockIds of the action.</param>
        /// <param name="slot">The target slot when apply the action.</param>
        public RockEngineAction(IRockPegasus pegasus, List<int> action, int slot)
        {
            if (action == null)
            {
                this.actions = new List<int>();
            }
            else
            {
                this.actions = action;
            }

            this.slot = slot;

            this.pegasus = pegasus;
            this.step = 0;
            this.Interpretation = this.GetInterpretation();
        }

        /// <summary>
        /// Gets the interpretation of this action.
        /// </summary>
        public string Interpretation { get; private set; }

        /// <summary>
        /// Apply one step.
        /// </summary>
        public void Apply()
        {
            RockGameHooks.PlayZoneSlotMousedOverValue = this.slot;
            RockGameHooks.EnablePlayZoneSlotMousedOver = true;

            // Pick source card
            if (this.step == 0)
            {
                this.pegasus.ClickObject(this.actions[0]);
            }
            else if (this.step == 1 && this.actions.Count == 1)
            {
                this.pegasus.DropObject();
            }
            else if (this.actions.Count > this.step)
            {
                this.pegasus.ClickObject(this.actions[this.step]);
            }
            else
            {
                this.pegasus.DropObject();
            }

            this.step++;

            RockGameHooks.EnablePlayZoneSlotMousedOver = false;
            RockGameHooks.PlayZoneSlotMousedOverValue = -1;

            return;
        }

        /// <summary>
        /// Is this action valid.
        /// </summary>
        /// <returns>True if this action is valid.</returns>
        public bool IsValid()
        {
            if (this.actions.Count == 0)
            {
                return false;
            }

            foreach (var rockId in this.actions)
            {
                if (this.pegasus.GetObject(rockId) == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Is this action done.
        /// </summary>
        /// <returns>True if this action is done.</returns>
        public bool IsDone()
        {
            return this.step > this.actions.Count;
        }

        /// <summary>
        /// Apply all steps.
        /// </summary>
        public void ApplyAll()
        {
            foreach (int cardId in this.actions)
            {
                var card = this.pegasus?.GetObject(cardId);
                if (card != null)
                {
                    if (card.CardId == "GAME_005")
                    {
                        continue;
                    }

                    this.pegasus?.ClickObject(card.RockId);
                }
            }

            this.step = this.actions.Count + 1;
        }

        /// <summary>
        /// Get the interpretation of this action.
        /// </summary>
        /// <returns>The interpretation of this action.</returns>
        private string GetInterpretation()
        {
            if (this.actions.Count == 0)
            {
                return string.Empty;
            }

            var sourceObject = this.pegasus.GetObject(this.actions[0]);
            if (this.actions.Count == 1)
            {
                return "Play: " + sourceObject.Name;
            }
            else
            {
                var targetEnities = new List<IRockObject>();

                for (int i = 1; i < this.actions.Count; i++)
                {
                    var rockId = this.actions[i];
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
    }
}
