using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class SimulationTests
    {
        [TestMethod]
        public void AutoShotRotation()
        {
            var config = new DefaultConfig();
            var sim = new Simulation(config);

            RandomGenerator.Seed(64852147);

            var result = sim.Run();
            var totalDamage = result.DamageEvents.Sum(x => x.Damage);

            foreach (var e in result.ProcessedEvents)
            {
                Debug.WriteLine(e);
            }

            // Golden output for DefaultConfig with the seed above. Regenerate these
            // values whenever DefaultConfig or the damage calculators intentionally change.
            var expected = new[]
            {
                new DamageEvent(0.0000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(1.9000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(3.8000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(5.7000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(7.6000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(9.5000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(11.4000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(13.3000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(15.2000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(17.1000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(19.0000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(20.9000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(22.8000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(24.7000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(26.6000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(28.5000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(30.4000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(32.3000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(34.2000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(36.1000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(38.0000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(39.9000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(41.8000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(43.7000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(45.6000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(47.5000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(49.4000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(51.3000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(53.2000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(55.1000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(57.0000, 177.49, DamageType.Hit, 0, 0, 1),
                new DamageEvent(58.9000, 177.49, DamageType.Hit, 0, 0, 1),
            };

            Assert.AreEqual(5679.54, totalDamage, 0.01);
            Assert.AreEqual(expected.Length, result.DamageEvents.Count());

            var actual = result.DamageEvents.ToList();

            for (var i = 0; i < expected.Length; i++)
            {
                AssertDamageEvent(expected[i], actual[i], i);
            }
        }

        private void AssertDamageEvent(DamageEvent expected, DamageEvent actual, int index)
        {
            Assert.AreEqual(expected.Damage, actual.Damage, 0.01, $"Damage mismatch at event {index}");
            Assert.AreEqual(expected.Timestamp, actual.Timestamp, 0.01, $"Timestamp mismatch at event {index}");
            Assert.AreEqual(expected.DamageType, actual.DamageType, $"DamageType mismatch at event {index}");
            Assert.AreEqual(expected.CritChance, actual.CritChance, 0.001, $"CritChance mismatch at event {index}");
            Assert.AreEqual(expected.HitChance, actual.HitChance, 0.001, $"HitChance mismatch at event {index}");
            Assert.AreEqual(expected.MissChance, actual.MissChance, 0.001, $"MissChance mismatch at event {index}");
        }
    }
}
