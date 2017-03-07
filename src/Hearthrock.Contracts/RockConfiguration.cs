// <copyright file="RockConfiguration.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Hearthrock Configuration contract
    /// </summary>
    public class RockConfiguration
    {
        /// <summary>
        /// Endpoint of Hearthrock bot.
        /// Empty of null means use local bot.
        /// </summary>
        public string BotEndpoint { get; set; }

        /// <summary>
        /// Endpoint of Hearthrock trace.
        /// Empty of null means use standard output to output trace.
        /// </summary>
        public string TraceEndpoint { get; set; }

        /// <summary>
        /// Prefered game mode.
        /// </summary>
        public RockGameMode GameMode { get; set; }

        /// <summary>
        /// Prefered deck index.
        /// Current selected deck will be used if the deck index is not available.
        /// </summary>
        public int DeckIndex { get; set; }

        /// <summary>
        /// Prefered opponent index.
        /// Index starts from one, and zero means random.
        /// </summary>
        public int OpponentIndex { get; set; }
    }
}
