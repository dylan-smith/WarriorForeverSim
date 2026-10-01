namespace WarriorForeverSim
{
    public class DodgeChanceCalculator : BaseStatCalculator
    {
        private const double BaseDodgeChance = 0.05;

        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<DodgeChanceCalculator>(weapon, state);

        // Mobs dodge 5% of attacks, adjusted by 0.1% per point of defense above (or below) the attacker's
        // weapon skill. Unlike crit and glancing, weapon skill above level * 5 still counts.
        // https://github.com/magey/classic-warrior/wiki/Attack-table#dodge
        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var bossDefense = state.Config.BossSettings.Defense;
            var weaponSkill = WeaponSkillCalculator.Calculate(weapon, state);

            var dodgeChance = BaseDodgeChance + ((bossDefense - weaponSkill) * 0.001);

            return dodgeChance.Normalize();
        }
    }
}
