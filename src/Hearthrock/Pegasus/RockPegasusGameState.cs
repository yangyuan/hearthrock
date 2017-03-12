// <copyright file="RockPegasusGameState.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    /// <summary>
    /// GameState of Pegasus
    /// </summary>
    public enum RockPegasusGameState
    {
        /// <summary>
        /// The None.
        /// </summary>
        None,

        /// <summary>
        /// Game is over.
        /// </summary>
        GameOver,

        /// <summary>
        /// Wait for mulligan action.
        /// </summary>
        WaitForMulligan,

        /// <summary>
        /// Wait for play action.
        /// </summary>
        WaitForPlay,

        /// <summary>
        /// Blocking by opponent, system, or some animation.
        /// </summary>
        Blocking
    }
}
