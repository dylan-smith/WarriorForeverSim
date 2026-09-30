using System;
using System.Linq;

namespace WarriorForeverSim
{
    public class Simulation
    {
        public readonly SimulationState State = new SimulationState();

        public Simulation(SimulationConfig config) => State.Config = config;

        public SimulationState Run()
        {
            // TODO: Generate Report/Analysis

            if (!State.Validate())
            {
                return State;
            }

            while (true)
            {
                ExecuteRotation();
                var nextEvent = GetNextEvent();

                State.CurrentTime = nextEvent.Timestamp;

                if (State.CurrentTime > State.Config.SimulationSettings.FightLength)
                {
                    return State;
                }

                while (nextEvent != null && nextEvent.Timestamp <= State.CurrentTime)
                {
                    State.Events.Remove(nextEvent);
                    nextEvent.ProcessEvent(State);

                    EventPublisher.PublishEvent(nextEvent, State);

                    nextEvent = GetNextEvent();
                }
            }

            throw new Exception("This should never happen");
        }

        private void ExecuteRotation()
        {
            // TODO: config setting for whether we are responsible for refreshing battle shout
            // TODO: config settings for potion usage
            // TODO: configurable reaction time delays for all abilities
            if (AutoAttack.CanUse(State))
            {
                AutoAttack.Use(State);
            }
        }

        private EventInfo GetNextEvent() => State.Events.OrderBy(e => e.Timestamp).FirstOrDefault();
    }
}
