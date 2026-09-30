namespace WarriorForeverSim
{
    public class DamageEvent : EventInfo
    {
        public readonly double Damage;
        public readonly DamageType DamageType;
        public readonly double MissChance;
        public readonly double CritChance;
        public readonly double HitChance;

        // The raw attack-table roll, for display. Miss, crit and hit chances are the widths of
        // consecutive table segments starting at 0, and the roll landed in one of them.
        public double? AttackRoll { get; init; }

        public DamageEvent(double timestamp, double damage, DamageType damageType, double missChance, double critChance, double hitChance) : base(timestamp)
        {
            Damage = damage;
            DamageType = damageType;
            MissChance = missChance;
            CritChance = critChance;
            HitChance = hitChance;
        }

        public override string Description => DamageType == DamageType.Miss ? "Miss" : $"{DamageType} for {Damage:F0}";

        public override void ProcessEvent(SimulationState state)
        {
            // TODO: Windfury proc
        }

        public override string ToString() => $"[{Timestamp.ToString("F1")}] {DamageType} for {Damage.ToString("F2")} [Miss: {MissChance.ToString("F3")}, Crit: {CritChance.ToString("F3")}, Hit: {HitChance.ToString("F3")}]";
    }
}
