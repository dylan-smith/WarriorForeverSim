using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class StatCalculatorTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        [TestMethod]
        public void StrengthCalculatorBaseStats()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;

            Assert.AreEqual(Constants.HUMAN_STR, StrengthCalculator.Calculate(state));
        }

        [TestMethod]
        public void CritCalculatorBaseStats()
        {
            var state = CreateCritState(bossLevel: 60);

            // 80 base agility / 20 agility per 1% crit
            Assert.AreEqual(0.04, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorAgilityToCrit()
        {
            var state = CreateCritState(bossLevel: 60);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(40));

            Assert.AreEqual(0.02, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorGearCritIsPercent()
        {
            var state = CreateCritState(bossLevel: 60);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(0));

            state.Config.Gear.Head = new GearItem() { CritRating = 2 };

            Assert.AreEqual(0.02, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponSkillBelowDefense()
        {
            var state = CreateCritState(bossLevel: 62);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            // 10% agility crit, 300 skill vs 310 defense = -2%
            Assert.AreEqual(0.08, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponSkillAboveLevelCapIgnored()
        {
            var state = CreateCritState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(305));

            // only 300 skill counts toward crit: 10% - 3%
            Assert.AreEqual(0.07, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);

            state.Config.BossSettings.Level = 60;
            Assert.AreEqual(0.10, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponSkillAboveDefense()
        {
            var state = CreateCritState(bossLevel: 58);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            // 300 skill vs 290 defense = +0.4%
            Assert.AreEqual(0.104, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorAuraSuppressionRemovesSmallAuraCritEntirely()
        {
            var state = CreateCritState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            state.Config.Gear.Head = new GearItem() { CritRating = 1 };

            // 10% agility + 1% aura - 3% weapon skill - 1% (all of the aura crit)
            Assert.AreEqual(0.07, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorAuraSuppressionCapsAtOnePointEightPercent()
        {
            var state = CreateCritState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            state.Config.Talents[Talent.Cruelty] = 5;

            // 10% agility + 5% aura - 3% weapon skill - 1.8%
            Assert.AreEqual(0.102, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorNoAuraSuppressionBelowThreeLevels()
        {
            var state = CreateCritState(bossLevel: 62);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(200));

            state.Config.Talents[Talent.Cruelty] = 5;

            // 10% agility + 5% aura - 2% weapon skill
            Assert.AreEqual(0.13, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorWeaponmasterOnlyForAxesAndPolearms()
        {
            var state = CreateCritState(bossLevel: 60);
            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(0));

            state.Config.Talents[Talent.Weaponmaster] = 5;

            Assert.AreEqual(0.0, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
            Assert.AreEqual(0.05, MeleeCritCalculator.Calculate(new GearItem() { WeaponType = WeaponType.OneHandedAxe }, state), 0.000001);
            Assert.AreEqual(0.05, MeleeCritCalculator.Calculate(new GearItem() { WeaponType = WeaponType.TwoHandedAxe }, state), 0.000001);
            Assert.AreEqual(0.05, MeleeCritCalculator.Calculate(new GearItem() { WeaponType = WeaponType.Polearm }, state), 0.000001);
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

        [TestMethod]
        public void RangedHasteCalculatorRatingToPercent()
        {
            var state = new SimulationState();

            state.Config.Gear.Head = new GearItem() { HasteRating = 300 };

            // https://tbc.wowhead.com/guides/classic-the-burning-crusade-stats-overview
            Assert.AreEqual(1.1899, MeleeHasteCalculator.Calculate(state), 0.0001);
        }

        // Strength - Sixx has a bug with improved strength of earth totem
        // Agility - Sixx doesn't round down aggressively enough, difference of 1
        // Sixx also doesn't appear to take into account the talent Survival Instincts anywhere

        [TestMethod]
        public void DefaultConfigNoBuffs()
        {
            var state = new SimulationState
            {
                Config = new DefaultConfig()
            };

            state.Config.Buffs.Clear();

            state.Validate();

            Assert.AreEqual(289, StrengthCalculator.Calculate(state));
            Assert.AreEqual(205, AgilityCalculator.Calculate(state));
            Assert.AreEqual(722, MeleeAttackPowerCalculator.Calculate(state));
            // 205 agility / 20 + 4% gear + 5% Cruelty - 3% weapon skill - 1.8% aura suppression
            Assert.AreEqual(0.1445, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.0001);
        }

        [TestMethod]
        public void DefaultConfigNoBuffsNoTalents()
        {
            var state = new SimulationState
            {
                Config = new DefaultConfig()
            };

            state.Config.Buffs.Clear();
            state.Config.Talents.Clear();

            state.Validate();

            Assert.AreEqual(289, StrengthCalculator.Calculate(state));
            Assert.AreEqual(205, AgilityCalculator.Calculate(state));
            Assert.AreEqual(722, MeleeAttackPowerCalculator.Calculate(state));
            // 205 agility / 20 + 4% gear - 3% weapon skill - 1.8% aura suppression
            Assert.AreEqual(0.0945, MeleeCritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.0001);
        }

        [TestMethod]
        public void NoGearNoBuffsNoTalents()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;
            state.Config.PlayerSettings.Level = 60;
            // intentionally setting boss to 60 to avoid the 3% crit suppression on raid bosses
            state.Config.BossSettings.Level = 60;

            // numbers taken from in game stat page with no gear and no talents
            Assert.AreEqual(320, MeleeAttackPowerCalculator.Calculate(state));
            // 80 agility / 20
            Assert.AreEqual(0.04, CritCalculator.Calculate(new GearItem(), state), 0.000001);
            Assert.AreEqual(120, StrengthCalculator.Calculate(state));
            Assert.AreEqual(80, AgilityCalculator.Calculate(state));
        }

        // TODO: Tests for all calculators that need to convert between rating and %
        // TODO: Should probably have a test for each calculator for base stats
        // TODO: Test for enchants
    }
}
