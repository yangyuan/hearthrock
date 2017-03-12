// <copyright file="RockTracer.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Hearthrock.Communication;
    using Hearthrock.Contracts;

    /// <summary>
    /// The tracer for diagnostics.
    /// </summary>
    public class RockTracer
    {
        /// <summary>
        /// The RockConfiguration. 
        /// </summary>
        private RockConfiguration configuration;

        /// <summary>
        /// The RockApiClient.
        /// </summary>
        private RockApiClient webApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockTracer" /> class.
        /// </summary>
        /// <param name="configuration">The RockConfiguration.</param>
        public RockTracer(RockConfiguration configuration)
        {
            this.configuration = configuration;
            this.webApiClient = new RockApiClient();
        }

        /// <summary>
        /// Gets the TraceLevel.
        /// </summary>
        public TraceLevel Level { get; private set; }

        /// <summary>
        /// Trace a message as error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            this.Trace(TraceLevel.Error, message);
        }

        /// <summary>
        /// Trace a message as warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            this.Trace(TraceLevel.Warning, message);
        }

        /// <summary>
        /// Trace a message as info.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            this.Trace(TraceLevel.Info, message);
        }

        /// <summary>
        /// Trace a message as verbose.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Verbose(string message)
        {
            this.Trace(TraceLevel.Verbose, message);
        }

        /// <summary>
        /// Trace a message.
        /// </summary>
        /// <param name="traceLevel">The TraceLevel.</param>
        /// <param name="message">The message.</param>
        private void Trace(TraceLevel traceLevel, string message)
        {
            var traceMessage = new Dictionary<string, string>();
            traceMessage.Add("Level", traceLevel.ToString());
            traceMessage.Add("Message", message);

            Console.WriteLine($"[{traceLevel}] {message}");

            if (!string.IsNullOrEmpty(this.configuration.TraceEndpoint))
            {
                this.webApiClient.PostAsync(this.configuration.TraceEndpoint, traceMessage);
            }
        }
    }
}
