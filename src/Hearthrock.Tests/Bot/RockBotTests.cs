

namespace Hearthrock.Tests.Bot
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Hearthrock.Contracts;
    using Hearthrock.Serialization;
    using Hearthrock.Bot;

    [TestClass]
    public class RockBotTests
    {
        [TestMethod]
        public void ScenarioTest1()
        {
            string json = "{\"Self\":{\"Resources\":5,\"PowerAvailable\":false,\"Hero\":{\"RockId\":64,\"Class\":1,\"Damage\":0,\"Health\":23,\"CanAttack\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":65,\"CardId\":\"CS2_034\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[],\"Cards\":[{\"RockId\":20,\"CardId\":\"CS2_172\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":30,\"CardId\":\"CS2_168\",\"Cost\":1,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":16,\"CardId\":\"CS2_120\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":12,\"CardId\":\"CS2_172\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":13,\"CardId\":\"CS2_122\",\"Cost\":3,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":19,\"CardId\":\"EX1_015\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":29,\"CardId\":\"CS2_119\",\"Cost\":4,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":18,\"CardId\":\"EX1_593\",\"Cost\":5,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false}]},\"Opponent\":{\"Resources\":0,\"PowerAvailable\":false,\"Hero\":{\"RockId\":66,\"Class\":1,\"Damage\":0,\"Health\":30,\"CanAttack\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":67,\"CardId\":\"CS1h_001\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[{\"RockId\":0,\"Damage\":1,\"Health\":5,\"BaseHealth\":5,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":0,\"Damage\":1,\"Health\":5,\"BaseHealth\":5,\"IsFrozen\":false,\"IsExhausted\":false,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":0,\"Damage\":1,\"Health\":3,\"BaseHealth\":3,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false}],\"Cards\":[{\"RockId\":51,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":35,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":45,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":50,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":34,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]}}";
            var scene = RockJsonSerializer.Deserialize<RockScene>(json);
            RockBot robot = new RockBot();
            var action = robot.GetAction(scene);

            Assert.IsNotNull(action);
            Assert.IsTrue(IsValidAction(action, scene));
        }

        [TestMethod]
        public void ScenarioTest2()
        {
            string json = "{\"Self\":{\"Resources\":0,\"PowerAvailable\":false,\"Hero\":{\"RockId\":64,\"Class\":1,\"Damage\":0,\"Health\":20,\"CanAttack\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":65,\"CardId\":\"CS2_034\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[{\"RockId\":18,\"Damage\":4,\"Health\":4,\"BaseHealth\":4,\"IsFrozen\":false,\"IsExhausted\":false,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":29,\"Damage\":2,\"Health\":7,\"BaseHealth\":7,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":true,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":20,\"Damage\":3,\"Health\":2,\"BaseHealth\":2,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":true,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false}],\"Cards\":[{\"RockId\":30,\"CardId\":\"CS2_168\",\"Cost\":1,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":16,\"CardId\":\"CS2_120\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":12,\"CardId\":\"CS2_172\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":13,\"CardId\":\"CS2_122\",\"Cost\":3,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":19,\"CardId\":\"EX1_015\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":33,\"CardId\":\"CS2_168\",\"Cost\":1,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false}]},\"Opponent\":{\"Resources\":0,\"PowerAvailable\":false,\"Hero\":{\"RockId\":66,\"Class\":1,\"Damage\":0,\"Health\":27,\"CanAttack\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":67,\"CardId\":\"CS1h_001\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[{\"RockId\":70,\"Damage\":1,\"Health\":8,\"BaseHealth\":8,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":40,\"Damage\":1,\"Health\":5,\"BaseHealth\":5,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":43,\"Damage\":1,\"Health\":3,\"BaseHealth\":3,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":35,\"Damage\":6,\"Health\":6,\"BaseHealth\":6,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false}],\"Cards\":[{\"RockId\":51,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":45,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":50,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":34,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":54,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]}}";
            var scene = RockJsonSerializer.Deserialize<RockScene>(json);
            RockBot robot = new RockBot();
            var action = robot.GetAction(scene);

            Assert.IsNotNull(action);
            Assert.IsTrue(IsValidAction(action, scene));
        }

        [TestMethod]
        public void ScenarioTest3()
        {
            string json = "{\"Self\":{\"Resources\":5,\"PowerAvailable\":false,\"Hero\":{\"RockId\":64,\"Class\":1,\"Damage\":0,\"Health\":17,\"CanAttack\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":65,\"CardId\":\"CS2_034\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[],\"Cards\":[{\"RockId\":5,\"CardId\":\"CS2_172\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":20,\"CardId\":\"CS2_022\",\"Cost\":4,\"IsSpell\":true,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":4,\"CardId\":\"CS2_029\",\"Cost\":4,\"IsSpell\":true,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":14,\"CardId\":\"CS2_025\",\"Cost\":2,\"IsSpell\":true,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":21,\"CardId\":\"CS2_122\",\"Cost\":3,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":11,\"CardId\":\"EX1_015\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":33,\"CardId\":\"EX1_277\",\"Cost\":1,\"IsSpell\":true,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]},\"Opponent\":{\"Resources\":0,\"PowerAvailable\":false,\"Hero\":{\"RockId\":66,\"Class\":1,\"Damage\":0,\"Health\":30,\"CanAttack\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":67,\"CardId\":\"CS2_056\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[{\"RockId\":44,\"Damage\":1,\"Health\":3,\"BaseHealth\":3,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":true,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":69,\"Damage\":1,\"Health\":1,\"BaseHealth\":1,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false},{\"RockId\":71,\"Damage\":1,\"Health\":1,\"BaseHealth\":1,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false}],\"Cards\":[{\"RockId\":37,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]}}";
            var scene = RockJsonSerializer.Deserialize<RockScene>(json);
            RockBot robot = new RockBot();
            var action = robot.GetAction(scene);

            Assert.IsNotNull(action);
            Assert.IsTrue(IsValidAction(action, scene));
        }

        [TestMethod]
        public void ScenarioTest4()
        {
            string json = "{\"Self\":{\"Resources\":0,\"PowerAvailable\":false,\"Hero\":{\"RockId\":64,\"Class\":1,\"Damage\":2,\"Health\":32,\"CanAttack\":true,\"IsExhausted\":true,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":65,\"CardId\":\"CS2_017\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[],\"Cards\":[{\"RockId\":29,\"CardId\":\"CS2_119\",\"Cost\":4,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":13,\"CardId\":\"CS2_182\",\"Cost\":4,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":33,\"CardId\":\"CS2_162\",\"Cost\":6,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":true,\"HasCharge\":false},{\"RockId\":21,\"CardId\":\"CS2_127\",\"Cost\":3,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":true,\"HasCharge\":false},{\"RockId\":68,\"CardId\":\"GAME_005\",\"Cost\":0,\"IsSpell\":true,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]},\"Opponent\":{\"Resources\":1,\"PowerAvailable\":false,\"Hero\":{\"RockId\":66,\"Class\":2,\"Damage\":0,\"Health\":28,\"CanAttack\":true,\"IsExhausted\":false,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":67,\"CardId\":\"DS1h_292\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[],\"Cards\":[{\"RockId\":43,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":58,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":41,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":61,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]}}";
            var scene = RockJsonSerializer.Deserialize<RockScene>(json);
            RockBot robot = new RockBot();
            var action = robot.GetAction(scene);

            Assert.IsNull(action);
        }

        [TestMethod]
        public void ScenarioTest5()
        {
            string json = "{\"Self\":{\"Resources\":2,\"PowerAvailable\":false,\"Hero\":{\"RockId\":64,\"Class\":1,\"Damage\":0,\"Health\":30,\"CanAttack\":true,\"IsExhausted\":false,\"HasWeapon\":false,\"WeaponRockId\":0,\"WeaponCanAttack\":false},\"Power\":{\"RockId\":65,\"CardId\":\"CS2_017\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[{\"RockId\":23,\"Damage\":2,\"Health\":7,\"BaseHealth\":7,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false}],\"Cards\":[{\"RockId\":10,\"CardId\":\"CS2_201\",\"Cost\":7,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":31,\"CardId\":\"CS2_200\",\"Cost\":6,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":true,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":32,\"CardId\":\"CS2_009\",\"Cost\":2,\"IsSpell\":true,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]},\"Opponent\":{\"Resources\":0,\"PowerAvailable\":false,\"Hero\":{\"RockId\":66,\"Class\":5,\"Damage\":0,\"Health\":26,\"CanAttack\":true,\"IsExhausted\":true,\"HasWeapon\":true,\"WeaponRockId\":71,\"WeaponCanAttack\":true},\"Power\":{\"RockId\":67,\"CardId\":\"CS2_083b\",\"Cost\":2,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},\"Minions\":[{\"RockId\":47,\"Damage\":2,\"Health\":4,\"BaseHealth\":4,\"IsFrozen\":false,\"IsExhausted\":true,\"IsAsleep\":false,\"IsStealthed\":false,\"CanAttack\":true,\"CanBeAttacked\":true,\"HasTaunt\":false,\"HasWindfury\":false,\"HasDivineShield\":false}],\"Cards\":[{\"RockId\":51,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":45,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":52,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false},{\"RockId\":56,\"CardId\":\"\",\"Cost\":0,\"IsSpell\":false,\"IsWeapon\":false,\"IsMinion\":false,\"HasTaunt\":false,\"HasCharge\":false}]}}";
            var scene = RockJsonSerializer.Deserialize<RockScene>(json);
            RockBot robot = new RockBot();
            var action = robot.GetAction(scene);

            Assert.IsNotNull(action);
            Assert.IsTrue(IsValidAction(action, scene));
        }


        private static bool IsValidAction(RockAction action, RockScene scene)
        {
            if (action.Source == 0)
            {
                return false;
            }

            foreach (var target in action.Targets)
            {
                if (target == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
