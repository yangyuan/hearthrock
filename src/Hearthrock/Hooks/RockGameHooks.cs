// <copyright file="RockGameHooks.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

using UnityEngine;

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

        /// <summary>
        /// The method return face position as mouse position.
        /// </summary>
        /// <returns>Opponent's face position</returns>
        public static Vector3 GetMousePosition(Vector3 position)
        {
            if (!RockUnity.IsRockEnabled) 
                return position;

            var pos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.8f, 0.0f));
            position.x = pos.x;
            position.y = pos.y;

            return position;
        }
    }
}
