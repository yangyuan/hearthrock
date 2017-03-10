// <copyright file="RockJsonSerializerTests.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Tests.CommunicationTests
{
    using System.Collections.Generic;

    using Hearthrock.Communication;
    using Hearthrock.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="RockJsonSerializer" /> class.
    /// </summary>
    [TestClass]
    public class RockJsonSerializerTests
    {
        [TestMethod]
        public void TestMethod2()
        {
            var rockScene = GenerateRockScene();
            var x = RockJsonSerializer.Serialize(rockScene);
        }


        private static RockScene GenerateRockScene()
        {
            var rockScene = new RockScene();
            rockScene.Self = GenerateRockPlayer();
            rockScene.Opponent = GenerateRockPlayer();

            return rockScene;
        }

        private static RockPlayer GenerateRockPlayer()
        {
            var rockPlayer = new RockPlayer();
            rockPlayer.Cards = new List<RockCard>();

            var rockCard = new RockCard();
            rockCard.CardId = "GAME_005";

            rockPlayer.Cards.Add(rockCard);

            rockPlayer.Hero = GenerateRockHero();
            return rockPlayer;
        }

        private static RockHero GenerateRockHero()
        {
            var rockHero = new RockHero();
            return rockHero;
        }
    }
}
