// <copyright file="RockConstants.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    /// <summary>
    /// Constants of Hearthrock
    /// </summary>
    public static class RockConstants
    {
        /// <summary>
        /// Default Endpoint
        /// </summary>
        public const string DefaultEndpoint = "http://127.0.0.1:7625";

        /// <summary>
        /// Default url relative path to send trace.
        /// </summary>
        public const string DefaultTracePath = "/trace";

        /// <summary>
        /// Default url relative path of bot.
        /// </summary>
        public const string DefaultBotPath = "/";

        /// <summary>
        /// Default url relative path of bot play action.
        /// </summary>
        public const string DefaultBotPlayRelativePath = "play";

        /// <summary>
        /// Default url relative path of bot mulligan action.
        /// </summary>
        public const string DefaultBotMulliganRelativePath = "mulligan";
    }
}
