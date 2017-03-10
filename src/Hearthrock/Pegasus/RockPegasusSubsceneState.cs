// <copyright file="RockPegasusSubsceneState.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    /// <summary>
    /// Subscene State for Pegasus.
    /// </summary>
    public enum RockPegasusSubsceneState
    {
        /// <summary>
        /// The None.
        /// </summary>
        None,

        /// <summary>
        /// Wait for choose a mode.
        /// </summary>
        WaitForChooseMode,

        /// <summary>
        /// Wait for choose a deck.
        /// </summary>
        WaitForChooseDeck,

        /// <summary>
        /// Wait for choose an opponent.
        /// </summary>
        WaitForChooseOpponent,

        /// <summary>
        /// All set.
        /// </summary>
        Ready
    }
}
