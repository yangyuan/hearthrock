// <copyright file="RockBot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot
{
    using System;
    using System.Collections.Generic;

    using Hearthrock.Bot.Score;
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
        public RockAction GetMulliganAction(RockScene scene)
        {
            // You can just return an null or empty list, which means keep all cards.
            //// return null;

            //// Put your codes in here.

            return this.DefaultGetMulliganAction(scene);
        }

        /// <summary>
        /// Generate a play action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be played.</returns>
        public RockAction GetPlayAction(RockScene scene)
        {
            // You can just return an null or empty list, which means ends turn.
            //// return null;

            //// Put your codes in here.

            RockSceneContext context = new RockSceneContext(scene);

            double bestScore = 0d;
            List<int> bestAction = null;
            Random r = new Random();

            foreach (List<int> action in scene.PlayOptions)
            {
                double score = PlayActionScore.ComputeScore(context, action);

                score += r.NextDouble();
                if (score >= bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                }
            }

            if (bestAction != null)
            {
                return RockAction.Create(bestAction);
            }

            return this.DefaultGetPlayAction(scene);
        }

        /// <summary>
        /// The default implementation of GetMulliganAction
        /// </summary>
        /// <param name="scene">the scene</param>
        /// <returns>the cards to be mulligan-ed</returns>
        private RockAction DefaultGetMulliganAction(RockScene scene)
        {
            List<int> cards = new List<int>();
            foreach (RockCard card in scene.Self.Cards)
            {
                if (card.Cost >= 4)
                {
                    cards.Add(card.RockId);
                }
            }

            return RockAction.Create(cards);
        }

        /// <summary>
        /// The default implementation of GetPlayAction
        /// </summary>
        /// <param name="scene">the scene</param>
        /// <returns>the cards to be played</returns>
        private RockAction DefaultGetPlayAction(RockScene scene)
        {
            if (scene.PlayOptions.Count == 0)
            {
                return RockAction.Create();
            }
            else
            {
                // this smart bot will randomly choose an play action.
                return RockAction.Create(scene.PlayOptions[new Random().Next(0, scene.PlayOptions.Count)]);
            }
        }
    }
}
