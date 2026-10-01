using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class SimulationReportTests
    {
        [TestCleanup]
        public void TestCleanup() => RandomGenerator.ClearMock();

        [TestMethod]
        public void SumsDamageAndCountsResults()
        {
            var state = new SimulationState();
            state.Config.SimulationSettings.FightLength = 10.0;
            state.ProcessedEvents.Add(new DamageEvent(0.0, 100.0, DamageType.Hit, new AttackTable(0, 0, 0, 0)));
            state.ProcessedEvents.Add(new DamageEvent(2.0, 200.0, DamageType.Crit, new AttackTable(0, 0, 0, 1)));
            state.ProcessedEvents.Add(new DamageEvent(4.0, 0.0, DamageType.Miss, new AttackTable(1, 0, 0, 0)));
            state.ProcessedEvents.Add(new DamageEvent(6.0, 50.0, DamageType.Hit, new AttackTable(0, 0, 0, 0)));
            state.ProcessedEvents.Add(new DamageEvent(8.0, 0.0, DamageType.Dodge, new AttackTable(0, 1, 0, 0)));
            state.ProcessedEvents.Add(new DamageEvent(10.0, 65.0, DamageType.Glancing, new AttackTable(0, 0, 1, 0)));
            state.Warnings.Add("warning");

            var report = SimulationReport.FromState(state);

            Assert.AreEqual(415.0, report.TotalDamage, 0.001);
            Assert.AreEqual(41.5, report.Dps, 0.001);
            Assert.AreEqual(2, report.Hits);
            Assert.AreEqual(1, report.Crits);
            Assert.AreEqual(1, report.Misses);
            Assert.AreEqual(1, report.Dodges);
            Assert.AreEqual(1, report.Glancings);
            Assert.HasCount(1, report.Warnings);
            Assert.IsEmpty(report.Errors);
        }

        [TestMethod]
        public void ZeroFightLengthGivesZeroDps()
        {
            var state = new SimulationState();
            state.ProcessedEvents.Add(new DamageEvent(0.0, 100.0, DamageType.Hit, new AttackTable(0, 0, 0, 0)));

            var report = SimulationReport.FromState(state);

            Assert.AreEqual(0.0, report.Dps);
        }

        [TestMethod]
        public void DefaultConfigProducesDps()
        {
            RandomGenerator.Seed(64852147);

            var state = new Simulation(new DefaultConfig()).Run();
            var report = SimulationReport.FromState(state);

            Assert.IsEmpty(report.Errors);
            Assert.IsGreaterThan(0.0, report.Dps);
        }
    }
}
