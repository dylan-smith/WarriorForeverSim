namespace WarriorForeverSim
{
    public class MeleeCritDamageMultiplierCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<MeleeCritDamageMultiplierCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var bossType = state.Config.BossSettings.BossType;

            var dmgMultiplier = 1.0;

            if (state.Config.Talents.TryGetValue(Talent.MonsterSlaying, out var monsterSlayingRank) && (bossType == BossType.Beast || bossType == BossType.Giant || bossType == BossType.Dragonkin))
            {
                dmgMultiplier += (0.01 * monsterSlayingRank);
            }

            if (state.Config.Talents.TryGetValue(Talent.HumanoidSlaying, out var humanoidSlayingRank) && bossType == BossType.Humanoid)
            {
                dmgMultiplier += (0.01 * humanoidSlayingRank);
            }

            if (state.Auras.Contains(Aura.RelentlessEarthstormDiamond))
            {
                dmgMultiplier += 0.03;
            }

            return dmgMultiplier;
        }
    }
}
