// <copyright file="RockPegasusFactory.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Pegasus
{
    using Hearthrock.Diagnostics;
    using Hearthrock.Pegasus.Internal;

    /// <summary>
    /// The Factory for IRockPegasus. 
    /// </summary>
    public static class RockPegasusFactory
    {
        /// <summary>
        /// The factory method for IRockPegasus. 
        /// </summary>
        /// <param name="tracer">The RockTracer.</param>
        /// <returns>A IRockPegasus instance.</returns>
        public static IRockPegasus CreatePegasus(RockTracer tracer)
        {
            return new RockPegasus(tracer);
        }
    }
}
