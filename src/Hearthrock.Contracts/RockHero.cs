// <copyright file="RockHero.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    /// <summary>
    /// Hero contract of Hearthrock
    /// </summary>
    public class RockHero : IRockObject
    {
        /// <summary>
        /// Gets or sets the Id of the hero.
        /// </summary>
        public int RockId { get; set; }

        /// <summary>
        /// Gets or sets the Name of the hero.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CardId of the hero.
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// Gets or sets the HeroClass of the hero.
        /// </summary>
        public RockHeroClass Class { get; set; }

        /// <summary>
        /// Gets or sets the Damage of the hero.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Gets or sets the Health of the hero.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the hero can attack.
        /// </summary>
        public bool CanAttack { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the hero is exhausted.
        /// </summary>
        public bool IsExhausted { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the hero have a quest.
        /// </summary>
        public bool IsQuest { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the hero have a secret.
        /// </summary>
        public bool IsSecret { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the hero have is frozen.
        /// </summary>
        public bool IsFrozen { get; set; }
    }
}
