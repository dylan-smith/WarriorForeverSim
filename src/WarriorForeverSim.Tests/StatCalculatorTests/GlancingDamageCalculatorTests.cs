using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.StatCalculatorTests
{
    [TestClass]
    public class GlancingDamageCalculatorTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
        }

        // Penalties from the weapon skill table at https://github.com/magey/classic-warrior/wiki/Attack-table#weapon-skill
        [TestMethod]
        [DataRow(300, 0.65)] // 15 below: low 0.55, high 0.75
        [DataRow(301, 0.69)] // 14 below: 31% penalty
        [DataRow(302, 0.73)] // 13 below: 27% penalty
        [DataRow(303, 0.77)] // 12 below: 23% penalty
        [DataRow(304, 0.81)] // 11 below: 19% penalty
        [DataRow(305, 0.85)] // 10 below: low 0.8, high 0.9
        [DataRow(306, 0.89)] // 9 below: 11% penalty
        [DataRow(307, 0.93)] // 8 below: 7% penalty
        [DataRow(308, 0.95)] // 7 below: low capped at 0.91, high at 0.99
        [DataRow(315, 0.95)] // equal: both ends capped
        [DataRow(320, 0.95)] // skill above defense: still capped
        public void GlancingDamageVsBoss(double weaponSkill, double expected)
        {
            var state = CreateGlancingState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(weaponSkill));

            Assert.AreEqual(expected, GlancingDamageCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void GlancingDamageUsesWeaponSkillFromGear()
        {
            var state = CreateGlancingState(bossLevel: 63);
            state.Config.Gear.Hands = new GearItem() { WeaponSkill = { [WeaponType.OneHandedSword] = 5 } };

            // unlike glancing chance, skill above level * 5 counts: 305 vs 315
            Assert.AreEqual(0.85, GlancingDamageCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        [TestMethod]
        public void GlancingDamageEndsAreClampedAtLargeSkillGaps()
        {
            var state = CreateGlancingState(bossLevel: 63);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(200));

            // 115 below: low end floored at 0, high end at 0.2
            Assert.AreEqual(0.1, GlancingDamageCalculator.Calculate(state.Config.Gear.MainHand, state), 0.000001);
        }

        private static SimulationState CreateGlancingState(int bossLevel)
        {
            var state = new SimulationState();
            state.Config.PlayerSettings.Race = Race.Human;
            state.Config.PlayerSettings.Level = 60;
            state.Config.BossSettings.Level = bossLevel;
            state.Config.Gear.MainHand = new GearItem() { WeaponType = WeaponType.OneHandedSword };

            return state;
        }
    }
}
