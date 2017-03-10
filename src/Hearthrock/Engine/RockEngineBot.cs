// <copyright file="RockBotClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using Hearthrock.Communication;
    using Hearthrock.Contracts;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class RockEngineBot : IRockBot
    {
        private RockConfiguration configuration;

        public RockEngineBot(RockConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<int> GetMulligan(RockScene scene)
        {
            if (string.IsNullOrEmpty(configuration.BotEndpoint))
            {
                var robot = new Bot.RockBot();
                var mulligan = robot.GetMulligan(scene);
                return mulligan;
            }
            else
            {
                var apiClient = new RockApiClient();
                var mulligan = apiClient.Post<List<int>>(configuration.BotEndpoint + RockConstants.DefaultBotMulliganRelativePath, scene);
                return mulligan;
            }
        }

        public RockAction GetAction(RockScene scene)
        {
            if (string.IsNullOrEmpty(configuration.BotEndpoint))
            {
                var robot = new Bot.RockBot();
                var action = robot.GetAction(scene);
                return action;
            }
            else
            {
                var apiClient = new RockApiClient();
                var action = apiClient.Post<RockAction>(configuration.BotEndpoint + RockConstants.DefaultBotActionRelativePath, scene);
                return action;
            }
        }
    }
}
