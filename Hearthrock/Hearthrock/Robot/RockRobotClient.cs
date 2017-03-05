// <copyright file="RockRobotClient.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Robot
{
    using Configuration;
    using Contracts;
    using MiniJson;
    using System;
    using System.Net;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class RockRobotClient
    {

        private RockConfiguration configuration;
        private WebClient webClient;

        public RockRobotClient(RockConfiguration configuration)
        {
            this.configuration = configuration;
            webClient = new WebClient();
        }

        public RockAction GetAction(RockScene scene)
        {
            try
            {
                string sceneJson = MiniJsonSerializer.Serialize(scene);
                string actionJson = this.Post(configuration.RobotEndpoint + "action", sceneJson);
                SendTrace(actionJson);
                var rockAction = MiniJsonSerializer.Deserialize<RockAction>(actionJson);
                SendTrace(MiniJsonSerializer.Serialize(rockAction));
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
