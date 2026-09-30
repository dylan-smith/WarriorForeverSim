using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class StrengthCalculatorTests
    {
        [TestMethod]
        public void StrengthCalculatorBaseStats()
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;

            Assert.AreEqual(Constants.HUMAN_STR, StrengthCalculator.Calculate(state));
        }
    }
}
