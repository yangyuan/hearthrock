// <copyright file="RockBot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot
{
    using System.Collections.Generic;
    using Hearthrock.Contracts;

    public class RockBot : IRockBot
    {
        public List<RockCard> GetMulligan(RockScene scene)
        {
            // return null; // You can just return an null or empty list, which means keep all cards.

            List<RockCard> cards = new List<RockCard>();
            foreach(RockCard card in scene.Self.Cards)
            {
                if (card.Cost >= 4)
                {
                    cards.Add(card);
                }
            }

            return cards;
        }

        public RockAction GetAction(RockScene scene)
        {
            // return null; // You can just return null, which means no action.

            RockAction action = null;

            var player = scene.Self;
            var enemyPlayer = scene.Opponent;

            var hero = player.Hero;
            var enemyHero = enemyPlayer.Hero;

            int resources = player.Resources;
            var cards = player.Cards;
            var minions = player.Minions;
            var power = player.Power;

            var enemyMinions = enemyPlayer.Minions;

            // Use a few lists to find best attackers and best targets
            var enemyMinionsWithTaunt = new List<RockMinion>();
            var enemyMinionsDangerous = new List<RockMinion>();
            var enemyMinionsGreatDangerous = new List<RockMinion>();
            var minionsWithoutTaunt = new List<RockMinion>();
            var minionsWithTaunt = new List<RockMinion>();
            var minionsAttacker = new List<RockMinion>();

            // The damage enemy can do in the next turn
            int enemyNextTurnDamage = RockBotHelper.ComputeEnemyNextTurnDamage(enemyHero, enemyMinions);

            foreach (var enemyMinion in enemyMinions)
            {
                if (enemyMinion.CanBeAttacked && !enemyMinion.IsStealthed)
                {
                    if (enemyMinion.HasTaunt)
                    {
                        enemyMinionsWithTaunt.Add(enemyMinion);
                    }
                    else if (RockBotHelper.IsEnemyMinoinDangerous(enemyMinion))
                    {
                        enemyMinionsDangerous.Add(enemyMinion);
                    }

                    if (RockBotHelper.IsEnemyMinoinGreatDangerous(enemyMinion))
                    {
                        enemyMinionsGreatDangerous.Add(enemyMinion);
                    }
                }
            }

            foreach (var minion in minions)
            {
                if (minion.CanAttack && !minion.IsExhausted && !minion.IsFrozen && !minion.IsAsleep && minion.Damage > 0)
                {
                    if (minion.HasTaunt)
                    {
                        minionsWithTaunt.Add(minion);
                    }
                    else
                    {
                        minionsWithoutTaunt.Add(minion);
                    }

                    minionsAttacker.Add(minion);
                }
            }

            enemyMinions.Sort(RockBotHelper.MinoinPowerCompare);
            enemyMinionsWithTaunt.Sort(RockBotHelper.MinoinPowerCompare);
            minionsWithoutTaunt.Sort(RockBotHelper.MinoinPowerCompare);
            minionsWithoutTaunt.Reverse();
            minionsWithTaunt.Sort(RockBotHelper.MinoinPowerCompare);
            minionsWithTaunt.Reverse();
            minionsAttacker.Sort(RockBotHelper.MinoinPowerCompare);
            minionsAttacker.Reverse();

            // TryPlayBestMinionCard
            action = TryPlayBestMinionCard(player, enemyHero, enemyNextTurnDamage);
            if (action != null)
            {
                return action;
            }

            // TryPlayCoinCard
            action = TryPlayCoinCard(resources, cards);
            if (action != null)
            {
                return action;
            }

            // TryPlayBestMinionCard again
            action = TryPlayBestMinionCard(player, enemyHero, enemyNextTurnDamage);
            if (action != null)
            {
                return action;
            }

            if (player.Resources == 2)
            {
                action = TryPlayHeroPower(player, hero, cards, power, enemyHero, enemyNextTurnDamage);
                if (action != null)
                {
                    return action;
                }
            }

            // TryPlayBestMinionCard again
            action = TryPlayBestMinionCard(player, enemyHero, enemyNextTurnDamage);
            if (action != null)
            {
                return action;
            }

            // TryPlaySpellCard
            action = TryPlaySpellCard(hero, resources, cards, enemyHero, enemyMinionsWithTaunt, minions);
            if (action != null)
            {
                return action;
            }

            // TryPlayMinionCard
            action = TryPlayMinionCard(player);
            if (action != null)
            {
                return action;
            }


            // TryPlayKill: notaunts > taunts
            action = TryPlayKill(minionsWithoutTaunt, enemyMinionsWithTaunt);
            if (action != null)
            {
                return action;
            }

            // TryPlayKill: taunts > taunts
            action = TryPlayKill(minionsWithTaunt, enemyMinionsWithTaunt);
            if (action != null)
            {
                return action;
            }

            // TryRandomAttack: taunts > taunts
            action = TryRandomAttack(minionsWithoutTaunt, enemyMinionsWithTaunt);
            if (action != null)
            {
                return action;
            }

            // TryRandomAttack: notaunts > taunts
            action = TryRandomAttack(minionsWithTaunt, enemyMinionsWithTaunt);
            if (action != null)
            {
                return action;
            }

            // TryPlayKill: notaunts > great dangerous
            action = TryPlayKill(minionsWithoutTaunt, enemyMinionsGreatDangerous);
            if (action != null)
            {
                return action;
            }

            // TryPlayKill: notaunts > dangerous
            action = TryPlayKill(minionsWithoutTaunt, enemyMinionsDangerous);
            if (action != null)
            {
                return action;
            }

            // TryRandomAttack: notaunts > great dangerous
            action = TryRandomAttack(minionsWithoutTaunt, enemyMinionsGreatDangerous);
            if (action != null)
            {
                return action;
            }

            // TryRandomAttack: notaunts > dangerous
            action = TryRandomAttack(minionsWithoutTaunt, enemyMinionsDangerous);
            if (action != null)
            {
                return action;
            }

            // TryPlayKill: taunts > great dangerous
            action = TryPlayKill(minionsWithTaunt, enemyMinionsGreatDangerous);
            if (action != null)
            {
                return action;
            }

            // TryRandomAttack: taunts > great dangerous
            action = TryRandomAttack(minionsWithTaunt, enemyMinionsGreatDangerous);
            if (action != null)
            {
                return action;
            }

            // attack face
            foreach (var card in minionsAttacker)
            {
                return RockAction.Create(card.RockId, enemyHero.RockId);
            }

            // attack face with hero weapon
            if (enemyMinionsWithTaunt.Count == 0 && hero.HasWeapon && hero.WeaponCanAttack)
            {
                return RockAction.Create(hero.WeaponRockId, enemyHero.RockId);
            }

            // attack face with hero
            if (enemyMinionsWithTaunt.Count == 0 && !hero.IsExhausted && hero.CanAttack && hero.Damage > 0)
            {
                return RockAction.Create(hero.RockId, enemyHero.RockId);
            }

            action = TryPlayHeroPower(player, hero, cards, power, enemyHero, enemyNextTurnDamage);
            if (action != null)
            {
                return action;
            }

            return null;
        }


        private static RockAction TryPlayHeroPower(RockPlayer player, RockHero hero, List<RockCard> cards, RockCard power, RockHero enemyHero, int enemyNextTurnDamage)
        {
            if (player.Resources < 2 || !player.PowerAvailable)
            {
                return null;
            }

                switch (hero.Class)
            {
                case RockHeroClass.Warlock:
                    if (cards.Count > 8 || hero.Health < 5)
                    {
                        return null;
                    }
                    else if (hero.Health < 12)
                    {
                        if (enemyNextTurnDamage + 2 > hero.Health)
                        {
                            return null;
                        }
                        else
                        {
                            return RockAction.Create(power.RockId);
                        }
                    }
                    else
                    {
                        return RockAction.Create(power.RockId);
                    }
                case RockHeroClass.Hunter:
                case RockHeroClass.Druid:
                case RockHeroClass.Paladin:
                case RockHeroClass.Rogue:
                case RockHeroClass.Shaman:
                case RockHeroClass.Warrior:
                    return RockAction.Create(power.RockId);
                case RockHeroClass.Priest:
                    return RockAction.Create(power.RockId, hero.RockId);
                case RockHeroClass.Mage:
                    return RockAction.Create(power.RockId, enemyHero.RockId);
                default:
                    break;
            }

            return null;
        }

        private static RockAction TryPlayKill(List<RockMinion> attackers, List<RockMinion> targets)
        {
            RockMinion bestTarget = null;
            RockMinion bestAttacker = null;

            //find taunt kill attacker
            foreach (var target in targets)
            {
                foreach (var attacker in attackers)
                {
                    if (target.Health <= attacker.Damage)
                    {
                        if (bestTarget == null)
                        {
                            bestTarget = target;
                            bestAttacker = attacker;
                        }
                        else
                        {
                            if (bestAttacker.Damage > attacker.Damage)
                            {
                                bestAttacker = attacker;
                            }
                        }
                    }
                }

                if (bestTarget != null)
                {
                    return RockAction.Create(bestAttacker.RockId, bestTarget.RockId);
                }
            }

            return null;
        }


        private static RockAction TryRandomAttack(List<RockMinion> attackers, List<RockMinion> targets)
        {
            //deal damage with no taunt
            foreach (var target in targets)
            {
                foreach (var attacker in attackers)
                {
                    return RockAction.Create(attacker.RockId, target.RockId);
                }
            }

            return null;
        }

        private static RockAction TryPlaySpellCard(RockHero hero, int resources, List<RockCard> cards, RockHero enemyHero, List<RockMinion> enemyMinionsWithTaunt, List<RockMinion> minions)
        {
            foreach (var card in cards)
            {
                if (resources < card.Cost)
                {
                    continue;
                }

                // ingore coin
                if (card.CardId == "GAME_005")
                {
                    continue;
                }

                if (card.IsSpell)
                {
                    if (card.RequireTarget())
                    {
                        if (card.CanTargetEnemyHero())
                        {
                            return RockAction.Create(card.RockId, enemyHero.RockId);
                        }
                        else if (card.CanTargetHero())
                        {
                            return RockAction.Create(card.RockId, hero.RockId);
                        }
                        else if (card.CanTargetEnemyMinion() && enemyMinionsWithTaunt.Count != 0)
                        {
                            return RockAction.Create(card.RockId, enemyMinionsWithTaunt[0].RockId);
                        }
                        else if (card.CanTargetMinion() && minions.Count != 0)
                        {
                            return RockAction.Create(card.RockId, minions[0].RockId);
                        }

                        continue;
                    }
                    else
                    {
                        return RockAction.Create(card.RockId);
                    }
                }

                if (card.IsWeapon && !hero.HasWeapon)
                {
                    return RockAction.Create(card.RockId);
                }
            }

            return null;
        }

        private static RockAction TryPlayCoinCard(int resources, List<RockCard> cards)
        {
            RockCard coinCard = null;
            bool needCoinCard = false;
            foreach (var card in cards)
            {
                if (card.CardId == "GAME_005")
                {
                    coinCard = card;
                    continue;
                }

                if (resources == card.Cost - 1)
                {
                    needCoinCard = true;
                }
            }

            if (coinCard != null && needCoinCard)
            {
                return RockAction.Create(coinCard.RockId);
            }

            return null;
        }

        private static RockAction TryPlayBestMinionCard(RockPlayer player, RockHero enemyHero, int enemyNextTurnDamage)
        {
            if (player.Minions.Count >= 7)
            {
                return null;
            }

            List<RockCard> availableCards = new List<RockCard>();

            foreach (var card in player.Cards)
            {
                if (player.Resources >= card.Cost && card.IsMinion)
                {
                    availableCards.Add(card);
                }
            }

            foreach (var card in availableCards)
            {
                if (card.HasTaunt &&
                    (card.Cost == player.Resources || ((card.Cost == player.Resources - 2) && player.PowerAvailable)))
                {
                    return RockAction.Create(card.RockId);
                }
            }

            // if hero has less health, need more emergency taunt
            // can waste one cost
            if ((player.Hero.Health - enemyNextTurnDamage) < 10)
            {
                foreach (var card in availableCards)
                {
                    if (card.HasTaunt &&
                        (card.Cost == player.Resources - 1 || ((card.Cost == player.Resources - 3) && player.PowerAvailable)))
                    {
                        return RockAction.Create(card.RockId);
                    }
                }
            }

            // if hero has much more health, play as much charge as possible
            if ((player.Hero.Health - enemyHero.Health) > 10)
            {
                foreach (var card in availableCards)
                {
                    if (card.HasCharge)
                    {
                        return RockAction.Create(card.RockId);
                    }
                }
            }

            return null;
        }

        private static RockAction TryPlayMinionCard(RockPlayer player)
        {
            if (player.Minions.Count >= 7)
            {
                return null;
            }

            List<RockCard> availableCards = new List<RockCard>();

            foreach (var card in player.Cards)
            {
                if (player.Resources >= card.Cost && card.IsMinion)
                {
                    availableCards.Add(card);
                }
            }

            for (int i = player.Resources; i >= 0; i--)
            {
                foreach (var card in availableCards)
                {
                    if (card.Cost == i)
                    {
                        return RockAction.Create(card.RockId);
                    }
                }
            }

            return null;
        }
    }
}
