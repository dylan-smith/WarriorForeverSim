namespace WarriorForeverSim
{
    public class DamageEvent : EventInfo
    {
        public readonly double Damage;
        public readonly DamageType DamageType;
        public readonly double MissChance;
        public readonly double CritChance;
        public readonly double HitChance;

        // Raw attack-table rolls, for display. CritRoll is null when the attack missed;
        // CritRollChance is the crit chance the crit roll was compared against.
        public double? MissRoll { get; init; }
        public double? CritRoll { get; init; }
        public double? CritRollChance { get; init; }

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
