// <copyright file="IRockBot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface of Hearthrock Bot.
    /// </summary>
    public interface IRockBot
    {
        /// <summary>
        /// Generate a mulligan action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be mulligan-ed.</returns>
        RockAction GetMulliganAction(RockScene scene);

        /// <summary>
        /// Generate a play action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be played.</returns>
        RockAction GetPlayAction(RockScene scene);

        /// <summary>
        /// Report the result of an action by providing the current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        void ReportActionResult(RockScene scene);
    }
}
