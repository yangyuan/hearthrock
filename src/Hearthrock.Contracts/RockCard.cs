// <copyright file="RockCard.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Card contract of Hearthrock
    /// </summary>
    public class RockCard
    {
        public int RockId { get; set; }

        public string CardId { get; set; }

        public int Cost { get; set; }

        // Bot author is responsible to know all about card from CardId
        public bool IsSpell { get; set; }

        public bool IsWeapon { get; set; }

        public bool IsMinion { get; set; }

        public bool HasTaunt { get; set; }

        public bool HasCharge { get; set; }

        /// <summary>
        /// Requirements to play this card.
        /// Should only be used as non-official reference.
        /// Bot author is responsible to know all the requirements from CardId.
        /// </summary>
        public List<RockActionRequirement> ActionRequirements { get; set; }
    }
}
