// <copyright file="PlayActionScore.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using System.Collections.Generic;
    using Hearthrock.Contracts;

    /// <summary>
    /// The class to calculate the score of the action
    /// </summary>
    public static class PlayActionScore
    {
        /// <summary>
        /// Compute score for one of the PlayAction.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext.</param>
        /// <param name="playAction">The PlayAction.</param>
        /// <returns>The score in double.</returns>
        public static double ComputeScore(RockSceneContext sceneContext, List<int> playAction)
        {
            switch (playAction.Count)
            {
                // Use Card (Spell/Enchantment/Minion/Weapon)
                // Use HeroPower
                case 1:
                    if (sceneContext.IsFriendlyCard(playAction[0]))
                    {
                        // Use Card
                        return ScoreForUseCard(sceneContext, playAction[0]);
                    }
                    else if (sceneContext.IsFriendlyHeroPower(playAction[0]))
                    {
                        // Use HeroPower
                        return ScoreForUseHeroPower(sceneContext, playAction[0]);
                    }
                    else
                    {
                        return 0;
                    }

                case 2:
                    if (sceneContext.IsFriendlyCard(playAction[0]))
                    {
                        // Use Spell/Enchantment on target
                        // Use Card with option
                        return 0;
                    }
                    else if (sceneContext.IsFriendlyHeroPower(playAction[0]))
                    {
                        // Use HeroPower on target
                        return ScoreForUseHeroPowerOnTarget(sceneContext, playAction[0], playAction[1]);
                    }
                    else if (sceneContext.IsFriendlyMinion(playAction[0]))
                    {
                        // Use Minion Attack
                        return ScoreForMinionAttack(sceneContext, playAction[0], playAction[1]);
                    }
                    else if (sceneContext.IsObjectType(playAction[0], RockObjectType.FriendlyHeroWeapon))
                    {
                        // HeroWeapon Attack
                        return ScoreForWeaponAttack(sceneContext, playAction[0], playAction[1]);
                    }
                    else
                    {
                        return 0;
                    }

                case 3:
                    {
                        // Use Card with option and on target
                    }

                    break;
            }

            return 0;
        }

        /// <summary>
        /// Compute score for using a Card.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="cardRockId">The RockId of Card</param>
        /// <returns>The score in double</returns>
        private static double ScoreForUseCard(RockSceneContext sceneContext, int cardRockId)
        {
            double score = 1; // initial score
            RockCard card = sceneContext.GetCard(cardRockId);

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.1d;

            return score;
        }

        /// <summary>
        /// Compute score for using a HeroPower.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="cardRockId">The RockId of HeroPower</param>
        /// <returns>The score in double</returns>
        private static double ScoreForUseHeroPower(RockSceneContext sceneContext, int cardRockId)
        {
            double score = 1; // initial score
            RockCard card = sceneContext.GetCard(cardRockId);

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.1d;

            return score;
        }

        /// <summary>
        /// Compute score for using a HeroPower.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="cardRockId">The RockId of HeroPower</param>
        /// <param name="targetRockId">The RockId of Target</param>
        /// <returns>The score in double</returns>
        private static double ScoreForUseHeroPowerOnTarget(RockSceneContext sceneContext, int cardRockId, int targetRockId)
        {
            double score = 1; // initial score
            RockCard card = sceneContext.GetCard(cardRockId);
            
            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.1d;

            var target = sceneContext.GetRockObject(targetRockId);
            switch (target.ObjectType)
            {
                case RockObjectType.FriendlyHero:
                    score -= 1.1d;
                    break;
                case RockObjectType.FriendlyMinion:
                    score -= 1.1d;
                    break;
                case RockObjectType.EnemyHero:
                    score += 0.1d;
                    break;
                case RockObjectType.EnemyMinion:
                    score += 0.1d;
                    if (((RockMinion)target.Object).Health < 2)
                    {
                        score += 0.5d;
                    }

                    break;
                default:
                    break;
            }

            return score;
        }

        /// <summary>
        /// Compute score for attacking a target.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="sourceRockId">The RockId of attacker</param>
        /// <param name="targetRockId">The RockId of target</param>
        /// <returns>The score in double</returns>
        private static double ScoreForMinionAttack(RockSceneContext sceneContext, int sourceRockId, int targetRockId)
        {
            return 0;
        }

        /// <summary>
        /// Compute score for attacking a target.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="sourceRockId">The RockId of attacker</param>
        /// <param name="targetRockId">The RockId of target</param>
        /// <returns>The score in double</returns>
        private static double ScoreForWeaponAttack(RockSceneContext sceneContext, int sourceRockId, int targetRockId)
        {
            return 0;
        }

        /// <summary>
        /// Compute score for using a Card on target.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="cardRockId">The RockId of Card</param>
        /// <param name="targetRockId">The RockId of Target</param>
        /// <returns>The score in double</returns>
        private static double ScoreForUseCardOnTarget(RockSceneContext sceneContext, int cardRockId, int targetRockId)
        {
            double score = 1; // initial score
            RockCard card = sceneContext.GetCard(cardRockId);

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.1d;

            return score;
        }
    }
}
