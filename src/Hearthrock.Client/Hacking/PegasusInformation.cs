// <copyright file="PegasusInformation.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Client.Hacking
{
    /// <summary>
    /// The class to hold the information of Pegasus
    /// </summary>
    public class PegasusInformation
    {
        /// <summary>
        /// Gets or sets the checksum of main assembly file.
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// Gets or sets the supported version.
        /// </summary>
        public string Version { get; set; }
    }
}
