namespace WarriorForeverSim
{
    public class RangedCritDamageMultiplierCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<RangedCritDamageMultiplierCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var bossType = state.Config.BossSettings.BossType;
            var critMultiplier = 1.0;

            if (state.Config.Talents.TryGetValue(Talent.MonsterSlaying, out var monsterSlayingRank) && (bossType == BossType.Beast || bossType == BossType.Giant || bossType == BossType.Dragonkin))
            {
                critMultiplier += (0.01 * monsterSlayingRank);
            }

            if (state.Config.Talents.TryGetValue(Talent.HumanoidSlaying, out var humanoidSlayingRank) && bossType == BossType.Humanoid)
            {
                critMultiplier += (0.01 * humanoidSlayingRank);
            }

            if (state.Config.Talents.TryGetValue(Talent.MortalShots, out var mortalShotsRank))
            {
                // Mortal shots only increases the BONUS crit damage by 6%, so overall damage is multiplied by 0.03
                // TODO: not sure if that is actually how the math is supposed to work
                critMultiplier += mortalShotsRank * 0.03;
            }

            if (state.Auras.Contains(Aura.RelentlessEarthstormDiamond))
            {
                critMultiplier += 0.03;
            }

            return critMultiplier;
        }
    }
}
