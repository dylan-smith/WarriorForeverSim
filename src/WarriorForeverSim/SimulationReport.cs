using System.Collections.Generic;
using System.Linq;

namespace WarriorForeverSim
{
    public class SimulationReport
    {
        public double TotalDamage { get; }
        public double Dps { get; }
        public int Hits { get; }
        public int Crits { get; }
        public int Misses { get; }
        public IReadOnlyList<string> Warnings { get; }
        public IReadOnlyList<string> Errors { get; }

        private SimulationReport(double totalDamage, double dps, int hits, int crits, int misses, IReadOnlyList<string> warnings, IReadOnlyList<string> errors)
        {
            TotalDamage = totalDamage;
            Dps = dps;
            Hits = hits;
            Crits = crits;
            Misses = misses;
            Warnings = warnings;
            Errors = errors;
        }

        public static SimulationReport FromState(SimulationState state)
        {
            var damageEvents = state.DamageEvents.ToList();
            var totalDamage = damageEvents.Sum(e => e.Damage);
            var fightLength = state.Config.SimulationSettings.FightLength;
            var dps = fightLength > 0 ? totalDamage / fightLength : 0.0;

            return new SimulationReport(
                totalDamage,
                dps,
                damageEvents.Count(e => e.DamageType == DamageType.Hit),
                damageEvents.Count(e => e.DamageType == DamageType.Crit),
                damageEvents.Count(e => e.DamageType == DamageType.Miss),
                state.Warnings.ToList(),
                state.Errors.ToList());
        }
    }
}
