using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;

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
            //
            // Base crit is 25.29%; Master Tactician adds 10% while active.
            // Base hit damage is 737.19; Expose Weakness raises it to 778.41 while active.
            var expected = new[]
            {
                new DamageEvent(0.4348, 1518.62, DamageType.Crit, 0, 0.2529, 0.7471),
                new DamageEvent(2.9565, 778.41, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(5.4783, 778.41, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(8.0000, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(10.5217, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(13.0435, 737.19, DamageType.Hit, 0, 0.3529, 0.6471),
                new DamageEvent(15.5652, 737.19, DamageType.Hit, 0, 0.3529, 0.6471),
                new DamageEvent(18.0870, 1518.62, DamageType.Crit, 0, 0.3529, 0.6471),
                new DamageEvent(20.6087, 1603.53, DamageType.Crit, 0, 0.2529, 0.7471),
                new DamageEvent(23.1304, 778.41, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(25.6522, 1603.53, DamageType.Crit, 0, 0.2529, 0.7471),
                new DamageEvent(28.1739, 1603.53, DamageType.Crit, 0, 0.3529, 0.6471),
                new DamageEvent(30.6957, 778.41, DamageType.Hit, 0, 0.3529, 0.6471),
                new DamageEvent(33.2174, 778.41, DamageType.Hit, 0, 0.3529, 0.6471),
                new DamageEvent(35.7391, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(38.2609, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(40.7826, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(43.3043, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(45.8261, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(48.3478, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(50.8696, 737.19, DamageType.Hit, 0, 0.2529, 0.7471),
                new DamageEvent(53.3913, 1518.62, DamageType.Crit, 0, 0.3529, 0.6471),
                new DamageEvent(55.9130, 778.41, DamageType.Hit, 0, 0.3529, 0.6471),
                new DamageEvent(58.4348, 778.41, DamageType.Hit, 0, 0.3529, 0.6471),
            };

            Assert.AreEqual(22924.47, totalDamage, 0.01);
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
