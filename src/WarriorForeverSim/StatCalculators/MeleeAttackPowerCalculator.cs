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

            if (state.Config.Buffs.Contains(Buff.BattleShout))
            {
                meleeAP += 305;
            }

            if (state.Config.Buffs.Contains(Buff.ImprovedBattleShout))
            {
                // Warrior talent Commanding Presence (at 5/5) gives a 25% bonus to battle shout buff
                meleeAP += 381;
            }

            if (state.Config.Talents.TryGetValue(Talent.SurvivalInstincts, out var survivalInstinctsRank))
            {
                meleeAP *= 1 + (0.02 * survivalInstinctsRank);
                meleeAP = meleeAP.Floor();
            }

            if (state.Config.Buffs.Contains(Buff.ImprovedHuntersMark))
            {
                meleeAP += 110;
            }
            else if (state.Config.Talents.TryGetValue(Talent.ImprovedHuntersMark, out var improvedHuntersMarkRank))
            {
                meleeAP += (110 * (0.2 * improvedHuntersMarkRank)).Floor();
            }

            return meleeAP;
        }
    }
}
