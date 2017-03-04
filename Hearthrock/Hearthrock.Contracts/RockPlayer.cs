// <copyright file="DirectoryAsync.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    class RockPlayer
    {
        public int Resources { get; set; }

        public RockHero Hero { get; set; }

        public RockCard Power { get; set; }

        public List<RockMinion> Minions { get; set; }

        public List<RockCard> Cards { get; set; }
    }
}
