// <copyright file="RockRobotClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Engine
{
    using Hearthrock.Contracts;
    using Hearthrock.Serialization;
    using System;
    using System.Net;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class RockBotClient
    {

        private RockConfiguration configuration;
        private WebClient webClient;

        public RockBotClient(RockConfiguration configuration)
        {
            this.configuration = configuration;
            webClient = new WebClient();
        }

        public RockAction GetAction(RockScene scene)
        {
            var robot = new Bot.RockBot();
            var action = robot.GetAction(scene);

            SendTrace(RockJsonSerializer.Serialize(scene));
            if (action != null)
            {
                SendTrace(RockJsonSerializer.Serialize(action));
            } else
            {
                SendTrace("NO ACTION!");
            }
            return action;

            try
            {
                string sceneJson = RockJsonSerializer.Serialize(scene);
                string actionJson = this.Post(configuration.BotEndpoint + "action", sceneJson);
                SendTrace(actionJson);
                var rockAction = RockJsonSerializer.Deserialize<RockAction>(actionJson);
                SendTrace(RockJsonSerializer.Serialize(rockAction));
                return rockAction;
            }
            catch (Exception e)
            {
                // for any exception
                Console.WriteLine(e);
            }

            return null;
        }

        public void SendTrace(string message)
        {
            try
            {
                string json = $"{{\"trace\":\"{message}\"}}";
                this.Post(configuration.TraceEndpoint, json);
            }
            catch (Exception e)
            {
                // for any exception
                Console.WriteLine(e);
            }
        }

        private T Post<T>(string endpoint, object obj)
        {
            var ret  = this.Post(endpoint, JsonUtility.ToJson(obj));
            return JsonUtility.FromJson<T>(ret);
        }

        private string Post(string endpoint, string json)
        {
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            return webClient.UploadString(endpoint, json);
        }
    }
}
