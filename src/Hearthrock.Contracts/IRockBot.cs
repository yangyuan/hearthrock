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
        /// Pick cards for starting hand.
        /// </summary>
        /// <param name="cards">The starting hand.</param>
        /// <returns>The cards picked for starting hand.</returns>
        List<RockCard> PickCards(List<RockCard> cards);

        /// <summary>
        /// Generate an action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The action.</returns>
        RockAction GetAction(RockScene scene);
    }
}
