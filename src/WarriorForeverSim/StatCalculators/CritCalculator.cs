namespace WarriorForeverSim
{
    // Crit as the character sheet shows it, before any suppression from the target's level.
    public class CritCalculator : BaseStatCalculator
    {
        private const double BaseCritChance = 0.0; // warriors have no base crit in Classic
        private const double AgilityPerCritPercent = 20; // level 60 value; other levels aren't modelled

        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<CritCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var critChance = BaseCritChance;
            critChance += AgilityCalculator.Calculate(state) / AgilityPerCritPercent / 100;
            critChance += AuraCritCalculator.Calculate(weapon, state);

            return critChance;
        }
    }
}
