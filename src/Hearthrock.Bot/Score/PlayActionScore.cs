// <copyright file="PlayActionScore.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using System;
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
                        switch (sceneContext.GetFriendlyRockPlayer().Hero.Class)
                        {
                            case RockHeroClass.Mage:
                                return ScoreForUseMageHeroPower(sceneContext, playAction[0], playAction[1]);
                            case RockHeroClass.Priest:
                                return ScoreForUsePriestHeroPower(sceneContext, playAction[0], playAction[1]);
                            default:
                                return 0d;
                        }
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
            RockCard card = sceneContext.GetRockCard(cardRockId);

            if (card.CardId == "GAME_005")
            {
                // the coin
                foreach (RockCard handCard in sceneContext.Scene.Self.Cards)
                {
                    if (handCard.Cost == sceneContext.Scene.Self.Resources + 1)
                    {
                        return 4.5;
                    }
                }

                return -4;
            }

            double score = 4;

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.4d;

            if (sceneContext.Scene.Self.Cards.Count >= 5)
            {
                score += 0.5;
            }

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
            double score = 4; // initial score
            RockCard card = sceneContext.GetRockCard(cardRockId);

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.4d;

            return score;
        }

        /// <summary>
        /// Compute score for using a HeroPower.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="cardRockId">The RockId of HeroPower</param>
        /// <param name="targetRockId">The RockId of Target</param>
        /// <returns>The score in double</returns>
        private static double ScoreForUseMageHeroPower(RockSceneContext sceneContext, int cardRockId, int targetRockId)
        {
            double score = 4; // initial score
            RockCard card = sceneContext.GetRockCard(cardRockId);
            
            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.4d;

            var target = sceneContext.GetRockObject(targetRockId);
            switch (target.ObjectType)
            {
                case RockObjectType.FriendlyHero:
                    score -= 5d;
                    break;
                case RockObjectType.FriendlyMinion:
                    score -= 5d;
                    break;
                case RockObjectType.EnemyHero:
                    score += 0.1d;
                    break;
                case RockObjectType.EnemyMinion:
                    score += 0.1d;
                    if (((RockMinion)target.Object).Health < 2)
                    {
                        score += ((RockMinion)target.Object).Damage;
                    }

                    break;
                default:
                    break;
            }

            return score;
        }

        /// <summary>
        /// Compute score for using a HeroPower.
        /// </summary>
        /// <param name="sceneContext">The RockSceneContext</param>
        /// <param name="cardRockId">The RockId of HeroPower</param>
        /// <param name="targetRockId">The RockId of Target</param>
        /// <returns>The score in double</returns>
        private static double ScoreForUsePriestHeroPower(RockSceneContext sceneContext, int cardRockId, int targetRockId)
        {
            double score = 4; // initial score
            RockCard card = sceneContext.GetRockCard(cardRockId);

            // adjust score by wasted cost
            int wastedCost = sceneContext.GetMininWastedCost(cardRockId);
            score -= wastedCost * 0.4d;

            var target = sceneContext.GetRockObject(targetRockId);
            switch (target.ObjectType)
            {
                case RockObjectType.EnemyHero:
                    score -= 5d;
                    break;
                case RockObjectType.EnemyMinion:
                    // but in some special case, we will want to heal them
                    score -= 5d;
                    break;
                case RockObjectType.FriendlyHero:
                    RockHero hero = (RockHero)target.Object;
                    if (hero.Health < 30)
                    {
                        score += 0.5d;
                    }
                    else
                    {
                        return 0;
                    }

                    score += (30 - hero.Health) * 0.05;
                    
                    break;
                case RockObjectType.FriendlyMinion:
                    RockMinion minion = (RockMinion)target.Object;
                    if (minion.Health < minion.BaseHealth)
                    {
                        score += minion.Damage * 0.4d;
                    }

                    break;
                default:
                    break;
            }

            return score;
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

            switch (card.CardType)
            {
                case RockCardType.Enchantment:
                    if (sceneContext.IsEnemy(targetRockId))
                    {
                        score -= 1;
                    }

                    break;
                case RockCardType.Spell:
                    if (sceneContext.IsFriendly(targetRockId))
                    {
                        score -= 1;
                    }

                    break;
                case RockCardType.Weapon:
                    if (sceneContext.GetFriendlyRockPlayer().HasWeapon)
                    {
                        score -= 1;
                    }
                    else
                    {
                        score += 4;
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

            double score = 4d;

            bool canKill = sourceMinion.Damage >= targetHero.Health;

            if (canKill)
            {
                score += 4;
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

            double benifit = Math.Pow(targetMinion.Damage, 1.5d) + targetMinion.Health
                - Math.Pow(sourceMinion.Damage, 1.5d) - sourceMinion.Health;

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

            if (sourceMinion.HasTaunt)
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
            else
            {
                score += benifit * 0.125;

                if (sourceMinion.HasAura)
                {
                    score -= 2;
                }

                if (sourceMinion.HasTaunt)
                {
                    score -= 2;
                }

                // !canKill && !canSurvive
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
    }
}
