// <copyright file="DirectoryAsync.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Action contract of Hearthrock
    /// </summary>
    class RockAction
    {
        /// <summary>
        /// Source entity of the action.
        /// </summary>
        public int Source { set; get; }

        /// <summary>
        /// Target entities of the action.
        /// </summary>
        public List<int> Targets { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rockIds"></param>
        /// <returns></returns>
        public static RockAction Create(params int[] rockIds)
        {
            var action = new RockAction();
            action.Source = rockIds[0];
            action.Targets = new List<int>();
            for (var i=0; i< rockIds.Length; i++)
            {
                action.Targets.Add(i);
            }

            return action;
        }
    }
}
