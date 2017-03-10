// <copyright file="RockHero.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    public class RockHero : RockObject
    {
        public RockHeroClass Class { get; set; }

        public int Damage { get; set; }

        public int Health { get; set; }

        public bool CanAttack { get; set; }

        public bool IsExhausted { get; set; }
        
        public bool HasWeapon { get; set; }

        public int WeaponRockId { get; set; }

        public bool WeaponCanAttack { get; set; }
    }
}
