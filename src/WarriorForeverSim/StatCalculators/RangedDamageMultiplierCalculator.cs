namespace WarriorForeverSim
{
    public class RangedDamageMultiplierCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<RangedDamageMultiplierCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var damageMultiplier = DamageMultiplierCalculator.Calculate(state);

            if (state.Config.Talents.TryGetValue(Talent.RangedWeaponSpecialization, out var rangedWeaponSpecializationRank))
            {
                damageMultiplier += 0.01 * rangedWeaponSpecializationRank;
            }

            return damageMultiplier;
        }
    }
}
