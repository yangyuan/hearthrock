// <copyright file="RockCard.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    public class RockCard
    {
        public int RockId { get; set; }

        public string CardId { get; set; }

        public int Cost { get; set; }

        // AI auther is responsible to know all about card from CardId
        public bool IsSpell { get; set; }

        public bool IsWeapon { get; set; }

        public bool IsMinion { get; set; }

        public bool HasTaunt { get; set; }

        public bool HasCharge { get; set; }
    }
}
