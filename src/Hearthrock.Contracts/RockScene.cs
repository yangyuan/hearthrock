// <copyright file="RockScene.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Scene contract of Hearthrock
    /// </summary>
    public class RockScene
    {
        /// <summary>
        /// Gets or sets the friendly player.
        /// </summary>
        public RockPlayer Self { get; set; }

        /// <summary>
        /// Gets or sets the enemy player.
        /// </summary>
        public RockPlayer Opponent { get; set; }

        /// <summary>
        /// Gets or sets the turn number of the game.
        /// </summary>
        public int Turn { get; set; }

        /// <summary>
        /// Gets or sets the play options.
        /// </summary>
        public List<List<int>> PlayOptions { get; set; }

        /// <summary>
        /// Gets or sets the action sequence of the game.
        /// The sequence increases during a session, and remain the same when reporting action results.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Gets or sets the unique client session GUID.
        /// SessionId changes when start a new game or re-start the bot.
        /// </summary>
        public string SessionId { get; set; }
    }
}
