using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearthrock
{
    class HearthrockUtils
    {
        public static ScenarioDbId RandomPracticeMission()
        {
            ScenarioDbId[] AI_EXPERT = {
                ScenarioDbId.PRACTICE_EXPERT_MAGE,
                ScenarioDbId.PRACTICE_EXPERT_WARLOCK,
                ScenarioDbId.PRACTICE_EXPERT_HUNTER,
                ScenarioDbId.PRACTICE_EXPERT_ROGUE,
                ScenarioDbId.PRACTICE_EXPERT_PRIEST,
                ScenarioDbId.PRACTICE_EXPERT_WARRIOR,
                ScenarioDbId.PRACTICE_EXPERT_DRUID,
                ScenarioDbId.PRACTICE_EXPERT_PALADIN,
                ScenarioDbId.PRACTICE_EXPERT_SHAMAN
            };

            Random random = new Random();
            return AI_EXPERT[random.Next(AI_EXPERT.Length)];
        }

    }


    public enum HEARTHROCK_GAMEMODE
    {
        PLAY_RANKED,
        PLAY_UNRANKED,
        PRACTICE_NORMAL,
        PRACTICE_EXPERT,
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
            power -= entity.GetRemainingHP();
            if (entity.HasDivineShield())
            {
                power -= 1;
            }
            return power;
        }

    }
    public enum HEARTHROCK_ACTIONTYPE
    {
        INVALID,
        PLAY,
        ATTACK,
    }

    public class RockAction
    {
        public HEARTHROCK_ACTIONTYPE type;
        public int step;
        public string msg;
        public Card card1;
        public Card card2;
        public RockAction()
        {
            type = HEARTHROCK_ACTIONTYPE.INVALID;
            step = 0;
            msg = "";
            card1 = null;
            card2 = null;
        }
    }
}
