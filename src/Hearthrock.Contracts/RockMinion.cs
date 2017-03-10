// <copyright file="RockMinion.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    public class RockMinion : RockObject
    {
        public int Damage { get; set; }

        public int Health { get; set; }

        public int BaseHealth { get; set; }

        public bool IsFrozen { get; set; }

        public bool IsExhausted { get; set; }

        public bool IsAsleep { get; set; }

        public bool IsStealthed { get; set; }

        public bool CanAttack { get; set; }

        public bool CanBeAttacked { get; set; }

        public bool HasTaunt { get; set; }

        public bool HasWindfury { get; set; }

        public bool HasDivineShield { get; set; }
    }
}
