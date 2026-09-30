using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class MissChanceCalculatorTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        [TestMethod]
        public void MissChanceCalculatorBaseMissSameLevel()
        {
            var state = CreateMissState(bossLevel: 60);

            Assert.AreEqual(0.05, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorBaseMissDualWielding()
        {
            var state = CreateMissState(bossLevel: 60);
            state.Config.Gear.OffHand = new GearItem() { WeaponType = WeaponType.OneHandedSword };

            Assert.AreEqual(0.24, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorSkillDifferenceOfTen()
        {
            var state = CreateMissState(bossLevel: 62);

            // 300 skill vs 310 defense: 5% + 10 * 0.1%
            Assert.AreEqual(0.06, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorSkillDifferenceOverTen()
        {
            var state = CreateMissState(bossLevel: 63);

            // 300 skill vs 315 defense: 5% + 15 * 0.2%
            Assert.AreEqual(0.08, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorDualWieldingVsBoss()
        {
            var state = CreateMissState(bossLevel: 63);
            state.Config.Gear.OffHand = new GearItem() { WeaponType = WeaponType.OneHandedSword };

            // 24% + 15 * 0.2%
            Assert.AreEqual(0.27, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorHitIsPercent()
        {
            var state = CreateMissState(bossLevel: 60);
            state.Config.Gear.Head = new GearItem() { HitRating = 3 };

            Assert.AreEqual(0.02, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorHitSumsAcrossGear()
        {
            var state = CreateMissState(bossLevel: 60);
            state.Config.Gear.Head = new GearItem() { HitRating = 2 };
            state.Config.Gear.Waist = new GearItem() { HitRating = 1 };

            Assert.AreEqual(0.02, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorFirstPercentOfHitIgnoredWhenSkillGapOverTen()
        {
            var state = CreateMissState(bossLevel: 63);
            state.Config.Gear.Head = new GearItem() { HitRating = 7 };

            // 8% - (7% - 1%)
            Assert.AreEqual(0.02, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorHitNotIgnoredWhenSkillGapIsTen()
        {
            var state = CreateMissState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(305));
            state.Config.Gear.Head = new GearItem() { HitRating = 5 };

            // 305 skill vs 315 defense: 5% + 10 * 0.1% - 5%
            Assert.AreEqual(0.01, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorNeverBelowZero()
        {
            var state = CreateMissState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(305));
            state.Config.Gear.Head = new GearItem() { HitRating = 7 };

            // 6% - 7%
            Assert.AreEqual(0.0, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void MissChanceCalculatorUsesWeaponSkillForWeaponType()
        {
            var state = CreateMissState(bossLevel: 63);
            state.Config.Gear.Hands = new GearItem() { WeaponSkill = { [WeaponType.OneHandedSword] = 5 } };

            // 305 sword skill vs 315 defense: 5% + 10 * 0.1%
            Assert.AreEqual(0.06, MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
            // no bonus for maces: 5% + 15 * 0.2%
            Assert.AreEqual(0.08, MissChanceCalculator.Calculate(new GearItem() { WeaponType = WeaponType.OneHandedMace }, state), 0.000001);
        }

        private static SimulationState CreateMissState(int bossLevel)
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
