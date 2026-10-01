using System;

namespace WarriorForeverSim
{
    // A single-roll melee attack table: consecutive miss, dodge, glancing, crit and hit segments
    // starting at 0. Each entry only gets the room the entries before it leave, so a large miss,
    // dodge or glancing chance pushes crit (and then hit) off the table.
    // Parry and block are left out because the sim attacks from behind.
    // https://github.com/magey/classic-warrior/wiki/Attack-table
    public class AttackTable
    {
        public double MissChance { get; }
        public double DodgeChance { get; }
        public double GlancingChance { get; }
        public double CritChance { get; }
        public double HitChance { get; }

        public AttackTable(double missChance, double dodgeChance, double glancingChance, double critChance)
        {
            var remaining = 1.0;

            MissChance = Take(missChance, ref remaining);
            DodgeChance = Take(dodgeChance, ref remaining);
            GlancingChance = Take(glancingChance, ref remaining);
            CritChance = Take(critChance, ref remaining);
            HitChance = remaining;
        }

        // White hits: everything above. Glancing blows only happen to white hits.
        public static AttackTable ForWhiteHit(GearItem weapon, SimulationState state) => new(
            MissChanceCalculator.Calculate(weapon, state),
            DodgeChanceCalculator.Calculate(weapon, state),
            GlancingChanceCalculator.Calculate(weapon, state),
            CritCalculator.Calculate(weapon, state));

        // A roll on a segment's upper boundary belongs to that segment.
        public DamageType Resolve(double roll)
        {
            var threshold = MissChance;

            if (roll <= threshold)
            {
                return DamageType.Miss;
            }

            threshold += DodgeChance;

            if (roll <= threshold)
            {
                return DamageType.Dodge;
            }

            threshold += GlancingChance;

            if (roll <= threshold)
            {
                return DamageType.Glancing;
            }

            threshold += CritChance;

            return roll <= threshold ? DamageType.Crit : DamageType.Hit;
        }

        private static double Take(double chance, ref double remaining)
        {
            var taken = Math.Min(Math.Max(chance, 0.0), remaining);
            remaining -= taken;
            return taken;
        }
    }
}
