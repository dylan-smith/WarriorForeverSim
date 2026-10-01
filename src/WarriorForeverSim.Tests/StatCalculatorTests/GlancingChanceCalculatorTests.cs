using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class GlancingChanceCalculatorTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        [TestMethod]
        [DataRow(60, 0.10)]
        [DataRow(61, 0.20)]
        [DataRow(62, 0.30)]
        [DataRow(63, 0.40)]
        public void GlancingChanceByLevelDifference(int bossLevel, double expected)
        {
            var state = CreateGlancingState(bossLevel);

            Assert.AreEqual(expected, GlancingChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void GlancingChanceIgnoresWeaponSkillAboveLevelCap()
        {
            var state = CreateGlancingState(bossLevel: 63);
            state.Config.Gear.Hands = new GearItem() { WeaponSkill = { [WeaponType.OneHandedSword] = 5 } };

            // weapon skill is capped at 60 * 5 = 300, so still 10% + 15 * 2%
            Assert.AreEqual(0.40, GlancingChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void GlancingChanceUsesWeaponSkillBelowLevelCap()
        {
            var state = CreateGlancingState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(295));

            // 295 skill vs 315 defense: 10% + 20 * 2%
            Assert.AreEqual(0.50, GlancingChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void GlancingChanceNeverBelowZero()
        {
            var state = CreateGlancingState(bossLevel: 55);

            // 300 skill vs 275 defense: 10% - 25 * 2%
            Assert.AreEqual(0.0, GlancingChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        private static SimulationState CreateGlancingState(int bossLevel)
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
