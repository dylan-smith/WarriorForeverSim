namespace HunterSim
{
    public class AutoShot
    {
        public static bool CanUse(SimulationState state) => !state.Auras.Contains(Aura.AutoShotOnCooldown);

        public static void Use(SimulationState state) => state.Events.Add(new AutoShotCastEvent(state.CurrentTime));
    }
}
