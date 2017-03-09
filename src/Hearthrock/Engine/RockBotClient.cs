// <copyright file="RockBotClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using Hearthrock.Contracts;
    using Hearthrock.Communication;
    using System;
    using System.Net;
    using System.Threading;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class RockBotClient
    {


        private RockConfiguration configuration;

        public RockBotClient(RockConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public RockAction GetAction(RockScene scene)
        {
            var robot = new Bot.RockBot();
            var action = robot.GetAction(scene);

            /// SendTrace(RockJsonSerializer.Serialize(scene));
            /// if (action != null)
            /// {
            ///     SendTrace(RockJsonSerializer.Serialize(action));
            /// } else
            /// {
            ///     SendTrace("NO ACTION!");
            /// }
            return action;

            //// try
            //// {
            ////     string sceneJson = RockJsonSerializer.Serialize(scene);
            ////     string actionJson = this.Post(configuration.BotEndpoint + "action", sceneJson);
            ////     SendTrace(actionJson);
            ////     var rockAction = RockJsonSerializer.Deserialize<RockAction>(actionJson);
            ////     SendTrace(RockJsonSerializer.Serialize(rockAction));
            ////     return rockAction;
            //// }
            //// catch (Exception e)
            //// {
            ////     // for any exception
            ////     Console.WriteLine(e);
            //// }
            //// 
            //// return null;
        }
    }
}
