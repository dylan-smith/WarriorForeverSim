using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class DodgeChanceCalculatorTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        [TestMethod]
        public void DodgeChanceSameLevel()
        {
            var state = CreateDodgeState(bossLevel: 60);

            Assert.AreEqual(0.05, DodgeChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void DodgeChanceVsBoss()
        {
            var state = CreateDodgeState(bossLevel: 63);

            // 300 skill vs 315 defense: 5% + 15 * 0.1%
            Assert.AreEqual(0.065, DodgeChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void DodgeChanceVsBossWithWeaponSkill()
        {
            var state = CreateDodgeState(bossLevel: 63);
            state.Config.Gear.Hands = new GearItem() { WeaponSkill = { [WeaponType.OneHandedSword] = 5 } };

            // 305 skill vs 315 defense: 5% + 10 * 0.1%
            Assert.AreEqual(0.06, DodgeChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void DodgeChanceUsesWeaponSkillAboveLevelCap()
        {
            var state = CreateDodgeState(bossLevel: 60);
            state.Config.Gear.Hands = new GearItem() { WeaponSkill = { [WeaponType.OneHandedSword] = 10 } };

            // 310 skill vs 300 defense: 5% - 10 * 0.1%
            Assert.AreEqual(0.04, DodgeChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void DodgeChanceNeverBelowZero()
        {
            var state = CreateDodgeState(bossLevel: 40);

            // 300 skill vs 200 defense: 5% - 100 * 0.1%
            Assert.AreEqual(0.0, DodgeChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void DodgeChanceUnaffectedByDualWielding()
        {
            var state = CreateDodgeState(bossLevel: 63);
            state.Config.Gear.OffHand = new GearItem() { WeaponType = WeaponType.OneHandedSword };

            Assert.AreEqual(0.065, DodgeChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        private static SimulationState CreateDodgeState(int bossLevel)
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;
            state.Config.PlayerSettings.Level = 60;
            state.Config.BossSettings.Level = bossLevel;
            state.Config.Gear.MainHand = new GearItem() { WeaponType = WeaponType.OneHandedSword };

            return state;
        }
    }
}
