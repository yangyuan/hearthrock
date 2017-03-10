// <copyright file="IRockObject.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    /// <summary>
    /// Interface of Hearthrock Object.
    /// </summary>
    public interface IRockObject
    {
        /// <summary>
        /// Gets the Id of the object.
        /// </summary>
        int RockId { get; }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the CardId of the object.
        /// All Card, Hero, Weapon, Minion have an CardId.
        /// Bot authors can use CardId to get some information of a card, hero, weapon or minion.
        /// </summary>
        string CardId { get; }
    }
}
