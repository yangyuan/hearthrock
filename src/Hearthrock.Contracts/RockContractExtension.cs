// <copyright file="RockContractExtension.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Hearthrock.Contracts
{
    public static class RockContractExtension
    {
        public static bool RequireTarget(this RockCard card)
        {
            return card.ActionRequirements.Contains(RockPlayRequirement.RequireTargetToPlay);
        }

        public static bool CanTargetHero(this RockCard card)
        {
            var includedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireEnemyTarget,
                RockPlayRequirement.RequireMinionTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }

        public static bool CanTargetEnemyHero(this RockCard card)
        {
            var includedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireFriendlyTarget,
                RockPlayRequirement.RequireMinionTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }

        public static bool CanTargetMinion(this RockCard card)
        {
            var includedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireEnemyTarget,
                RockPlayRequirement.RequireHeroTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }
        public static bool CanTargetEnemyMinion(this RockCard card)
        {
            var includedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockPlayRequirement> {
                RockPlayRequirement.RequireFriendlyTarget,
                RockPlayRequirement.RequireHeroTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }




        private static bool CheckRequirement(this RockCard card, List<RockPlayRequirement>  includedRequirements, List<RockPlayRequirement> excludedRequirements)
        {
            foreach (var requirement in includedRequirements)
            {
                if (!card.ActionRequirements.Contains(requirement))
                {
                    return false;
                }
            }

            foreach (var requirement in excludedRequirements)
            {
                if (card.ActionRequirements.Contains(requirement))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
