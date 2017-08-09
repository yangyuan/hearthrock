// <copyright file="RockPegasusExtensions.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus.Internal
{
    using System.Reflection;

    /// <summary>
    /// Extensions for Pegasus classes.
    /// </summary>
    public static class RockPegasusExtensions
    {
        /// <summary>
        /// Expose IsBlockingPowerProcessor of GameState.
        /// </summary>
        /// <param name="gameState">The GameState</param>
        /// <returns>The return of IsBlockingPowerProcessor</returns>
        public static bool IsBlockingPowerProcessor(this GameState gameState)
        {
            MethodInfo methodInfo = gameState.GetType().GetMethod("IsBlockingPowerProcessor", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)methodInfo.Invoke(gameState, null);
        }

        /// <summary>
        /// Set the value of ActivityDetected of InactivePlayerKicker.
        /// </summary>
        /// <param name="inactivePlayerKicker">The InactivePlayerKicker</param>
        /// <param name="value">The Value of ActivityDetected</param>
        public static void SetActivityDetected(this InactivePlayerKicker inactivePlayerKicker, bool value)
        {
            FieldInfo fieldInfo = inactivePlayerKicker.GetType().GetField("m_activityDetected", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(inactivePlayerKicker, value);
        }

        /// <summary>
        /// Get the number of permanent resources.
        /// </summary>
        /// <param name="player">The Player</param>
        /// <returns>The number of permanent resources</returns>
        public static int GetNumPermanentResources(this Player player)
        {
            return player.GetTag(GAME_TAG.RESOURCES);
        }

        /// <summary>
        /// Get the number of temporary resources.
        /// </summary>
        /// <param name="player">The Player</param>
        /// <returns>The number of temporary resources</returns>
        public static int GetNumTemporaryResources(this Player player)
        {
            return player.GetTag(GAME_TAG.TEMP_RESOURCES);
        }
    }
}
