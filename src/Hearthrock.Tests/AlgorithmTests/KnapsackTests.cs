// <copyright file="KnapsackTests.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Tests.AlgorithmTests
{
    using Hearthrock.Bot.Algorithm;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="Knapsack" /> class.
    /// </summary>
    [TestClass]
    public class KnapsackTests
    {
        /// <summary>
        /// Tests for ComputeMinWastedSpace.
        /// </summary>
        [TestMethod]
        public void TestComputeMinWastedSpace()
        {
            Assert.AreEqual(4, Knapsack.ComputeMinWastedSpace(5, new int[] { 1 }));
            Assert.AreEqual(0, Knapsack.ComputeMinWastedSpace(5, new int[] { 1, 2, 3 }));
            Assert.AreEqual(1, Knapsack.ComputeMinWastedSpace(7, new int[] { 1, 2, 3 }));
            Assert.AreEqual(0, Knapsack.ComputeMinWastedSpace(6, new int[] { 3, 1, 2, 3 }));
            Assert.AreEqual(0, Knapsack.ComputeMinWastedSpace(8, new int[] { 3, 1, 2, 3 }));
            Assert.AreEqual(1, Knapsack.ComputeMinWastedSpace(4, new int[] { 3, 2, 5, 3 }));
            Assert.AreEqual(0, Knapsack.ComputeMinWastedSpace(6, new int[] { 3, 2, 5, 3 }));
            Assert.AreEqual(6, Knapsack.ComputeMinWastedSpace(6, new int[] { }));
        }
    }
}
