using System;
using System.Collections.Generic;
using System.Text;

namespace Hearthrock.Robot.Sample
{
    /// <summary>
    /// Sample Card Database.
    /// Different robot may need different information, so robot auther is responsible to design and maintain their own database.
    /// </summary>
    public class SampleCardDatabase
    {
        public struct CardInfo
        {
            public bool RequireChoice;
            public bool RequireTarget;
            public bool CanTargetEnemyHero;
            public bool CanTargetEnemyMinion;
            public bool CanTargetHero;
            public bool CanTargetMinion;
        }

        private static Dictionary<string, CardInfo> CardInfos = GetCardInfos();

        public static CardInfo GetCardInfo(string cardId)
        {
            if (CardInfos.ContainsKey(cardId))
            {
                return CardInfos[cardId];
            }

            return new CardInfo {  };
        }

        private static Dictionary<string, CardInfo> GetCardInfos()
        {
            var dictionary = new Dictionary<string, CardInfo>();

            dictionary.Add("CS2_022", new CardInfo { RequireTarget = true, CanTargetEnemyMinion = true });
            dictionary.Add("CS2_029", new CardInfo { RequireTarget = true, CanTargetEnemyHero = true });
            dictionary.Add("CS2_009", new CardInfo { RequireTarget = true, CanTargetMinion = true });
            dictionary.Add("CS2_007", new CardInfo { RequireTarget = true, CanTargetHero =  true, CanTargetMinion = true }); // Healing Touch

            return dictionary;
        }

    }
}
