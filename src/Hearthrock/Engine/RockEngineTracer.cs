// <copyright file="RockEngineTracer.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

using Hearthrock.Communication;
using Hearthrock.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Hearthrock.Engine
{
    /// <summary>
    /// Rock Engine Tracer
    /// </summary>
    public class RockEngineTracer
    {
        public TraceLevel Level { get; private set; }

        private RockConfiguration configuration;

        private RockApiClient webApiClient;

        public RockEngineTracer(RockConfiguration configuration)
        {
            this.configuration = configuration;
            this.webApiClient = new RockApiClient();
        }


        public void Error(string message)
        {
            Trace(TraceLevel.Error, message);
        }

        public void Warning(string message)
        {
            Trace(TraceLevel.Warning, message);
        }

        public void Info(string message)
        {
            Trace(TraceLevel.Info, message);
        }

        public void Verbose(string message)
        {
            Trace(TraceLevel.Verbose, message);
        }

        private void Trace(TraceLevel traceLevel, string message)
        {
            var traceMessage = new Dictionary<string, string>();
            traceMessage.Add("Level", traceLevel.ToString());
            traceMessage.Add("Message", message);

            Console.WriteLine($"[{traceLevel}] {message}");

            if (!string.IsNullOrEmpty(this.configuration.TraceEndpoint))
            {
                new Thread(delegate ()
                {
                    try
                    {
                        webApiClient.Post(configuration.TraceEndpoint, traceMessage);
                    }
                    catch (Exception e)
                    {
                        // for any exception
                        Console.WriteLine(e);
                    }
                }).Start();
            }
        }

        SceneMgr.Mode scenemgr_mode = SceneMgr.Mode.INVALID;
        public void TraceSceneMode(SceneMgr.Mode mode)
        {
            if (scenemgr_mode != mode)
            {
                scenemgr_mode = mode;
                this.Info(scenemgr_mode.ToString());
            }
        }
    }
}
