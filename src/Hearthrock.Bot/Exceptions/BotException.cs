// <copyright file="BotException.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Exceptions
{
    using System;

    /// <summary>
    /// Base exception for Hearthrock.Bot namespace.
    /// </summary>
    public class BotException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotException" /> class.
        /// </summary>
        public BotException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BotException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BotException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BotException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference.</param>
        public BotException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
