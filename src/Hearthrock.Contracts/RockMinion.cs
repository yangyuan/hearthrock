// <copyright file="RockMinion.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    /// <summary>
    /// Minion contract of Hearthrock
    /// </summary>
    public class RockMinion : IRockObject
    {
        /// <summary>
        /// Gets or sets the Id of the minion.
        /// </summary>
        public int RockId { get; set; }

        /// <summary>
        /// Gets or sets the Name of the minion.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CardId of the minion.
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// Gets or sets the Damage of the minion.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Gets or sets the Health of the minion.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets the BaseHealth of the minion.
        /// </summary>
        public int BaseHealth { get; set; }

        /// <summary>
        ///  Gets or sets a minion race.
        /// </summary>
        public RockRace Race { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion is frozen.
        /// </summary>
        public bool IsFrozen { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion is exhausted.
        /// </summary>
        public bool IsExhausted { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion is asleep.
        /// </summary>
        public bool IsAsleep { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion is stealth-ed.
        /// </summary>
        public bool IsStealthed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion can attack.
        /// </summary>
        public bool CanAttack { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion can be attacked.
        /// </summary>
        public bool CanBeAttacked { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has taunt.
        /// </summary>
        public bool HasTaunt { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has wind-fury.
        /// </summary>
        public bool HasWindfury { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has divine shield.
        /// </summary>
        public bool HasDivineShield { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has aura.
        /// </summary>
        public bool HasAura { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has enraged.
        /// </summary>
        public bool IsEnraged { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has trigger visual.
        /// </summary>
        public bool HasTriggerVisual { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has inspire.
        /// </summary>
        public bool HasInspire { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has deathrattle.
        /// </summary>
        public bool HasDeathrattle { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has battlecry.
        /// </summary>
        public bool HasBattlecry { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion has lifesteal.
        /// </summary>
        public bool HasLifesteal { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the minion is poisonous.
        /// </summary>
        public bool IsPoisonous { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the minion is silenced.
        /// </summary>
        public bool IsSilenced { get; set; }

    }
}
