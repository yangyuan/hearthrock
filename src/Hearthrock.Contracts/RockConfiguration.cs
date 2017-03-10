// <copyright file="RockConfiguration.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    /// <summary>
    /// Configuration contract of Hearthrock
    /// </summary>
    public class RockConfiguration
    {
        /// <summary>
        /// Gets or sets the TraceLevel.
        /// Off = 0, Error = 1, Warning = 2, Info = 3, Verbose = 4.
        /// </summary>
        public int TraceLevel { get; set; }

        /// <summary>
        /// Gets or sets the endpoint of Hearthrock bot.
        /// Empty of null means use local bot.
        /// </summary>
        public string BotEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the endpoint of Hearthrock trace.
        /// Empty of null means use standard output to output trace.
        /// </summary>
        public string TraceEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the preferred game mode.
        /// </summary>
        public RockGameMode GameMode { get; set; }

        /// <summary>
        /// Gets or sets the preferred deck index.
        /// Current selected deck will be used if the deck index is not available.
        /// </summary>
        public int DeckIndex { get; set; }

        /// <summary>
        /// Gets or sets the preferred opponent index.
        /// Index starts from one, and zero means random.
        /// </summary>
        public int OpponentIndex { get; set; }
    }
}
