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
        List<int> GetMulliganAction(RockScene scene);

        /// <summary>
        /// Generate a play action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be played.</returns>
        List<int> GetPlayAction(RockScene scene);
    }
}
