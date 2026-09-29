namespace WarriorForeverSim
{
    public class StrengthCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<StrengthCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var strength = state.Config.PlayerSettings.Strength;
            strength += state.Config.Gear.GetStatTotal(x => x.Strength);

            return strength;
        }
    }
}
