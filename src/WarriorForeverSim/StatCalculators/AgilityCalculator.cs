namespace WarriorForeverSim
{
    public class AgilityCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<AgilityCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var agility = state.Config.PlayerSettings.Agility;
            agility += state.Config.Gear.GetStatTotal(x => x.Agility);

            return agility;
        }
    }
}
