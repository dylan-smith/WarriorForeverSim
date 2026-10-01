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
            state.ProcessedEvents.Add(new DamageEvent(0.0, 150.4, DamageType.Hit, new AttackTable(0.1, 0, 0, 0.2)));
            state.ProcessedEvents.Add(new SwingTimerCompletedEvent(1.9));
            state.ProcessedEvents.Add(new DamageEvent(1.9, 0.0, DamageType.Miss, new AttackTable(0.1, 0, 0, 0.2)));
            state.ProcessedEvents.Add(new DamageEvent(3.8, 300.0, DamageType.Crit, new AttackTable(0.1, 0, 0, 0.2)));

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
        public void DescribesDodgeAndGlancingBlows()
        {
            var state = new SimulationState();
            state.ProcessedEvents.Add(new DamageEvent(0.0, 0.0, DamageType.Dodge, new AttackTable(0.08, 0.065, 0.4, 0.2)));
            state.ProcessedEvents.Add(new DamageEvent(1.9, 97.4, DamageType.Glancing, new AttackTable(0.08, 0.065, 0.4, 0.2)));

            var log = SimulationLogEntry.FromState(state);

            Assert.AreEqual("Dodge", log[0].Description);
            Assert.AreEqual(0.065, log[0].DodgeChance);
            Assert.AreEqual(0.4, log[0].GlancingChance);
            Assert.AreEqual("Glancing for 97", log[1].Description);
            Assert.AreEqual(97.4, log[1].TotalDamage, 0.001);
        }

        [TestMethod]
        public void IncludesDetailsRollsAndAuras()
        {
            var state = new SimulationState();
            var damage = new DamageEvent(0.0, 100.0, DamageType.Hit, new AttackTable(0.1, 0, 0, 0.2))
            {
                AttackRoll = 0.5,
                ActiveAuras = [Aura.SwingTimerCooldown],
            };
            damage.AddDetail("Damage", "Final damage", "100.0");
            state.ProcessedEvents.Add(damage);

            var entry = SimulationLogEntry.FromState(state).Single();

            Assert.AreEqual(0.5, entry.AttackRoll);
            Assert.HasCount(1, entry.Details);
            Assert.AreEqual("Damage", entry.Details[0].Section);
            Assert.AreEqual("Final damage", entry.Details[0].Label);
            Assert.AreEqual("100.0", entry.Details[0].Value);
            CollectionAssert.AreEqual(new[] { Aura.SwingTimerCooldown }, entry.ActiveAuras.ToArray());
        }

        [TestMethod]
        public void PublishedEventsSnapshotActiveAuras()
        {
            RandomGenerator.Seed(64852147);

            var state = new Simulation(new DefaultConfig()).Run();
            var log = SimulationLogEntry.FromState(state);

            // A swing puts the swing timer on cooldown; its completion clears it.
            Assert.Contains(Aura.SwingTimerCooldown, log.First(e => e.Event == nameof(AutoAttackSwingEvent)).ActiveAuras);
            Assert.DoesNotContain(Aura.SwingTimerCooldown, log.First(e => e.Event == nameof(SwingTimerCompletedEvent)).ActiveAuras);
            Assert.IsNotEmpty(log.First(e => e.Event == nameof(DamageEvent)).Details);
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
