using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class SimulationLogEntryTests
    {
        [TestCleanup]
        public void TestCleanup() => RandomGenerator.ClearMock();

        [TestMethod]
        public void BuildsLogInOrderWithRunningTotal()
        {
            var state = new SimulationState();
            state.ProcessedEvents.Add(new AutoAttackSwingEvent(0.0));
            state.ProcessedEvents.Add(new DamageEvent(0.0, 150.4, DamageType.Hit, 0.1, 0.2, 0.7));
            state.ProcessedEvents.Add(new SwingTimerCompletedEvent(1.9));
            state.ProcessedEvents.Add(new DamageEvent(1.9, 0.0, DamageType.Miss, 0.1, 0.2, 0.7));
            state.ProcessedEvents.Add(new DamageEvent(3.8, 300.0, DamageType.Crit, 0.1, 0.2, 0.7));

            var log = SimulationLogEntry.FromState(state);

            Assert.HasCount(5, log);

            Assert.AreEqual(nameof(AutoAttackSwingEvent), log[0].Event);
            Assert.AreEqual("Main-hand swing", log[0].Description);
            Assert.IsNull(log[0].Damage);
            Assert.IsNull(log[0].DamageType);
            Assert.AreEqual(0.0, log[0].TotalDamage);

            Assert.AreEqual("Hit for 150", log[1].Description);
            Assert.AreEqual(150.4, log[1].Damage);
            Assert.AreEqual(DamageType.Hit, log[1].DamageType);
            Assert.AreEqual(0.1, log[1].MissChance);
            Assert.AreEqual(0.2, log[1].CritChance);

            Assert.AreEqual("Swing timer ready", log[2].Description);
            Assert.AreEqual(1.9, log[2].Timestamp);
            Assert.AreEqual(150.4, log[2].TotalDamage, 0.001);

            Assert.AreEqual("Miss", log[3].Description);
            Assert.AreEqual(150.4, log[3].TotalDamage, 0.001);

            Assert.AreEqual("Crit for 300", log[4].Description);
            Assert.AreEqual(450.4, log[4].TotalDamage, 0.001);
        }

        [TestMethod]
        public void MatchesReportForDefaultConfig()
        {
            RandomGenerator.Seed(64852147);

            var state = new Simulation(new DefaultConfig()).Run();
            var log = SimulationLogEntry.FromState(state);
            var report = SimulationReport.FromState(state);

            Assert.HasCount(state.ProcessedEvents.Count, log);
            Assert.AreEqual(report.TotalDamage, log.Last().TotalDamage, 0.001);
        }
    }
}
