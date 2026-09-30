using System.Collections.Generic;

namespace WarriorForeverSim
{
    public abstract class EventInfo
    {
        public double Timestamp { get; set; }

        // Extra data recorded while the event is processed, for the combat log tooltip.
        public IList<EventDetail> Details { get; } = new List<EventDetail>();

        // Auras active right after the event was processed (set by EventPublisher).
        public IReadOnlyList<Aura> ActiveAuras { get; set; } = [];

        public EventInfo(double timestamp) => Timestamp = timestamp;

        // Human-readable summary shown in the combat log.
        public virtual string Description => GetType().Name;

        public void AddDetail(string section, string label, string value) => Details.Add(new EventDetail(section, label, value));

        public abstract void ProcessEvent(SimulationState state);

        public override string ToString() => $"[{Timestamp.ToString("F1")}] {GetType().Name}";
    }
}
