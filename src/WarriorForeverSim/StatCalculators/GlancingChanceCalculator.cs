using System;

namespace WarriorForeverSim
{
    public class GlancingChanceCalculator : BaseStatCalculator
    {
        private const double BaseGlancingChance = 0.10;

        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<GlancingChanceCalculator>(weapon, state);

        // White hits against mobs glance 10% of the time, plus 2% per point of defense above the attacker's
        // weapon skill, with no more than level * 5 weapon skill counting. That makes it 40% against a
        // level 63 boss no matter how much weapon skill you have.
        // https://github.com/magey/classic-warrior/wiki/Attack-table#glancing-blows
        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var bossDefense = state.Config.BossSettings.Defense;
            var weaponSkill = Math.Min(state.Config.PlayerSettings.Level * 5, WeaponSkillCalculator.Calculate(weapon, state));

            var glancingChance = BaseGlancingChance + ((bossDefense - weaponSkill) * 0.02);

            return glancingChance.Normalize();
        }
    }
}
