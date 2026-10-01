using System;

namespace WarriorForeverSim
{
    public class GlancingDamageCalculator : BaseStatCalculator
    {
        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<GlancingDamageCalculator>(weapon, state);

        // The fraction of normal damage a glancing blow deals. In game it is rolled between a low and a high
        // end that both depend on defense minus weapon skill (uncapped, so weapon skill above level * 5 helps
        // here); the sim uses average weapon damage, so it uses the average of the two ends too.
        // 300 skill vs. 315 defense averages 0.65 (a 35% penalty), 305 skill averages 0.85.
        // https://github.com/magey/classic-warrior/wiki/Attack-table#glancing-blows
        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var bossDefense = state.Config.BossSettings.Defense;
            var weaponSkill = WeaponSkillCalculator.Calculate(weapon, state);
            var defenseSkillDiff = bossDefense - weaponSkill;

            // The wiki gives the low end no floor; keep it from going negative against very high defense.
            var lowEnd = Math.Clamp(1.3 - (0.05 * defenseSkillDiff), 0.0, 0.91);
            var highEnd = Math.Clamp(1.2 - (0.03 * defenseSkillDiff), 0.2, 0.99);

            return (lowEnd + highEnd) / 2;
        }
    }
}
