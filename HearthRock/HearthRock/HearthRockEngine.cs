using System;
using System.Collections.Generic;
using System.Text;

namespace HearthRock
{
    public enum ROCK_GAME_MODE
    {
        PRACTICE_NORMAL,
        PRACTICE_EXPERT,
        PLAY_UNRANKED,
        PLAY_RANKED,
    }
    class HearthRockEngine
    {

        public static ROCK_GAME_MODE RockGameMode { get; set; }

        private static void Log(string message)
        {
            UnityEngine.MonoBehaviour.print(DateTime.Now + ": " + message);
        }

        public static MissionID RandomAIMissionID(bool expert)
        {
            MissionID[] AI_NORMAL = {
                MissionID.AI_NORMAL_MAGE,
                MissionID.AI_NORMAL_WARLOCK,
                MissionID.AI_NORMAL_HUNTER,
                MissionID.AI_NORMAL_ROGUE,
                MissionID.AI_NORMAL_PRIEST,
                MissionID.AI_NORMAL_WARRIOR,
                MissionID.AI_NORMAL_DRUID,
                MissionID.AI_NORMAL_PALADIN,
                MissionID.AI_NORMAL_SHAMAN
            };
            MissionID[] AI_EXPERT = {
                MissionID.AI_EXPERT_MAGE,
                MissionID.AI_EXPERT_WARLOCK,
                MissionID.AI_EXPERT_HUNTER,
                MissionID.AI_EXPERT_ROGUE,
                MissionID.AI_EXPERT_PRIEST,
                MissionID.AI_EXPERT_WARRIOR,
                MissionID.AI_EXPERT_DRUID,
                MissionID.AI_EXPERT_PALADIN,
                MissionID.AI_EXPERT_SHAMAN
            };
            MissionID[] AI = expert ? AI_EXPERT : AI_NORMAL;

            Random random = new Random();
            return AI[random.Next(AI.Length)];
        }

        private static bool HeroSpellReady = true;

        public static void RockEnd()
        {
            HeroSpellReady = true;
        }

        public static RockAction RockIt()
        {
            RockAction action = new RockAction();

            Player player = GameState.Get().GetLocalPlayer();
            Player player_enemy = GameState.Get().GetFirstOpponentPlayer(GameState.Get().GetLocalPlayer());
            Card hero = player.GetHeroCard();
            Card hero_enemy = player_enemy.GetHeroCard();
            int resource = player.GetNumAvailableResources();

            List<Card> crads = player.GetHandZone().GetCards();
            List<Card> minions = player.GetBattlefieldZone().GetCards();
            List<Card> minions_enemy = player_enemy.GetBattlefieldZone().GetCards();
            Card heropower = player.GetHeroPowerCard();


            // find best match taunt attacker
            List<Card> minion_taunts_enemy = new List<Card>();
            List<Card> minion_notaunts = new List<Card>();
            List<Card> minion_taunts = new List<Card>();
            List<Card> minion_attacker = new List<Card>();

            minions_enemy.Sort(new CardPowerComparer());
            minion_taunts_enemy.Sort(new CardPowerComparer());
            minion_notaunts.Sort(new CardPowerComparer());
            minion_notaunts.Reverse();
            minion_taunts.Sort(new CardPowerComparer());
            minion_taunts.Reverse();
            minion_attacker.Sort(new CardPowerComparer());
            minion_attacker.Reverse();

            int attack_count_enemy = 0;
            foreach (Card minion_enemy in minions_enemy)
            {
                attack_count_enemy += minion_enemy.GetEntity().GetATK();
                if (minion_enemy.GetEntity().HasWindfury())
                {
                    attack_count_enemy += minion_enemy.GetEntity().GetATK();
                    Log("attack_count_enemy: HasWindfury");
                }
            }
            attack_count_enemy += hero_enemy.GetEntity().GetATK();
            Log("attack_count_enemy: " + attack_count_enemy);

            foreach (Card card_oppo in minions_enemy)
            {
                if (card_oppo.GetEntity().CanBeAttacked() && card_oppo.GetEntity().HasTaunt() && !card_oppo.GetEntity().IsStealthed())
                {
                    minion_taunts_enemy.Add(card_oppo);
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

            // PlayEmergencyCard 
            RockAction action_temp = PlayEmergencyCard(resource, crads, hero, hero_enemy, attack_count_enemy);
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
                if (resource == card.GetEntity().GetCost()-1)
                {
                    need_coin_card = true;
                }
            }

            // use coin
            if (coin_card != null && need_coin_card)
            {
                action.type = ROCK_ACTION_TYPE.PLAY;
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
                    action.type = ROCK_ACTION_TYPE.PLAY;
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
                if (card.GetEntity().IsMinion() && GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards().Count < 6 && (card.GetEntity().GetCost() == resource || ((card.GetEntity().GetCost() == resource - 2) && HeroSpellReady)))
                {
                    action.type = ROCK_ACTION_TYPE.PLAY;
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

                if (card.GetEntity().IsMinion() && GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards().Count < 6)
                {
                    action.type = ROCK_ACTION_TYPE.PLAY;
                    action.card1 = card;
                    return action;
                }
                if (card.GetEntity().IsMinion() && GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards().Count == 6)
                {
                    if (card.GetEntity().HasCharge() || card.GetEntity().HasTaunt())
                    {
                        action.type = ROCK_ACTION_TYPE.PLAY;
                        action.card1 = card;
                        return action;
                    }
                }
            }


            // begin attack

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
                    action.type = ROCK_ACTION_TYPE.ATTACK;
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
                    action.type = ROCK_ACTION_TYPE.ATTACK;
                    action.card1 = card;
                    action.card2 = card_oppo;
                    return action;
                }
            }

            // no taunt, but in danger
            if (minion_taunts_enemy.Count == 0 && (hero.GetEntity().GetHealth() - attack_count_enemy) < 10 )
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
                                action.type = ROCK_ACTION_TYPE.ATTACK;
                                action.card1 = card;
                                action.card2 = card_oppo;
                                return action;
                            }
                        }
                    }
                }
            }

            // no taunt, but in great danger
            if (minion_taunts_enemy.Count == 0 && (hero.GetEntity().GetHealth() - attack_count_enemy) < 0 )
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
                                action.type = ROCK_ACTION_TYPE.ATTACK;
                                action.card1 = card;
                                action.card2 = card_oppo;
                                return action;
                            }
                        }
                    }
                }
            }


            /*
            foreach (Card card_oppo in minions_enemy)
            {
                

                foreach

                if (card_oppo.GetEntity().HasTaunt())
                {
                    Card bestchoice = null;
                    foreach (Card card in minions)
                    {
                        if (card.GetEntity().CanAttack() && !card.GetEntity().IsExhausted() && !card.GetEntity().IsFrozen() && !card.GetEntity().IsAsleep() && card.GetEntity().GetATK() > 0)
                        {
                            if (card.GetEntity().GetATK() > card_oppo.GetEntity().GetRemainingHP())
                            {
                                if (bestchoice == null) bestchoice = card;
                                else if (bestchoice.GetEntity().GetATK() > card.GetEntity().GetATK())
                                {
                                    bestchoice = card;
                                }
                            }
                        }
                    }

                    if (bestchoice != null)
                    {
                        action.type = ROCK_ACTION_TYPE.ATTACK;
                        action.card1 = bestchoice;
                        action.card2 = card_oppo;
                        return action;
                    }
                }
            }
             * 
            */

            foreach (Card card in minions)
            {
                if (card.GetEntity().CanAttack() && !card.GetEntity().IsExhausted() && !card.GetEntity().IsFrozen() && !card.GetEntity().IsAsleep() && card.GetEntity().GetATK() > 0)
                {
                    foreach (Card card_oppo in minions_enemy)
                    {
                        // for bug, should noy run
                        if (card_oppo.GetEntity().HasTaunt())
                        {
                            action.type = ROCK_ACTION_TYPE.ATTACK;
                            action.card1 = card;
                            action.card2 = card_oppo;
                            return action;
                        }
                    }
                    action.type = ROCK_ACTION_TYPE.ATTACK;
                    action.card1 = card;
                    action.card2 = player_enemy.GetHeroCard();
                    return action;
                }
            }

            if (resource >= 2 && HeroSpellReady)
            {
                HeroSpellReady = false;
                TAG_CLASS hero_class = player.GetHeroCard().GetEntity().GetClass();
                switch (hero_class)
                {
                    case TAG_CLASS.WARLOCK:
                        Log("hero health " + hero.GetEntity().GetRemainingHP());
                        if (crads.Count > 8)
                        {
                            return action;
                        }
                        if (hero.GetEntity().GetRemainingHP() < 5)
                        {
                            return action;
                        }
                        else if (hero.GetEntity().GetRemainingHP() < 12)
                        {
                            

                            if (attack_count_enemy + 2 > hero.GetEntity().GetRemainingHP())
                            {
                                return action;
                            }
                            else
                            {
                                action.type = ROCK_ACTION_TYPE.PLAY;
                                action.card1 = heropower;
                                return action;
                            }
                        }
                        else
                        {
                            action.type = ROCK_ACTION_TYPE.PLAY;
                            action.card1 = heropower;
                            return action;
                        }
                    case TAG_CLASS.HUNTER:
                    case TAG_CLASS.DRUID:
                    case TAG_CLASS.PALADIN:
                    case TAG_CLASS.ROGUE:
                    case TAG_CLASS.SHAMAN:
                    case TAG_CLASS.WARRIOR:
                        action.type = ROCK_ACTION_TYPE.PLAY;
                        action.card1 = heropower;
                        return action;
                    case TAG_CLASS.PRIEST:
                        action.type = ROCK_ACTION_TYPE.ATTACK;
                        action.card1 = heropower;
                        action.card2 = player.GetHeroCard();
                        return action;
                    case TAG_CLASS.MAGE:
                        action.type = ROCK_ACTION_TYPE.ATTACK;
                        action.card1 = heropower;
                        action.card2 = player_enemy.GetHeroCard();
                        return action;
                    default:
                        break;
                }
            }
            return action;

        }


        private static RockAction PlayKill(List<Card> minion_target, List<Card> minion_attacker)
        {
            RockAction action = new RockAction();
            Card target_best = null;
            Card attacker_best = null;

            //find taunt kill attacker
            foreach (Card card_oppo in minion_target)
            {
                foreach (Card card in minion_attacker)
                {
                    if (card_oppo.GetEntity().GetRemainingHP() <= card.GetEntity().GetATK())
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
                    action.type = ROCK_ACTION_TYPE.ATTACK;
                    action.card1 = attacker_best;
                    action.card2 = target_best;
                    return action;
                }
            }
            return null;
        }

        private static RockAction PlayEmergencyCard(int resource, List<Card> crads, Card hero, Card hero_enemy, int attack_count_enemy)
        {
            RockAction action = new RockAction();

            // best fit taunt
            foreach (Card card in crads)
            {
                if (resource < card.GetEntity().GetCost())
                {
                    continue;
                }
                if (card.GetEntity().IsMinion() && card.GetEntity().HasTaunt() && GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards().Count < 7 && (card.GetEntity().GetCost() == resource || ((card.GetEntity().GetCost() == resource - 2) && HeroSpellReady)))
                {
                    action.type = ROCK_ACTION_TYPE.PLAY;
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
                    if (card.GetEntity().IsMinion() && card.GetEntity().HasTaunt() && GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards().Count < 7 && (card.GetEntity().GetCost() == resource - 1 || ((card.GetEntity().GetCost() == resource - 3) && HeroSpellReady)))
                    {
                        action.type = ROCK_ACTION_TYPE.PLAY;
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
                    if (card.GetEntity().IsMinion() && GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards().Count < 7)
                    {
                        if (card.GetEntity().HasCharge())
                        {
                            action.type = ROCK_ACTION_TYPE.PLAY;
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
            int power_x = x.GetEntity().GetATK() - x.GetEntity().GetRemainingHP();
            int power_y = y.GetEntity().GetATK() - y.GetEntity().GetRemainingHP();
            return power_x - power_y;
        }
    
    }
    public enum ROCK_ACTION_TYPE
    {
        INVALID,
        PLAY,
        ATTACK,
    }

    public class RockAction
    {
        public ROCK_ACTION_TYPE type;
        public int step;
        public string msg;
        public Card card1;
        public Card card2;
        public RockAction()
        {
            type = ROCK_ACTION_TYPE.INVALID;
            step = 0;
            msg = "";
            card1 = null;
            card2 = null;
        }
    }
}
