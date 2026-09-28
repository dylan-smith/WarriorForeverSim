namespace WarriorForeverSim
{
    public class DamageMultiplierCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<DamageMultiplierCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var bossType = state.Config.BossSettings.BossType;
            var damageMultiplier = 1.0;

            if (state.Config.Talents.TryGetValue(Talent.MonsterSlaying, out var monsterSlayingRank) && (bossType == BossType.Beast || bossType == BossType.Giant || bossType == BossType.Dragonkin))
            {
                damageMultiplier *= 1 + (0.01 * monsterSlayingRank);
            }

            if (state.Config.Talents.TryGetValue(Talent.HumanoidSlaying, out var humanoidSlayingRank) && bossType == BossType.Humanoid)
            {
                damageMultiplier *= 1 + (0.01 * humanoidSlayingRank);
            }

            if (state.Config.Talents.TryGetValue(Talent.FocusedFire, out var focusedFireRank))
            {
                damageMultiplier *= 1 + (0.01 * focusedFireRank);
            }

            return damageMultiplier;
        }
    }
}
