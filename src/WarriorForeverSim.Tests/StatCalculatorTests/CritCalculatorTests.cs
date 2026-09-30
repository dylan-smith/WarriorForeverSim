using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class CritCalculatorTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        [TestMethod]
        public void CritCalculatorBaseStats()
        {
            var state = CreateCritState(bossLevel: 60);

            // 80 base agility / 20 agility per 1% crit
            Assert.AreEqual(0.04, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorAgilityToCrit()
        {
            var state = CreateCritState(bossLevel: 60);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(40));

            Assert.AreEqual(0.02, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorGearCritIsPercent()
        {
            var state = CreateCritState(bossLevel: 60);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(0));

            state.Config.Gear.Head = new GearItem() { CritRating = 2 };

            Assert.AreEqual(0.02, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponSkillBelowDefense()
        {
            var state = CreateCritState(bossLevel: 62);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            // 10% agility crit, 300 skill vs 310 defense = -2%
            Assert.AreEqual(0.08, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponSkillAboveLevelCapIgnored()
        {
            var state = CreateCritState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(305));

            // only 300 skill counts toward crit: 10% - 3%
            Assert.AreEqual(0.07, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);

            state.Config.BossSettings.Level = 60;
            Assert.AreEqual(0.10, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponSkillAboveDefense()
        {
            var state = CreateCritState(bossLevel: 58);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            // 300 skill vs 290 defense = +0.4%
            Assert.AreEqual(0.104, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorAuraSuppressionRemovesSmallAuraCritEntirely()
        {
            var state = CreateCritState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            state.Config.Gear.Head = new GearItem() { CritRating = 1 };

            // 10% agility + 1% aura - 3% weapon skill - 1% (all of the aura crit)
            Assert.AreEqual(0.07, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorAuraSuppressionCapsAtOnePointEightPercent()
        {
            var state = CreateCritState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            state.Config.Talents[Talent.Cruelty] = 5;

            // 10% agility + 5% aura - 3% weapon skill - 1.8%
            Assert.AreEqual(0.102, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorNoAuraSuppressionBelowThreeLevels()
        {
            var state = CreateCritState(bossLevel: 62);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            state.Config.Talents[Talent.Cruelty] = 5;

            // 10% agility + 5% aura - 2% weapon skill
            Assert.AreEqual(0.13, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponmasterOnlyForAxesAndPolearms()
        {
            var state = CreateCritState(bossLevel: 60);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(0));

            state.Config.Talents[Talent.Weaponmaster] = 5;

            Assert.AreEqual(0.0, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
            Assert.AreEqual(0.05, CritCalculator.Calculate(new GearItem() { WeaponType = WeaponType.OneHandedAxe }, state), 0.000001);
            Assert.AreEqual(0.05, CritCalculator.Calculate(new GearItem() { WeaponType = WeaponType.TwoHandedAxe }, state), 0.000001);
            Assert.AreEqual(0.05, CritCalculator.Calculate(new GearItem() { WeaponType = WeaponType.Polearm }, state), 0.000001);
        }

        private static SimulationState CreateCritState(int bossLevel)
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
