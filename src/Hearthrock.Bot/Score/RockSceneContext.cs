// <copyright file="RockSceneContext.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using System;
    using System.Collections.Generic;

    using Hearthrock.Bot.Algorithm;
    using Hearthrock.Bot.Exceptions;
    using Hearthrock.Contracts;

    /// <summary>
    /// Context class for RockScene.
    /// The structure of RockScene is not good for analysis,
    /// this class provide some cache and help methods to simplify the usage if RockScene.
    /// </summary>
    public class RockSceneContext
    {
        /// <summary>
        /// The RockObjects in RockScene.
        /// </summary>
        private Dictionary<int, RockObject> objects;

        /// <summary>
        /// The PlayAction costs.
        /// </summary>
        private Dictionary<int, int> costs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RockSceneContext" /> class.
        /// </summary>
        /// <param name="scene">The RockScene</param>
        public RockSceneContext(RockScene scene)
        {
            this.Scene = scene;

            this.objects = new Dictionary<int, RockObject>();
            this.costs = new Dictionary<int, int>();
            this.ExtractRockObjects(scene);
            this.AnalyzePlayOptionCosts(scene);
        }

        /// <summary>
        /// Gets the RockScene.
        /// </summary>
        public RockScene Scene { get; private set; }

        /// <summary>
        /// Get the minion wasted cost when use this 
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>The cost</returns>
        public int GetMininWastedCost(int id)
        {
            int space = this.Scene.Self.Resources;
            List<int> values = new List<int>();

            foreach (int value in this.costs.Values)
            {
                values.Add(value);
            }

            int ret = Knapsack.ComputeMinWastedSpace(space, values.ToArray());

            foreach (RockCard handCard in this.Scene.Self.Cards)
            {
                if (handCard.CardId == "GAME_005")
                {
                    return Math.Min(ret, Knapsack.ComputeMinWastedSpace(space + 1, values.ToArray()));
                }
            }

            return ret;
        }

        /// <summary>
        /// Is RockObject IsFriendlyCard.
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <param name="type">The RockObjectType</param>
        /// <returns>True if it is a certain RockObjectType</returns>
        public bool IsObjectType(int id, RockObjectType type)
        {
            RockObject obj;
            if (this.objects.TryGetValue(id, out obj))
            {
                if (obj.ObjectType == type)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get RockObject by RockId.
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>The RockObject or null</returns>
        public RockObject GetRockObject(int id)
        {
            RockObject obj;
            if (this.objects.TryGetValue(id, out obj))
            {
                return obj;
            }

            throw new BotException("The RockObject cannot be found");
        }

        /// <summary>
        /// Get a RockObject as T by id.
        /// </summary>
        /// <typeparam name="T">The RockObject subtype.</typeparam>
        /// <param name="id">The RockId.</param>
        /// <returns>The RockObject as T.</returns>
        public T GetRockObjectAs<T>(int id)
        {
            RockObject obj = this.GetRockObject(id);
            if (obj.Object is T)
            {
                return (T)obj.Object;
            }

            throw new BotException($"The RockObject is not a {nameof(T)}, it's a {obj.Object.GetType()}. The RockObjectType is {obj.ObjectType}");
        }

        /// <summary>
        /// Analyze PlayOption costs.
        /// </summary>
        /// <param name="scene">The RockScene</param>
        private void AnalyzePlayOptionCosts(RockScene scene)
        {
            foreach (var playOption in scene.PlayOptions)
            {
                int id = playOption[0];

                if (!this.costs.ContainsKey(id))
                {
                    if (this.IsObjectType(id, RockObjectType.FriendlyHeroPower))
                    {
                        this.costs.Add(id, 2);
                    }
                    else if (this.IsObjectType(id, RockObjectType.FriendlyCard))
                    {
                        var card = this.GetRockCard(id);
                        this.costs.Add(id, card.Cost);
                    }
                    else
                    {
                        this.costs.Add(id, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Extract RockObjects in the RockScene into the Dictionary.
        /// </summary>
        /// <param name="scene">The RockScene</param>
        private void ExtractRockObjects(RockScene scene)
        {
            this.ExtractRockObjects(scene.Self, true);
            this.ExtractRockObjects(scene.Opponent, false);
        }

        /// <summary>
        /// Extract RockObjects in the RockPlayer into the Dictionary.
        /// </summary>
        /// <param name="rockPlayer">The RockPlayer</param>
        /// <param name="friendly">If the player is friendly or not.</param>
        private void ExtractRockObjects(RockPlayer rockPlayer, bool friendly)
        {
            foreach (var card in rockPlayer.Cards)
            {
                this.RegisterRockObject(card, friendly ? RockObjectType.FriendlyCard : RockObjectType.EnemyCard);
                if (card.Options.Count > 0)
                {
                    foreach (var option in card.Options)
                    {
                        this.RegisterRockObject(option, friendly ? RockObjectType.FriendlyCard : RockObjectType.EnemyCard);
                    }
                }
            }

            foreach (var minion in rockPlayer.Minions)
            {
                this.RegisterRockObject(minion, friendly ? RockObjectType.FriendlyMinion : RockObjectType.EnemyMinion);
            }

            this.RegisterRockObject(rockPlayer.Hero, friendly ? RockObjectType.FriendlyHero : RockObjectType.EnemyHero);
            this.RegisterRockObject(rockPlayer.Power, friendly ? RockObjectType.FriendlyHeroPower : RockObjectType.EnemyHeroPower);
            if (rockPlayer.HasWeapon)
            {
                this.RegisterRockObject(rockPlayer.Weapon, friendly ? RockObjectType.FriendlyHeroWeapon : RockObjectType.EnemyHeroWeapon);
            }
        }

        /// <summary>
        /// Register a RockObject in the Dictionary.
        /// </summary>
        /// <param name="rockObject">The IRockObject</param>
        /// <param name="type">The RockObjectType</param>
        private void RegisterRockObject(IRockObject rockObject, RockObjectType type)
        {
            if (!this.objects.ContainsKey(rockObject.RockId))
            {
                this.objects.Add(rockObject.RockId, new RockObject(rockObject, type));
            }
        }
    }
}
