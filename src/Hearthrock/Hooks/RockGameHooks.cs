// <copyright file="RockGameHooks.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Hooks
{
    using System;

    /// <summary>
    /// Hooks for Hearthrock
    /// </summary>
    public static class RockGameHooks
    {
        /// <summary>
        ///  Gets or sets a value indicating whether PlayZoneSlotMousedOver is enabled.
        /// </summary>
        public static bool EnablePlayZoneSlotMousedOver { get; set; }

        /// <summary>
        /// Gets or sets the value of PlayZoneSlotMousedOver.
        /// </summary>
        public static int PlayZoneSlotMousedOverValue { get; set; }

        /// <summary>
        /// The method to filter the return value of PlayZoneSlotMousedOver.
        /// </summary>
        /// <param name="position">The original position</param>
        /// <returns>The updated position</returns>
        public static int PlayZoneSlotMousedOver(int position)
        {
            if (EnablePlayZoneSlotMousedOver)
            {
                int count = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCardCount();
                int result = PlayZoneSlotMousedOverValue;

                if (result < 0 || result > count)
                {
                    // By default, the position will be the middle slot
                    result = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCardCount() / 2;
                }

                Console.WriteLine("SlotPosition " + position + " >>>>>>>>> " + result);
                return result;
            }
            else
            {
                return position;
            }
        }
    }
}
