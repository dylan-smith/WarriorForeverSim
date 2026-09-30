namespace WarriorForeverSim
{
    public static class EventPublisher
    {
        public static void PublishEvent(EventInfo e, SimulationState state)
        {
            state.ProcessedEvents.Add(e);

            // switch (e)
            // {
            //     case AutoAttackSwingEvent ev:
            //         ExposeWeakness.ProcessEvent(ev, state);
            //         MasterTactician.ProcessEvent(ev, state);
            //         break;
            // }
        }
    }
}
