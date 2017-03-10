// <copyright file="RockEngineBot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System.Collections.Generic;

    using Hearthrock.Communication;
    using Hearthrock.Contracts;

    /// <summary>
    /// RockEngine's IRockBot
    /// </summary>
    public class RockEngineBot : IRockBot
    {
        /// <summary>
        /// The RockConfiguration.
        /// </summary>
        private RockConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockEngineBot" /> class.
        /// </summary>
        /// <param name="configuration">The RockConfiguration.</param>
        public RockEngineBot(RockConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public List<int> GetMulliganAction(RockScene scene)
        {
            if (string.IsNullOrEmpty(configuration.BotEndpoint))
            {
                var robot = new Bot.RockBot();
                var mulligan = robot.GetMulliganAction(scene);
                return mulligan;
            }
            else
            {
                var apiClient = new RockApiClient();
                var mulligan = apiClient.Post<List<int>>(configuration.BotEndpoint + RockConstants.DefaultBotMulliganRelativePath, scene);
                return mulligan;
            }
        }

        /// <summary>
        /// Generate an action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The action.</returns>
        public List<int> GetPlayAction(RockScene scene)
        {
            if (string.IsNullOrEmpty(configuration.BotEndpoint))
            {
                var robot = new Bot.RockBot();
                var action = robot.GetPlayAction(scene);
                return action;
            }
            else
            {
                var apiClient = new RockApiClient();
                var action = apiClient.Post<List<int>>(configuration.BotEndpoint + RockConstants.DefaultBotPlayRelativePath, scene);
                return action;
            }
        }
    }
}
