// <copyright file="RockPlayer.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Player contract of Hearthrock
    /// </summary>
    public class RockPlayer
    {
        /// <summary>
        /// Gets or sets the resources of player.
        /// </summary>
        public int Resources { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the power of player is available.
        /// </summary>
        public bool PowerAvailable { get; set; }

        /// <summary>
        /// Gets or sets the hero of player.
        /// </summary>
        public RockHero Hero { get; set; }

        /// <summary>
        /// Gets or sets the power of player.
        /// </summary>
        public RockCard Power { get; set; }

        /// <summary>
        /// Gets or sets the minions of player.
        /// </summary>
        public List<RockMinion> Minions { get; set; }

        /// <summary>
        /// Gets or sets the cards of player.
        /// </summary>
        public List<RockCard> Cards { get; set; }
    }
}
