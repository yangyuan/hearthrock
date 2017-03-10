// <copyright file="RockCardExtension.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot
{
    using System.Collections.Generic;
    using Hearthrock.Contracts;

    /// <summary>
    /// Extension for RockCard.
    /// </summary>
    public static class RockCardExtension
    {
        const int RequireMinionTarget = 1;
        const int RequireFriendlyTarget = 2;
        const int RequireEnemyTarget = 3;
        const int RequireTargetToPlay = 11;
        const int RequireHeroTarget = 17;

        /// <summary>
        /// If the card require a target to play.
        /// </summary>
        /// <param name="card">The RockCard.</param>
        /// <returns>True if the card require a target to play.</returns>
        public static bool RequireTarget(this RockCard card)
        {
            return card.PlayRequirements.Contains(RequireTargetToPlay);
        }

        public static bool CanTargetHero(this RockCard card)
        {
            var includedRequirements = new List<int> {
                RequireTargetToPlay
            };

            var excludedRequirements = new List<int> {
                RequireEnemyTarget,
                RequireMinionTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }

        public static bool CanTargetEnemyHero(this RockCard card)
        {
            var includedRequirements = new List<int> {
                RequireTargetToPlay
            };

            var excludedRequirements = new List<int> {
                RequireFriendlyTarget,
                RequireMinionTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }

        public static bool CanTargetMinion(this RockCard card)
        {
            var includedRequirements = new List<int> {
                RequireTargetToPlay
            };

            var excludedRequirements = new List<int> {
                RequireEnemyTarget,
                RequireHeroTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }
        public static bool CanTargetEnemyMinion(this RockCard card)
        {
            var includedRequirements = new List<int> {
                RequireTargetToPlay
            };

            var excludedRequirements = new List<int> {
                RequireFriendlyTarget,
                RequireHeroTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }




        private static bool CheckRequirement(this RockCard card, List<int>  includedRequirements, List<int> excludedRequirements)
        {
            foreach (var requirement in includedRequirements)
            {
                if (!card.PlayRequirements.Contains(requirement))
                {
                    return false;
                }
            }

            foreach (var requirement in excludedRequirements)
            {
                if (card.PlayRequirements.Contains(requirement))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
