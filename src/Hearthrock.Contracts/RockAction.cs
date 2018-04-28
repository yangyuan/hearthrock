// <copyright file="RockAction.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// The action of the bot.
    /// </summary>
    public class RockAction
    {
        /// <summary>
        /// Gets or sets the version of the action
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the sequence of the objects.
        /// </summary>
        public List<int> Objects { get; set; }

        /// <summary>
        /// Gets or sets the slot, slot is used when to place a minion.
        /// Index starts from zero.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// The factory method of RockAction
        /// </summary>
        /// <returns>The RockAction.</returns>
        public static RockAction Create()
        {
            return Create(new List<int>(), -1);
        }

        /// <summary>
        /// The factory method of RockAction
        /// </summary>
        /// <param name="objects">The RockObject IDs of the action.</param>
        /// <returns>The RockAction.</returns>
        public static RockAction Create(List<int> objects)
        {
            return Create(objects, -1);
        }

        /// <summary>
        /// The factory method of RockAction
        /// </summary>
        /// <param name="objects">The RockObject IDs of the action.</param>
        /// <param name="slot">The slot when apply the action.</param>
        /// <returns>The RockAction.</returns>
        public static RockAction Create(List<int> objects, int slot)
        {
            var result = new RockAction();
            result.Version = 1;
            result.Objects = objects;
            result.Slot = slot;

            return result;
        }
    }
}
