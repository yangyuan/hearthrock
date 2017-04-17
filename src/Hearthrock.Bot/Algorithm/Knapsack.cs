// <copyright file="Knapsack.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Algorithm
{
    using System;

    /// <summary>
    /// class for Knapsack algorithms
    /// </summary>
    public static class Knapsack
    {
        /// <summary>
        /// Compute min wasted space for a bag.
        /// </summary>
        /// <param name="space">The bag space.</param>
        /// <param name="values">The values to be put into bad.</param>
        /// <returns>Possible min wasted space.</returns>
        public static int ComputeMinWastedSpace(int space, int[] values)
        {
            if (values.Length == 0)
            {
                return space;
            }

            int lastValue = values[values.Length - 1];
            int[] resuValues = values.Slice(0, values.Length - 1);

            if (space - lastValue >= 0)
            {
                return Math.Min(
                    ComputeMinWastedSpace(space - lastValue, resuValues),
                    ComputeMinWastedSpace(space, resuValues));
            }
            else
            {
                return ComputeMinWastedSpace(space, resuValues);
            }
        }
    }
}
