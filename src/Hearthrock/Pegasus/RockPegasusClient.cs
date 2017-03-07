// <copyright file="RockPegasusClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>


namespace Hearthrock.Pegasus
{
    using PegasusShared;
    using Hearthrock.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class RockPegasusClient
    {
        /// <summary>
        /// The method to FindGame.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <param name="deckId"></param>
        /// <param name="missionId"></param>
        public static void FindGame(RockGameMode gameMode, long deckId, int missionId)
        {
            Options.Get().SetBool(Option.HAS_PLAYED_EXPERT_AI, true);
            GameType gameType = RockPegasusHelper.GetGameType(gameMode);
            FormatType formatType = RockPegasusHelper.GetFormatType(gameMode);

            GameMgr.Get().FindGame(gameType, formatType, missionId, deckId, 0L);
        }
    }
}
