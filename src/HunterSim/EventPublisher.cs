using System;

namespace HunterSim
{
    public static class EventPublisher
    {
        public static void PublishEvent(EventInfo e, SimulationState state)
        {
            state.ProcessedEvents.Add(e);
            Console.WriteLine(e.ToString());

            switch (e)
            {
                case AutoShotCompletedEvent ev:
                    ImprovedAspectOfTheHawk.ProcessEvent(ev, state);
                    ExposeWeakness.ProcessEvent(ev, state);
                    MasterTactician.ProcessEvent(ev, state);
                    break;
            }
        }
    }
}
