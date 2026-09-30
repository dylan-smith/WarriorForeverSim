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
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;
            state.Config.PlayerSettings.Level = 70;
            state.Config.BossSettings.Level = 73;

            Assert.AreEqual(0.0, MeleeCritCalculator.Calculate(state), 0.000001);
        }

        [TestMethod]
        public void CritCalculatorSuppressionAmount()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Level = 70;
            state.Config.BossSettings.Level = 73;

            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(Constants.AGI_FOR_ZERO_CRIT));
            Assert.AreEqual(0.0, MeleeCritCalculator.Calculate(state));

            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(Constants.AGI_FOR_ZERO_CRIT + 1));
            Assert.IsTrue(MeleeCritCalculator.Calculate(state) > 0.0);
        }

        [TestMethod]
        public void CritCalculatorAgilityToCrit()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Level = 70;
            state.Config.BossSettings.Level = 73;

            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(Constants.AGI_FOR_ZERO_CRIT + 80));

            // https://tbc.wowhead.com/guides/classic-the-burning-crusade-stats-overview
            Assert.AreEqual(0.02, MeleeCritCalculator.Calculate(state), 0.00001);
        }

        [TestMethod]
        public void CritCalculatorRatingToCrit()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Level = 70;
            state.Config.BossSettings.Level = 73;

            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), new FakeStatCalculator(Constants.AGI_FOR_ZERO_CRIT));

            state.Config.Gear.Head = new GearItem() { CritRating = 44.16 };

            // https://tbc.wowhead.com/guides/classic-the-burning-crusade-stats-overview
            Assert.AreEqual(0.02, MeleeCritCalculator.Calculate(state), 0.00001);
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
            // subtracting out the crit suppression that the sim includes but the stat page in game doesn't
            Assert.AreEqual(0.0, MeleeCritCalculator.Calculate(state), 0.0001);
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
            // subtracting out the crit suppression that the sim includes but the stat page in game doesn't
            Assert.AreEqual(0.0, MeleeCritCalculator.Calculate(state), 0.0001);
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
            // in-game it says 2.17% but the sim subtracts 1.8% for crit suppression aura
            Assert.AreEqual(0.0, MeleeCritCalculator.Calculate(state), 0.000001);
            Assert.AreEqual(120, StrengthCalculator.Calculate(state));
            Assert.AreEqual(80, AgilityCalculator.Calculate(state));
        }

        // TODO: Tests for all calculators that need to convert between rating and %
        // TODO: Should probably have a test for each calculator for base stats
        // TODO: Test for enchants
    }
}
