namespace WarriorForeverSim
{
    public class RangedAttackPowerCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<RangedAttackPowerCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var rangedAP = AttackPowerCalculator.Calculate(state);
            rangedAP += state.Config.Gear.GetStatTotal(x => x.RangedAttackPower);

            // This appears to be base RAP, I tested it by removing all gear and talents
            rangedAP += 130;

            if (state.Auras.Contains(Aura.AspectOfTheHawk))
            {
                rangedAP += 155;
            }

            if (state.Config.Talents.TryGetValue(Talent.CarefulAim, out var carefulAimRank))
            {
                var intellect = IntellectCalculator.Calculate(state);
                rangedAP += intellect * (0.15 * carefulAimRank);
                rangedAP = rangedAP.Floor();
            }

            if (state.Config.Talents.TryGetValue(Talent.MasterMarksman, out var masterMarksmanRank))
            {
                rangedAP *= 1 + (0.02 * masterMarksmanRank);
                rangedAP = rangedAP.Floor();
            }

            if (state.Config.Talents.TryGetValue(Talent.SurvivalInstincts, out var survivalInstinctsRank))
            {
                rangedAP *= 1 + (0.02 * survivalInstinctsRank);
                rangedAP = rangedAP.Floor();
            }

            if (state.Config.Buffs.Contains(Buff.HuntersMark) || state.Config.Buffs.Contains(Buff.ImprovedHuntersMark))
            {
                rangedAP += 440;
            }

            return rangedAP;
        }
    }
}
