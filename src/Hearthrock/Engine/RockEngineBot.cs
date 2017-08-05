// <copyright file="RockEngineBot.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using System;
    using System.Collections.Generic;

    using Hearthrock.Communication;
    using Hearthrock.Contracts;
    using Hearthrock.Diagnostics;

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
        /// The RockTracer.
        /// </summary>
        private RockTracer tracer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockEngineBot" /> class.
        /// </summary>
        /// <param name="configuration">The RockConfiguration.</param>
        /// <param name="tracer">The RockTracer.</param>
        public RockEngineBot(RockConfiguration configuration, RockTracer tracer)
        {
            this.configuration = configuration;
            this.tracer = tracer;
        }

        /// <summary>
        /// Generate a mulligan action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be mulligan-ed.</returns>
        public RockAction GetMulliganAction(RockScene scene)
        {
            this.tracer.Verbose(RockJsonSerializer.Serialize(scene));

            try
            {
                if (string.IsNullOrEmpty(this.configuration.BotEndpoint))
                {
                    var robot = new Bot.RockBot();
                    var mulligan = robot.GetMulliganAction(scene);
                    this.tracer.Verbose(RockJsonSerializer.Serialize(mulligan));
                    return mulligan;
                }
                else
                {
                    var apiClient = new RockApiClient();
                    var mulligan = apiClient.Post<RockAction>($"{this.configuration.BotEndpoint}{RockConstants.DefaultBotMulliganRelativePath}", scene);
                    this.tracer.Verbose(RockJsonSerializer.Serialize(mulligan));
                    return mulligan;
                }
            }
            catch (Exception e)
            {
                this.tracer.Error($"Unexpected Exception from Bot: {e}");
                return RockAction.Create();
            }
        }

        /// <summary>
        /// Generate a play action for current scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The cards to be played.</returns>
        public RockAction GetPlayAction(RockScene scene)
        {
            this.tracer.Verbose(RockJsonSerializer.Serialize(scene));

            try
            {
                if (string.IsNullOrEmpty(this.configuration.BotEndpoint))
                {
                    var robot = new Bot.RockBot();
                    var action = robot.GetPlayAction(scene);
                    this.tracer.Verbose(RockJsonSerializer.Serialize(action));
                    return action;
                }
                else
                {
                    var apiClient = new RockApiClient();
                    var action = apiClient.Post<RockAction>($"{this.configuration.BotEndpoint}{RockConstants.DefaultBotPlayRelativePath}", scene);
                    this.tracer.Verbose(RockJsonSerializer.Serialize(action));
                    return action;
                }
            }
            catch (Exception e)
            {
                this.tracer.Error($"Unexpected Exception from Bot: {e}");
                return RockAction.Create();
            }
        }
    }
}
