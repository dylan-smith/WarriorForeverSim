namespace WarriorForeverSim
{
    public class AttackPowerCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<AttackPowerCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var attackPower = state.Config.Gear.GetStatTotal(x => x.AttackPower);
            attackPower += AgilityCalculator.Calculate(state);

            return attackPower;
        }
    }
}
