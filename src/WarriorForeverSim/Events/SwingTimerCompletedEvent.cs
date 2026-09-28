namespace WarriorForeverSim
{
    public class SwingTimerCompletedEvent : EventInfo
    {
        public SwingTimerCompletedEvent(double timestamp) : base(timestamp)
        { }

        public override void ProcessEvent(SimulationState state)
        {
            if (!state.Auras.Remove(Aura.SwingTimerCooldown))
            {
                // TODO: Richer exceptions
                throw new System.Exception("SwingTimerCooldown aura was expected in State");
            }
        }
    }
}
