// <copyright file="RockActionTests.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Tests.Contracts
{
    using System.Collections.Generic;

    using Hearthrock.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The tests for RockAction.
    /// </summary>
    [TestClass]
    public class RockActionTests
    {
        /// <summary>
        /// TestMethod for RockAction factory methods.
        /// </summary>
        [TestMethod]
        public void TestRockActionFactoryMethods()
        {
            var action = RockAction.Create(new List<int>());
            Assert.AreEqual(action.Version, 1);
            Assert.AreEqual(action.Objects.Count, 0);
            Assert.AreEqual(action.Slot, -1);

            action = RockAction.Create(new List<int>(), 2);
            Assert.AreEqual(action.Version, 1);
            Assert.AreEqual(action.Objects.Count, 0);
            Assert.AreEqual(action.Slot, 2);
        }
    }
}
