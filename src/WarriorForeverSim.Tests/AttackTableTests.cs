using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests
{
    [TestClass]
    public class AttackTableTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        [TestMethod]
        public void SegmentsAreConsecutive()
        {
            var table = new AttackTable(0.08, 0.065, 0.4, 0.2);

            Assert.AreEqual(0.08, table.MissChance, 0.000001);
            Assert.AreEqual(0.065, table.DodgeChance, 0.000001);
            Assert.AreEqual(0.4, table.GlancingChance, 0.000001);
            Assert.AreEqual(0.2, table.CritChance, 0.000001);
            Assert.AreEqual(0.255, table.HitChance, 0.000001);
        }

        [TestMethod]
        [DataRow(0.0, DamageType.Miss)]
        [DataRow(0.08, DamageType.Miss)]
        [DataRow(0.0801, DamageType.Dodge)]
        [DataRow(0.145, DamageType.Dodge)]
        [DataRow(0.1451, DamageType.Glancing)]
        [DataRow(0.545, DamageType.Glancing)]
        [DataRow(0.5451, DamageType.Crit)]
        [DataRow(0.745, DamageType.Crit)]
        [DataRow(0.7451, DamageType.Hit)]
        [DataRow(1.0, DamageType.Hit)]
        public void ResolvesRollToSegment(double roll, DamageType expected)
        {
            var table = new AttackTable(0.08, 0.065, 0.4, 0.2);

            Assert.AreEqual(expected, table.Resolve(roll));
        }

        [TestMethod]
        public void GlancingPushesCritOffTable()
        {
            // a 40% glancing chance leaves 100 - 8 - 6.5 - 40 = 45.5% of the table for crit
            var table = new AttackTable(0.08, 0.065, 0.4, 0.6);

            Assert.AreEqual(0.455, table.CritChance, 0.000001);
            Assert.AreEqual(0.0, table.HitChance, 0.000001);
            Assert.AreEqual(DamageType.Crit, table.Resolve(0.99));
        }

        [TestMethod]
        public void MissPushesEverythingOffTable()
        {
            var table = new AttackTable(1.2, 0.065, 0.4, 0.2);

            Assert.AreEqual(1.0, table.MissChance, 0.000001);
            Assert.AreEqual(0.0, table.DodgeChance, 0.000001);
            Assert.AreEqual(0.0, table.GlancingChance, 0.000001);
            Assert.AreEqual(0.0, table.CritChance, 0.000001);
            Assert.AreEqual(0.0, table.HitChance, 0.000001);
        }

        [TestMethod]
        public void NegativeChancesTakeNoRoom()
        {
            var table = new AttackTable(-0.01, 0.05, 0.1, 0.2);

            Assert.AreEqual(0.0, table.MissChance, 0.000001);
            Assert.AreEqual(0.65, table.HitChance, 0.000001);
        }

        [TestMethod]
        public void WhiteHitUsesCalculators()
        {
            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.08));
            BaseStatCalculator.InjectMock(typeof(DodgeChanceCalculator), new FakeStatCalculator(0.065));
            BaseStatCalculator.InjectMock(typeof(GlancingChanceCalculator), new FakeStatCalculator(0.4));
            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.2));

            var table = AttackTable.ForWhiteHit(new GearItem(), new SimulationState());

            Assert.AreEqual(0.08, table.MissChance, 0.000001);
            Assert.AreEqual(0.065, table.DodgeChance, 0.000001);
            Assert.AreEqual(0.4, table.GlancingChance, 0.000001);
            Assert.AreEqual(0.2, table.CritChance, 0.000001);
        }
    }
}
