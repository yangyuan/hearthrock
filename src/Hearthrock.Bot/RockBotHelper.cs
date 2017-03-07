// <copyright file="RockBotHelper.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Bot
{
    using System.Collections.Generic;

    using Hearthrock.Contracts;

    /// <summary>
    /// Helper class for SampleRobot
    /// </summary>
    public static class RockBotHelper
    {
        /// <summary>
        /// This function will compare the enemy minion's health and damage.
        /// This method is dumb, to make it smart, we can make it return double.
        /// </summary>
        /// <param name="enimyMinoin">The RockMinion of enemy minion.</param>
        /// <returns>true iff we think this minion is dangerous.</returns>
        public static bool IsEnemyMinoinDangerous(RockMinion enemyMinoin)
        {
            if (enemyMinoin.Health > 4)
            {
                if (enemyMinoin.Damage - enemyMinoin.Health > 4)
                {
                    return true;
                }
            }
            else if (enemyMinoin.Health > 2) // 3,4
            {
                if (enemyMinoin.Damage - enemyMinoin.Health > 3)
                {
                    return true;
                }
            }
            else // 1,2
            {
                if (enemyMinoin.Damage - enemyMinoin.Health > 2)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This function will compare the enemy minion's health and damage.
        /// This method is dumb, to make it smart, we can make it return double.
        /// </summary>
        /// <param name="enimyMinoin">The RockMinion of enemy minion.</param>
        /// <returns>true iff we think this minion is great dangerous.</returns>
        public static bool IsEnemyMinoinGreatDangerous(RockMinion enemyMinoin)
        {
            if (enemyMinoin.Health > 3)// (10,4)
            {
                if (enemyMinoin.Damage - enemyMinoin.Health > 5)
                {
                    return true;
                }
            }
            else if (enemyMinoin.Health > 2) // (8,3)
            {
                if (enemyMinoin.Damage - enemyMinoin.Health > 4)
                {
                    return true;
                }
            }
            else // (5,1) (6,2)
            {
                if (enemyMinoin.Damage - enemyMinoin.Health > 3)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The damage enemy can do in the next turn.
        /// </summary>
        /// <param name="enemyHero">The hero of enemy.</param>
        /// <param name="enemyMinions">The minions of enemy.</param>
        /// <returns>The overall damage.</returns>
        public static int ComputeEnemyNextTurnDamage(RockHero enemyHero, List<RockMinion> enemyMinions)
        {
            int enemyNextTurnDamage = 0;

            foreach (var enemyMinion in enemyMinions)
            {
                enemyNextTurnDamage += enemyMinion.Damage;
                if (enemyMinion.HasWindfury)
                {
                    enemyNextTurnDamage += enemyMinion.Damage;
                }
            }

            enemyNextTurnDamage += enemyHero.Damage;

            return enemyNextTurnDamage;
        }

        /// <summary>
        /// Minoin power compare function, can be used to sort minions.
        /// </summary>
        /// <param name="x">The first minion.</param>
        /// <param name="y">The second minion.</param>
        /// <returns>Compare result.</returns>
        public static int MinoinPowerCompare(RockMinion x, RockMinion y)
        {
            return x.GetPower() - y.GetPower();
        }

        /// <summary>
        /// Get the "power" of a minion, "power" is an int value to describ the minion's ability.
        /// </summary>
        /// <param name="rockMinion">The minion.</param>
        /// <returns>the "power" of the minion.</returns>
        private static int GetPower(this RockMinion rockMinion)
        {
            int power = rockMinion.Damage;
            if (rockMinion.HasWindfury)
            {
                power += rockMinion.Damage;
            }
            power -= rockMinion.Health;
            if (rockMinion.HasDivineShield)
            {
                power -= 1;
            }
            return power;
        }
    }
}
