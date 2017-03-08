using System;
using System.Collections.Generic;

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
}
