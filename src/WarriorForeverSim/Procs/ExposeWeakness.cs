namespace WarriorForeverSim
{
    public static class ExposeWeakness
    {
        public static int AttackPower { get; set; } = 0;

        public static void ProcessEvent(AutoShotCompletedEvent e, SimulationState state)
        {
            if (e.DamageEvent.DamageType != DamageType.Crit)
            {
                return;
            }

            if (state.Config.Talents.TryGetValue(Talent.ExposeWeakness, out var exposeWeaknessRank))
            {
                var procChance = exposeWeaknessRank / 3.0;

                var roll = RandomGenerator.Roll(RollType.ExposeWeaknessProc);

                if (roll <= procChance)
                {
                    state.Events.Add(new ExposeWeaknessProcEvent(e.Timestamp));
                }
            }
        }
    }
}
