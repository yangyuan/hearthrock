// <copyright file="RockSceneContext.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using System.Collections.Generic;

    using Hearthrock.Contracts;

    /// <summary>
    /// Context class for RockScene.
    /// The structure of RockScene is not good for analysis,
    /// this class provide some cache and help methods to simplify the usage if RockScene.
    /// </summary>
    public class RockSceneContext
    {
        /// <summary>
        /// The RockScene.
        /// </summary>
        private RockScene scene;

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
            this.scene = scene;

            this.objects = new Dictionary<int, RockObject>();
            this.costs = new Dictionary<int, int>();
            this.ExtractRockObjects(scene);
            this.AnalyzePlayOptionCosts(scene);
        }

        /// <summary>
        /// Get a RockCard by id.
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>The RockCard</returns>
        public RockCard GetCard(int id)
        {
            return (RockCard)this.objects[id].Object;
        }

        /// <summary>
        /// Get the cost of a card/action
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>The cost</returns>
        public int GetCost(int id)
        {
            return this.costs[id];
        }

        /// <summary>
        /// Get the minion wasted cost when use this 
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>The cost</returns>
        public int GetMininWastedCost(int id)
        {
            // TODO: should use dynamic programing to solve the cost
            return 0;
        }

        /// <summary>
        /// Is RockObject FriendlyHeroPower.
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a FriendlyHeroPower</returns>
        public bool IsFriendlyHeroPower(int id)
        {
            RockObject obj;
            if (this.objects.TryGetValue(id, out obj))
            {
                if (obj.ObjectType == RockObjectType.FriendlyHeroPower)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Is RockObject IsFriendlyCard.
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a IsFriendlyCard</returns>
        public bool IsFriendlyCard(int id)
        {
            RockObject obj;
            if (this.objects.TryGetValue(id, out obj))
            {
                if (obj.ObjectType == RockObjectType.FriendlyCard)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Is RockObject IsFriendlyCard.
        /// </summary>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a IsFriendlyCard</returns>
        public bool IsFriendlyMinion(int id)
        {
            RockObject obj;
            if (this.objects.TryGetValue(id, out obj))
            {
                if (obj.ObjectType == RockObjectType.FriendlyMinion)
                {
                    return true;
                }
            }

            return false;
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

            return null;
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
                    if (this.IsFriendlyHeroPower(id))
                    {
                        this.costs.Add(id, 2);
                    }
                    else if (this.IsFriendlyCard(id))
                    {
                        var card = this.GetCard(id);
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
