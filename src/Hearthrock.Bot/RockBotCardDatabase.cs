// <copyright file="RockBotCardDatabase.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot
{
    using System.Collections.Generic;

    /// <summary>
    /// Sample Card Database.
    /// Different bot may need different information, so bot auther is responsible to design and maintain their own card database.
    /// </summary>
    public class RockBotCardDatabase
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

            return new CardInfo { };
        }

        private static Dictionary<string, CardInfo> GetCardInfos()
        {
            var dictionary = new Dictionary<string, CardInfo>();

            dictionary.Add("CS2_022", new CardInfo { RequireTarget = true, CanTargetEnemyMinion = true });
            dictionary.Add("CS2_029", new CardInfo { RequireTarget = true, CanTargetEnemyHero = true });
            dictionary.Add("CS2_009", new CardInfo { RequireTarget = true, CanTargetMinion = true });
            dictionary.Add("CS2_007", new CardInfo { RequireTarget = true, CanTargetHero = true, CanTargetMinion = true }); // Healing Touch

            return dictionary;
        }

    }
}
