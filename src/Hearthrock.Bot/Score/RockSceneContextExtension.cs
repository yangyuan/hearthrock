// <copyright file="RockSceneContextExtension.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot.Score
{
    using Hearthrock.Bot.Exceptions;
    using Hearthrock.Contracts;

    /// <summary>
    /// Extension for RockSceneContext
    /// </summary>
    public static class RockSceneContextExtension
    {
        /// <summary>
        /// Get a RockCard by id.
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId.</param>
        /// <returns>The RockCard</returns>
        public static RockCard GetRockCard(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.GetRockObjectAs<RockCard>(id);
        }

        /// <summary>
        /// Get a RockMinion by id
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId.</param>
        /// <returns>The RockMinion.</returns>
        public static RockMinion GetRockMinion(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.GetRockObjectAs<RockMinion>(id);
        }

        /// <summary>
        /// Get a RockHero by id
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId.</param>
        /// <returns>The RockHero.</returns>
        public static RockHero GetRockHero(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.GetRockObjectAs<RockHero>(id);
        }

        /// <summary>
        /// Is RockObject FriendlyHeroPower.
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a FriendlyHeroPower</returns>
        public static bool IsFriendlyHeroPower(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.IsObjectType(id, RockObjectType.FriendlyHeroPower);
        }

        /// <summary>
        /// Is RockObject IsFriendlyCard.
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a IsFriendlyCard</returns>
        public static bool IsFriendlyCard(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.IsObjectType(id, RockObjectType.FriendlyCard);
        }

        /// <summary>
        /// Is RockObject IsFriendlyCard.
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a IsFriendlyCard</returns>
        public static bool IsFriendlyMinion(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.IsObjectType(id, RockObjectType.FriendlyMinion);
        }

        /// <summary>
        /// Is RockObject is EnemyMinion.
        /// </summary>
        /// <param name="rockSceneContext">The RockSceneContext.</param>
        /// <param name="id">The RockId</param>
        /// <returns>True if it is a IsFriendlyCard</returns>
        public static bool IsEnemyMinion(this RockSceneContext rockSceneContext, int id)
        {
            return rockSceneContext.IsObjectType(id, RockObjectType.EnemyMinion);
        }
    }
}
