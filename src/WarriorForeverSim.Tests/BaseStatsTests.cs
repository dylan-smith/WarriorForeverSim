using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class BaseStatsTests
    {
        [TestMethod]
        public void Human()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;

            Assert.AreEqual(120, StrengthCalculator.Calculate(state));
            Assert.AreEqual(80, AgilityCalculator.Calculate(state));
            Assert.AreEqual(110, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(30, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void Dwarf()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Dwarf;

            Assert.AreEqual(122, StrengthCalculator.Calculate(state));
            Assert.AreEqual(76, AgilityCalculator.Calculate(state));
            Assert.AreEqual(113, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(29, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void Gnome()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Gnome;

            Assert.AreEqual(115, StrengthCalculator.Calculate(state));
            Assert.AreEqual(83, AgilityCalculator.Calculate(state));
            Assert.AreEqual(109, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(35, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void NightElf()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.NightElf;

            Assert.AreEqual(117, StrengthCalculator.Calculate(state));
            Assert.AreEqual(85, AgilityCalculator.Calculate(state));
            Assert.AreEqual(109, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(30, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void Orc()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Orc;

            Assert.AreEqual(123, StrengthCalculator.Calculate(state));
            Assert.AreEqual(77, AgilityCalculator.Calculate(state));
            Assert.AreEqual(112, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(27, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void Tauren()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Tauren;

            Assert.AreEqual(125, StrengthCalculator.Calculate(state));
            Assert.AreEqual(75, AgilityCalculator.Calculate(state));
            Assert.AreEqual(112, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(25, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void Troll()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Troll;

            Assert.AreEqual(121, StrengthCalculator.Calculate(state));
            Assert.AreEqual(82, AgilityCalculator.Calculate(state));
            Assert.AreEqual(111, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(26, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void Undead()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Undead;

            Assert.AreEqual(119, StrengthCalculator.Calculate(state));
            Assert.AreEqual(78, AgilityCalculator.Calculate(state));
            Assert.AreEqual(111, state.Config.PlayerSettings.Stamina);
            Assert.AreEqual(28, state.Config.PlayerSettings.Intellect);
        }

        [TestMethod]
        public void NoRace()
        {
            var state = new SimulationState();

            Assert.ThrowsExactly<Exception>(() => AgilityCalculator.Calculate(state));
        }

        // TODO: Tests for the other stat calculator "base" values
    }
}
