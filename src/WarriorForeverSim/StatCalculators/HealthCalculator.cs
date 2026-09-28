namespace WarriorForeverSim
{
    public class HealthCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<HealthCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var health = state.Config.PlayerSettings.Health;
            health += StaminaCalculator.Calculate(state) * 10;

            if (state.Config.Talents.TryGetValue(Talent.EnduranceTraining, out var enduranceTrainingRank))
            {
                health *= 1 + (enduranceTrainingRank * 0.01);
            }

            if (state.Config.Talents.TryGetValue(Talent.Survivalist, out var survivalistRank))
            {
                health *= 1 + (survivalistRank * 0.02);
            }

            return health;
        }
    }
}
