namespace WarriorForeverSim
{
    public class MeleeAttackPowerCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<MeleeAttackPowerCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var meleeAP = AttackPowerCalculator.Calculate(state);
            meleeAP += state.Config.Gear.GetStatTotal(x => x.MeleeAttackPower);
            meleeAP += StrengthCalculator.Calculate(state);

            // Base MAP seems to be 120, tested this by removing all gear/talents
            meleeAP += 120;

            return meleeAP;
        }
    }
}
