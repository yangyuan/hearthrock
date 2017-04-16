// <copyright file="PlayActionScore.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using System.Collections.Generic;
    using Hearthrock.Contracts;
    using System;

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
                        if (sceneContext.IsEnemyMinion(playAction[1]))
                        {
                            // Use Minion Attack
                            return ScoreForMinionAttack(sceneContext, playAction[0], playAction[1]);
                        }

                        return ScoreForMinionAttackHero(sceneContext, playAction[0], playAction[1]);
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
            RockCard card = sceneContext.GetRockCard(cardRockId);

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
            RockCard card = sceneContext.GetRockCard(cardRockId);

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
            RockCard card = sceneContext.GetRockCard(cardRockId);
            
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
        private static double ScoreForMinionAttackHero(RockSceneContext sceneContext, int sourceRockId, int targetRockId)
        {
            var targetHero = sceneContext.GetRockHero(targetRockId);
            var sourceMinion = sceneContext.GetRockMinion(sourceRockId);

            double score = 0d;

            bool canKill = sourceMinion.Damage >= targetHero.Health;

            if (canKill)
            {
                score += 8;
            }
            else
            {
                score += sourceMinion.Damage;
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
            var targetMinion = sceneContext.GetRockMinion(targetRockId);
            var sourceMinion = sceneContext.GetRockMinion(sourceRockId);

            double score = 0d;

            bool canKill = false;
            if (!targetMinion.HasDivineShield)
            {
                canKill = sourceMinion.Damage >= targetMinion.Health;
            }

            bool canSurvive = true;
            if (!sourceMinion.HasDivineShield)
            {
                canSurvive = targetMinion.Damage <= sourceMinion.Health;
            }

            double benifit = Math.Pow(targetMinion.Damage + targetMinion.Health, 1.5d)
                - Math.Pow(sourceMinion.Damage - sourceMinion.Health, 1.5d);

            if (targetMinion.HasWindfury)
            {
                benifit += targetMinion.Damage;
            }

            if (sourceMinion.HasWindfury)
            {
                benifit -= sourceMinion.Damage;
            }

            if (targetMinion.HasAura)
            {
                benifit += 2;
            }

            if (sourceMinion.HasAura)
            {
                benifit -= 2;
            }

            if (canKill)
            {
                score += targetMinion.Damage;
            }
            else if (canSurvive)
            {
                score += sourceMinion.Damage;
            }

            if (canKill && canSurvive)
            {
                score += benifit;
            }
            else if (!canKill && canSurvive)
            {
                score += benifit * 0.625;
            }
            else if (canKill && !canSurvive)
            {
                score += benifit * 0.475;
            }
            else // !canKill && !canSurvive
            {
                score += 0;
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
            RockCard card = sceneContext.GetRockCard(cardRockId);

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.1d;

            return score;
        }
    }
}
