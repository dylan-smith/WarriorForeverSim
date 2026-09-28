using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class BaseStatsTests
    {
        [TestMethod]
        public void Dwarf()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Dwarf;

            Assert.AreEqual(66, StrengthCalculator.Calculate(state));
            Assert.AreEqual(147, AgilityCalculator.Calculate(state));
        }

        [TestMethod]
        public void NightElf()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.NightElf;

            Assert.AreEqual(61, StrengthCalculator.Calculate(state));
            Assert.AreEqual(156, AgilityCalculator.Calculate(state));
        }

        [TestMethod]
        public void Orc()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Orc;

            Assert.AreEqual(67, StrengthCalculator.Calculate(state));
            Assert.AreEqual(148, AgilityCalculator.Calculate(state));
        }

        [TestMethod]
        public void Tauren()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Tauren;

            Assert.AreEqual(69, StrengthCalculator.Calculate(state));
            Assert.AreEqual(146, AgilityCalculator.Calculate(state));
        }

        [TestMethod]
        public void Troll()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Troll;

            Assert.AreEqual(65, StrengthCalculator.Calculate(state));
            Assert.AreEqual(153, AgilityCalculator.Calculate(state));
        }

        // TODO: Other Races

        [TestMethod]
        public void NoRace()
        {
            var state = new SimulationState();

            Assert.ThrowsExactly<Exception>(() => AgilityCalculator.Calculate(state));
        }

        // TODO: Tests for the other stat calculator "base" values
    }
}
