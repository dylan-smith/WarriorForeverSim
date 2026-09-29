namespace WarriorForeverSim
{
    public class MissChanceCalculator : BaseStatCalculator
    {
        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<MissChanceCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            // TODO: This is main hand white hits only, yellow attacks have a different calc (and I think off hand too)
            var bossDefense = state.Config.BossSettings.Defense;
            var weaponSkill = WeaponSkillCalculator.Calculate(weapon, state);
            var defenseSkillDiff = bossDefense - weaponSkill;

            var baseMissChance = state.Config.Gear.IsDualWielding() ? 0.24 : 0.05;
            var hitSuppression = defenseSkillDiff > 10
                ? defenseSkillDiff * 0.002
                : defenseSkillDiff * 0.001;

            var missChance = baseMissChance + hitSuppression;

            var hitChance = state.Config.Gear.GetStatTotal(x => x.HitRating);
            missChance -= hitChance;

            return missChance.Normalize();
        }
    }
}
