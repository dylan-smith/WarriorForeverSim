namespace WarriorForeverSim
{
    public class MeleeCritDamageMultiplierCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<MeleeCritDamageMultiplierCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var dmgMultiplier = 1.0;

            return dmgMultiplier;
        }
    }
}
