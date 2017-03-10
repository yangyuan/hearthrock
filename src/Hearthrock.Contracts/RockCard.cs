// <copyright file="RockCard.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Card contract of Hearthrock
    /// Bot author is responsible to know all about a card from CardId.
    /// </summary>
    public class RockCard : IRockObject
    {
        /// <summary>
        /// Gets or sets the Id of the card.
        /// </summary>
        public int RockId { get; set; }

        /// <summary>
        /// Gets or sets the Name of the card.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CardId of the card.
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// Gets or sets the CardType of the card.
        /// </summary>
        public RockCardType CardType { get; set; }

        /// <summary>
        /// Gets or sets the cost of the card.
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the card has taunt.
        /// </summary>
        public bool HasTaunt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the card has charge.
        /// </summary>
        public bool HasCharge { get; set; }

        /// <summary>
        /// Gets or sets the requirements to play this card.
        /// Should only be used as non-official reference.
        /// Bot author is responsible to know all the requirements from CardId.
        /// </summary>
        public List<int> PlayRequirements { get; set; }
    }
}
