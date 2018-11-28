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
        /// Gets or sets the available resources of player.
        /// </summary>
        public int Resources { get; set; }

        /// <summary>
        /// Gets or sets the permanent resources of player.
        /// </summary>
        public int PermanentResources { get; set; }

        /// <summary>
        /// Gets or sets the temporary resources of player.
        /// </summary>
        public int TemporaryResources { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the power of player is available.
        /// </summary>
        public bool PowerAvailable { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the hero has weapon.
        /// </summary>
        public bool HasWeapon { get; set; }

        /// <summary>
        /// Gets or sets the hero of player.
        /// </summary>
        public RockHero Hero { get; set; }

        /// <summary>
        /// Gets or sets the power of player.
        /// </summary>
        public RockCard Power { get; set; }

        /// <summary>
        /// Gets or sets the power of player.
        /// </summary>
        public RockCard Weapon { get; set; }

        /// <summary>
        /// Gets or sets the minions of player.
        /// </summary>
        public List<RockMinion> Minions { get; set; }

        /// <summary>
        /// Gets or sets the cards of player.
        /// </summary>
        public List<RockCard> Cards { get; set; }

        /// <summary>
        /// Gets or sets the (card) choices of player.
        /// </summary>
        public List<RockCard> Choices { get; set; }

        /// <summary>
        /// Gets or sets the Graveyard of player.
        /// </summary>
        public List<RockCard> Graveyard { get; set; }

        /// <summary>
        /// Gets or sets the Deck of player.
        /// </summary>
        public List<RockCard> Deck { get; set; }
    }
}