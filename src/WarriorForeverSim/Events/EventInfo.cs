namespace WarriorForeverSim
{
    public abstract class EventInfo
    {
        public double Timestamp { get; set; }

        public EventInfo(double timestamp) => Timestamp = timestamp;

        // Human-readable summary shown in the combat log.
        public virtual string Description => GetType().Name;

        public abstract void ProcessEvent(SimulationState state);

        public override string ToString() => $"[{Timestamp.ToString("F1")}] {GetType().Name}";
    }
}
