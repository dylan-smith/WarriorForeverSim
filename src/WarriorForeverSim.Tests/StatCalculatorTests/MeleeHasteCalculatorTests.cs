using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class MeleeHasteCalculatorTests
    {
        [TestMethod]
        public void RangedHasteCalculatorRatingToPercent()
        {
            var state = new SimulationState();

            state.Config.Gear.Head = new GearItem() { HasteRating = 300 };

            // https://tbc.wowhead.com/guides/classic-the-burning-crusade-stats-overview
            Assert.AreEqual(1.1899, MeleeHasteCalculator.Calculate(state), 0.0001);
        }
    }
}
