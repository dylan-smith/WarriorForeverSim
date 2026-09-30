using System.Collections.Generic;

namespace WarriorForeverSim
{
    public class SimulationLogEntry
    {
        public double Timestamp { get; }
        public string Event { get; }
        public string Description { get; }
        public double? Damage { get; }
        public DamageType? DamageType { get; }
        public double? MissChance { get; }
        public double? CritChance { get; }
        public double TotalDamage { get; }

        private SimulationLogEntry(EventInfo e, double totalDamage)
        {
            Timestamp = e.Timestamp;
            Event = e.GetType().Name;
            Description = e.Description;
            TotalDamage = totalDamage;

            if (e is DamageEvent damageEvent)
            {
                Damage = damageEvent.Damage;
                DamageType = damageEvent.DamageType;
                MissChance = damageEvent.MissChance;
                CritChance = damageEvent.CritChance;
            }
        }

        public static IReadOnlyList<SimulationLogEntry> FromState(SimulationState state)
        {
            var log = new List<SimulationLogEntry>();
            var totalDamage = 0.0;

            foreach (var e in state.ProcessedEvents)
            {
                if (e is DamageEvent damageEvent)
                {
                    totalDamage += damageEvent.Damage;
                }

                log.Add(new SimulationLogEntry(e, totalDamage));
            }

            return log;
        }
    }
}
