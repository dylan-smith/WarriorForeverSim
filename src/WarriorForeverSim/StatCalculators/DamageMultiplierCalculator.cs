namespace WarriorForeverSim
{
    public class DamageMultiplierCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<DamageMultiplierCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var damageMultiplier = 1.0;

            return damageMultiplier;
        }
    }
}
