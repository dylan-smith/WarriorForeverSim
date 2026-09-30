using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class CombinedStatCalculatorTests
    {
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
            Assert.AreEqual(0.1445, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.0001);
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
            Assert.AreEqual(0.0945, CritCalculator.Calculate(state.Config.Gear.MainHand, state), 0.0001);
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
