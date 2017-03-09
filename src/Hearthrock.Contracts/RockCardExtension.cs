// <copyright file="RockConfiguration.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Hearthrock.Contracts
{
    public static class RockCardExtension
    {
        public static bool RequireTarget(this RockCard card)
        {
            return card.ActionRequirements.Contains(RockActionRequirement.RequireTargetToPlay);
        }

        public static bool CanTargetHero(this RockCard card)
        {
            var includedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireEnemyTarget,
                RockActionRequirement.RequireMinionTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }

        public static bool CanTargetEnemyHero(this RockCard card)
        {
            var includedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireFriendlyTarget,
                RockActionRequirement.RequireMinionTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }

        public static bool CanTargetMinion(this RockCard card)
        {
            var includedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireEnemyTarget,
                RockActionRequirement.RequireHeroTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }
        public static bool CanTargetEnemyMinion(this RockCard card)
        {
            var includedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireTargetToPlay
            };

            var excludedRequirements = new List<RockActionRequirement> {
                RockActionRequirement.RequireFriendlyTarget,
                RockActionRequirement.RequireHeroTarget,
            };

            return card.CheckRequirement(includedRequirements, excludedRequirements);
        }




        private static bool CheckRequirement(this RockCard card, List<RockActionRequirement>  includedRequirements, List<RockActionRequirement> excludedRequirements)
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
