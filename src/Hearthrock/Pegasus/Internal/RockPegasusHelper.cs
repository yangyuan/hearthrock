// <copyright file="RockPegasusHelper.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus.Internal
{
    using System;

    using Hearthrock.Contracts;
    using PegasusShared;

    /// <summary>
    /// Helper class for RockPegasus.
    /// </summary>
    internal static class RockPegasusHelper
    {
        /// <summary>
        /// Get Pegasus Game Type from RockGameMode
        /// </summary>
        /// <param name="gameMode">The RockGameMode</param>
        /// <returns>The GameType</returns>
        public static GameType GetGameType(RockGameMode gameMode)
        {
            switch (gameMode)
            {
                case RockGameMode.NormalPractice:
                case RockGameMode.ExpertPractice:
                    return GameType.GT_VS_AI;
                case RockGameMode.Casual:
                case RockGameMode.WildCasual:
                    return GameType.GT_CASUAL;
                case RockGameMode.Ranked:
                case RockGameMode.WildRanked:
                    return GameType.GT_RANKED;
                default:
                    return GameType.GT_UNKNOWN;
            }
        }

        /// <summary>
        /// Get Pegasus Format Type from RockGameMode
        /// </summary>
        /// <param name="gameMode">The RockGameMode</param>
        /// <returns>The FormatType</returns>
        public static FormatType GetFormatType(RockGameMode gameMode)
        {
            switch (gameMode)
            {
                case RockGameMode.Casual:
                case RockGameMode.Ranked:
                    return FormatType.FT_STANDARD;
                case RockGameMode.NormalPractice:
                case RockGameMode.ExpertPractice:
                case RockGameMode.WildCasual:
                case RockGameMode.WildRanked:
                    return FormatType.FT_WILD;
                default:
                    return FormatType.FT_UNKNOWN;
            }
        }

        /// <summary>
        /// Get RockPegasusSceneState from scene mode.
        /// </summary>
        /// <param name="sceneMode">The Pegasus Scene Mode.</param>
        /// <returns>The RockPegasusSceneState.</returns>
        public static RockPegasusSceneState GetPegasusSceneState(SceneMgr.Mode sceneMode)
        {
            switch (sceneMode)
            {
                case SceneMgr.Mode.STARTUP:
                case SceneMgr.Mode.LOGIN:
                case SceneMgr.Mode.RESET:
                    return RockPegasusSceneState.BlockingScene;
                case SceneMgr.Mode.COLLECTIONMANAGER:
                case SceneMgr.Mode.PACKOPENING:
                case SceneMgr.Mode.FRIENDLY:
                case SceneMgr.Mode.CREDITS:
                case SceneMgr.Mode.DRAFT:
                case SceneMgr.Mode.TAVERN_BRAWL:
                    return RockPegasusSceneState.CancelableScene;
                case SceneMgr.Mode.TOURNAMENT:
                    return RockPegasusSceneState.TournamentScene;
                case SceneMgr.Mode.HUB:
                    return RockPegasusSceneState.HubScene;
                case SceneMgr.Mode.GAMEPLAY:
                    return RockPegasusSceneState.GamePlay;
                case SceneMgr.Mode.ADVENTURE:
                    return RockPegasusSceneState.AdventureScene;
                case SceneMgr.Mode.INVALID:
                case SceneMgr.Mode.FATAL_ERROR:
                default:
                    return RockPegasusSceneState.InvalidScene;
            }
        }
    }
}
