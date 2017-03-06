using System;
using System.Collections.Generic;
using System.Text;

namespace Hearthrock
{

    /// <summary>
    /// this class manage the main logic of Hearthrock.
    /// including but not limited to: interact with Hearthstone, keep program active
    /// </summary>
    class HearthrockRobot
    {
       

        private static bool HeroSpellReady = true;

        public static void RockEnd()
        {
            HeroSpellReady = true;
        }

        public static bool IsEnimyDangerous(Entity enimy) {
            if (enimy.GetHealth() > 4)
            {
                if (enimy.GetATK() - enimy.GetHealth() > 4)
                {
                    return true;
                }
            }
            else if (enimy.GetHealth() > 2) // 3,4
            {
                if (enimy.GetATK() - enimy.GetHealth() > 3)
                {
                    return true;
                }
            }
            else // 1,2
            {
                if (enimy.GetATK() - enimy.GetHealth() > 2)
                {
                    return true;
                }
            }

            return false;
        }

        public static RockActionInternal RockIt()
        {
            RockActionInternal action = new RockActionInternal();

            Player player = GameState.Get().GetFriendlySidePlayer();
            Player player_enemy = GameState.Get().GetFirstOpponentPlayer(GameState.Get().GetFriendlySidePlayer());
            Card hero = player.GetHeroCard();
            Card hero_enemy = player_enemy.GetHeroCard();
            int resource = player.GetNumAvailableResources();

            List<Card> crads = player.GetHandZone().GetCards();
            List<Card> minions = player.GetBattlefieldZone().GetCards();
            List<Card> minions_enemy = player_enemy.GetBattlefieldZone().GetCards();
            Card heropower = player.GetHeroPowerCard();

            // find best match taunt attacker
            List<Card> minion_taunts_enemy = new List<Card>();
            List<Card> minion_dangerous_enemy = new List<Card>();
            List<Card> minion_notaunts = new List<Card>();
            List<Card> minion_taunts = new List<Card>();
            List<Card> minion_attacker = new List<Card>();


            int attack_count_enemy = 0;
            foreach (Card minion_enemy in minions_enemy)
            {
                attack_count_enemy += minion_enemy.GetEntity().GetATK();
                if (minion_enemy.GetEntity().HasWindfury())
                {
                    attack_count_enemy += minion_enemy.GetEntity().GetATK();
                }
            }
            attack_count_enemy += hero_enemy.GetEntity().GetATK();

            foreach (Card card_oppo in minions_enemy)
            {
                if (card_oppo.GetEntity().CanBeAttacked() && !card_oppo.GetEntity().IsStealthed())
                {
                    if (card_oppo.GetEntity().HasTaunt())
                    {
                        minion_taunts_enemy.Add(card_oppo);
                    }
                    else if (IsEnimyDangerous(card_oppo.GetEntity()))
                    {
                        minion_dangerous_enemy.Add(card_oppo);
                    }
                }
            }

            foreach (Card card in minions)
            {
                if (card.GetEntity().CanAttack() && !card.GetEntity().IsExhausted() && !card.GetEntity().IsFrozen() && !card.GetEntity().IsAsleep() && card.GetEntity().GetATK() > 0)
                {
                    if (card.GetEntity().HasTaunt())
                    {
                        minion_taunts.Add(card);
                    }
                    else
                    {
                        minion_notaunts.Add(card);
                    }
                    minion_attacker.Add(card);
                }
            }


            minions_enemy.Sort(new CardPowerComparer());
            minion_taunts_enemy.Sort(new CardPowerComparer());
            minion_notaunts.Sort(new CardPowerComparer());
            minion_notaunts.Reverse();
            minion_taunts.Sort(new CardPowerComparer());
            minion_taunts.Reverse();
            minion_attacker.Sort(new CardPowerComparer());
            minion_attacker.Reverse();

            // PlayEmergencyCard 
            RockActionInternal action_temp = PlayEmergencyCard(resource, crads, hero, hero_enemy, attack_count_enemy);
            if (action_temp != null)
            {
                action = action_temp;
                return action;
            }

            // if coin necessory
            Card coin_card = null;
            bool need_coin_card = false;
            foreach (Card card in crads)
            {
                if (card.GetEntity().GetCardId() == "GAME_005")
                {
                    coin_card = card;
                    continue;
                }
                if (resource == card.GetEntity().GetCost() - 1)
                {
                    need_coin_card = true;
                }
            }

            // use coin
            if (coin_card != null && need_coin_card)
            {
                action.type = RockActionTypeInternal.Play;
                action.card1 = coin_card;
                return action;
            }

            // PlayEmergencyCard  again
            action_temp = PlayEmergencyCard(resource, crads, hero, hero_enemy, attack_count_enemy);
            if (action_temp != null)
            {
                action = action_temp;
                return action;
            }


            // use as much spell as possible
            foreach (Card card in crads)
            {
                // but not the coin
                if (card.GetEntity().GetCardId() == "GAME_005")
                {
                    continue;
                }
                if (resource < card.GetEntity().GetCost())
                {
                    continue;
                }
                if (card.GetEntity().IsSpell() || card.GetEntity().IsWeapon())
                {
                    action.type = RockActionTypeInternal.Play;
                    action.card1 = card;
                    return action;
                }
            }

            // find a minion which can use all resource
            foreach (Card card in crads)
            {
                if (resource < card.GetEntity().GetCost())
                {
                    continue;
                }
                if (card.GetEntity().IsMinion() && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count < 6 && (card.GetEntity().GetCost() == resource || ((card.GetEntity().GetCost() == resource - 2) && HeroSpellReady)))
                {
                    action.type = RockActionTypeInternal.Play;
                    action.card1 = card;
                    return action;
                }
            }

            // find a minion which can use all resource
            foreach (Card card in crads)
            {
                if (resource < card.GetEntity().GetCost())
                {
                    continue;
                }

                if (card.GetEntity().IsMinion() && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count < 6)
                {
                    action.type = RockActionTypeInternal.Play;
                    action.card1 = card;
                    return action;
                }
                if (card.GetEntity().IsMinion() && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count == 6)
                {
                    if (card.GetEntity().HasCharge() || card.GetEntity().HasTaunt())
                    {
                        action.type = RockActionTypeInternal.Play;
                        action.card1 = card;
                        return action;
                    }
                }
            }


            // begin attack
            { // deal with his taunts
                // PlayKill  notaunts > taunts
                action_temp = PlayKill(minion_taunts_enemy, minion_notaunts);
                if (action_temp != null)
                {
                    action = action_temp;
                    return action;
                }

                // PlayKill  taunts > taunts
                action_temp = PlayKill(minion_taunts_enemy, minion_taunts);
                if (action_temp != null)
                {
                    action = action_temp;
                    return action;
                }

                //deal damage with no taunt
                foreach (Card card_oppo in minion_taunts_enemy)
                {
                    foreach (Card card in minion_notaunts)
                    {
                        action.type = RockActionTypeInternal.Attack;
                        action.card1 = card;
                        action.card2 = card_oppo;
                        return action;
                    }
                }

                //deal damage with taunt
                foreach (Card card_oppo in minion_taunts_enemy)
                {
                    foreach (Card card in minion_taunts)
                    {
                        action.type = RockActionTypeInternal.Attack;
                        action.card1 = card;
                        action.card2 = card_oppo;
                        return action;
                    }
                }

            }

            { // deal with his dangerous
                // TODO: should check maybe i can kill him  
                // PlayKill  notaunts > dangerous
                action_temp = PlayKill(minion_dangerous_enemy, minion_notaunts);
                if (action_temp != null)
                {
                    action = action_temp;
                    return action;
                }


                //deal damage with no taunt
                foreach (Card card_oppo in minion_dangerous_enemy)
                {
                    foreach (Card card in minion_notaunts)
                    {
                        action.type = RockActionTypeInternal.Attack;
                        action.card1 = card;
                        action.card2 = card_oppo;
                        return action;
                    }
                }
            }

            { // deal with enemy depends on my health 
                // TODO: should check maybe i can kill him  
                // no taunt, but in danger
                if (minion_taunts_enemy.Count == 0 && (hero.GetEntity().GetHealth() - attack_count_enemy) < 10)
                {
                    foreach (Card card_oppo in minions_enemy)
                    {
                        // find dangerous card
                        if (card_oppo.GetEntity().GetATK() - card_oppo.GetEntity().GetHealth() > 3)
                        {
                            foreach (Card card in minion_attacker)
                            {
                                // if can kill, kill
                                if (card.GetEntity().GetATK() >= card_oppo.GetEntity().GetHealth())
                                {
                                    action.type = RockActionTypeInternal.Attack;
                                    action.card1 = card;
                                    action.card2 = card_oppo;
                                    return action;
                                }
                            }
                        }
                    }
                }

                // no taunt, but in great danger
                if (minion_taunts_enemy.Count == 0 && (hero.GetEntity().GetHealth() - attack_count_enemy) < 0)
                {
                    foreach (Card card_oppo in minions_enemy)
                    {
                        // find dangerous card
                        if (card_oppo.GetEntity().GetATK() - card_oppo.GetEntity().GetHealth() > 1)
                        {
                            foreach (Card card in minion_attacker)
                            {
                                if (card.GetEntity().GetATK() > card_oppo.GetEntity().GetATK())
                                {
                                    continue;
                                }
                                // if can kill, kill
                                if (card.GetEntity().GetATK() >= card_oppo.GetEntity().GetHealth())
                                {
                                    action.type = RockActionTypeInternal.Attack;
                                    action.card1 = card;
                                    action.card2 = card_oppo;
                                    return action;
                                }
                            }
                        }
                    }
                }
            } // END deal with enemy depends on my health 


            // attack his face!
            foreach (Card card in minions)
            {
                if (card.GetEntity().CanAttack() && !card.GetEntity().IsExhausted() && !card.GetEntity().IsFrozen() && !card.GetEntity().IsAsleep() && card.GetEntity().GetATK() > 0)
                {
                    foreach (Card card_oppo in minions_enemy)
                    {
                        // for bug, should noy run
                        if (card_oppo.GetEntity().HasTaunt())
                        {
                            action.type = RockActionTypeInternal.Attack;
                            action.card1 = card;
                            action.card2 = card_oppo;
                            return action;
                        }
                    }
                    action.type = RockActionTypeInternal.Attack;
                    action.card1 = card;
                    action.card2 = player_enemy.GetHeroCard();
                    return action;
                }


            }


            if (minion_taunts_enemy.Count == 0 && player.HasWeapon() && player.GetWeaponCard().GetEntity().CanAttack())
            {
                action.type = RockActionTypeInternal.Attack;
                action.card1 = player.GetWeaponCard();
                action.card2 = player_enemy.GetHeroCard();
                return action;
            }


            Entity me = player.GetHeroCard().GetEntity();
            if (minion_taunts_enemy.Count == 0 && me.CanAttack() && me.GetATK() > 0)
            {
                action.type = RockActionTypeInternal.Attack;
                action.card1 = player.GetHeroCard();
                action.card2 = player_enemy.GetHeroCard();
                return action;
            }


            if (resource >= 2 && HeroSpellReady)
            {
                HeroSpellReady = false;
                TAG_CLASS hero_class = player.GetHeroCard().GetEntity().GetClass();
                switch (hero_class)
                {
                    case TAG_CLASS.WARLOCK:
                        if (crads.Count > 8)
                        {
                            return action;
                        }
                        if (hero.GetEntity().GetRealTimeRemainingHP() < 5)
                        {
                            return action;
                        }
                        else if (hero.GetEntity().GetRealTimeRemainingHP() < 12)
                        {


                            if (attack_count_enemy + 2 > hero.GetEntity().GetRealTimeRemainingHP())
                            {
                                return action;
                            }
                            else
                            {
                                action.type = RockActionTypeInternal.Play;
                                action.card1 = heropower;
                                return action;
                            }
                        }
                        else
                        {
                            action.type = RockActionTypeInternal.Play;
                            action.card1 = heropower;
                            return action;
                        }
                    case TAG_CLASS.HUNTER:
                    case TAG_CLASS.DRUID:
                    case TAG_CLASS.PALADIN:
                    case TAG_CLASS.ROGUE:
                    case TAG_CLASS.SHAMAN:
                    case TAG_CLASS.WARRIOR:
                        action.type = RockActionTypeInternal.Play;
                        action.card1 = heropower;
                        return action;
                    case TAG_CLASS.PRIEST:
                        action.type = RockActionTypeInternal.Attack;
                        action.card1 = heropower;
                        action.card2 = player.GetHeroCard();
                        return action;
                    case TAG_CLASS.MAGE:
                        action.type = RockActionTypeInternal.Attack;
                        action.card1 = heropower;
                        action.card2 = player_enemy.GetHeroCard();
                        return action;
                    default:
                        break;
                }
            }
            return action;

        }


        private static RockActionInternal PlayKill(List<Card> minion_target, List<Card> minion_attacker)
        {
            RockActionInternal action = new RockActionInternal();
            Card target_best = null;
            Card attacker_best = null;

            //find taunt kill attacker
            foreach (Card card_oppo in minion_target)
            {
                foreach (Card card in minion_attacker)
                {
                    if (card_oppo.GetEntity().GetRealTimeRemainingHP() <= card.GetEntity().GetATK())
                    {
                        if (target_best == null)
                        {
                            target_best = card_oppo;
                            attacker_best = card;
                        }
                        else
                        {
                            if (attacker_best.GetEntity().GetATK() > card.GetEntity().GetATK())
                            {
                                attacker_best = card;
                            }
                        }
                    }
                }

                if (target_best != null)
                {
                    action.type = RockActionTypeInternal.Attack;
                    action.card1 = attacker_best;
                    action.card2 = target_best;
                    return action;
                }
            }
            return null;
        }

        private static RockActionInternal PlayEmergencyCard(int resource, List<Card> crads, Card hero, Card hero_enemy, int attack_count_enemy)
        {
            RockActionInternal action = new RockActionInternal();

            // best fit taunt
            foreach (Card card in crads)
            {
                if (resource < card.GetEntity().GetCost())
                {
                    continue;
                }
                if (card.GetEntity().IsMinion() && card.GetEntity().HasTaunt() && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count < 7 && (card.GetEntity().GetCost() == resource || ((card.GetEntity().GetCost() == resource - 2) && HeroSpellReady)))
                {
                    action.type = RockActionTypeInternal.Play;
                    action.card1 = card;
                    return action;
                }
            }

            // if hero has less health, need more emergency taunt
            if ((hero.GetEntity().GetHealth() - attack_count_enemy) < 10)
            {
                // not perfect fit taunt
                foreach (Card card in crads)
                {

                    if (resource < card.GetEntity().GetCost())
                    {
                        continue;
                    }
                    // waste a cost
                    if (card.GetEntity().IsMinion() && card.GetEntity().HasTaunt() && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count < 7 && (card.GetEntity().GetCost() == resource - 1 || ((card.GetEntity().GetCost() == resource - 3) && HeroSpellReady)))
                    {
                        action.type = RockActionTypeInternal.Play;
                        action.card1 = card;
                        return action;
                    }
                }
            }

            // if hero has more health, play as much charge as possible
            if ((hero.GetEntity().GetHealth() - hero_enemy.GetEntity().GetHealth()) > 10)
            {
                foreach (Card card in crads)
                {
                    if (resource < card.GetEntity().GetCost())
                    {
                        continue;
                    }

                    // waste a cost
                    if (card.GetEntity().IsMinion() && GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards().Count < 7)
                    {
                        if (card.GetEntity().HasCharge())
                        {
                            action.type = RockActionTypeInternal.Play;
                            action.card1 = card;
                            return action;
                        }
                    }
                }
            }
            return null;
        }

    }
    public class CardPowerComparer : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            int power_x = FetchPower(x.GetEntity());
            int power_y = FetchPower(y.GetEntity());
            return power_x - power_y;
        }

        private int FetchPower(Entity entity)
        {
            int power = entity.GetATK();
            if (entity.HasWindfury())
            {
                power += entity.GetATK();
            }
            power -= entity.GetRealTimeRemainingHP();
            if (entity.HasDivineShield())
            {
                power -= 1;
            }
            return power;
        }
    }

}
