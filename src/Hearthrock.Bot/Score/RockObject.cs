// <copyright file="RockObject.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using Hearthrock.Contracts;

    /// <summary>
    /// The RockObject contains a IRockObject and a RockObjectType.
    /// </summary>
    public class RockObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RockObject" /> class.
        /// </summary>
        /// <param name="obj">The IRockObject</param>
        /// <param name="type">The RockObjectType</param>
        public RockObject(IRockObject obj, RockObjectType type)
        {
            this.Object = obj;
            this.ObjectType = type;
        }

        /// <summary>
        /// Gets the IRockObject.
        /// </summary>
        public IRockObject Object { get; private set; }

        /// <summary>
        /// Gets the RockObjectType.
        /// </summary>
        public RockObjectType ObjectType { get; private set; }
    }
}
