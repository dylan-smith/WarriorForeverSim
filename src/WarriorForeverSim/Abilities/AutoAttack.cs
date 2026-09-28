namespace WarriorForeverSim
{
    public class AutoAttack
    {
        public static bool CanUse(SimulationState state) => state.Config.Gear.MainHand != null && !state.Auras.Contains(Aura.SwingTimerCooldown);

        public static void Use(SimulationState state) => state.Events.Add(new AutoAttackSwingEvent(state.CurrentTime));
    }
}
