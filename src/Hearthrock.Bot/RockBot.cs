// <copyright file="RockBot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot
{
    using System;
    using System.Collections.Generic;

    using Hearthrock.Contracts;

    /// <summary>
    /// The build-in bot for hearthrock.
    /// </summary>
    public class RockBot : IRockBot
    {
        /// <summary>
        /// Generate a mulligan action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be mulligan-ed.</returns>
        public List<int> GetMulliganAction(RockScene scene)
        {
            // You can just return an null or empty list, which means keep all cards.
            //// return null;

            List<int> cards = new List<int>();
            foreach (RockCard card in scene.Self.Cards)
            {
                if (card.Cost >= 4)
                {
                    cards.Add(card.RockId);
                }
            }

            return cards;
        }

        /// <summary>
        /// Generate a play action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be played.</returns>
        public List<int> GetPlayAction(RockScene scene)
        {            
            // You can just return an null or empty list, which means ends turn.
            //// return null;

            if (scene.PlayOptions.Count == 0)
            {
                return null;
            }
            else
            {
                // this smart bot will randomly choose an play action.
                return scene.PlayOptions[new Random().Next(0, scene.PlayOptions.Count)];
            }
        }
    }
}
