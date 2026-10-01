namespace WarriorForeverSim
{
    public class DamageEvent : EventInfo
    {
        public readonly double Damage;
        public readonly DamageType DamageType;
        public readonly AttackTable AttackTable;

        // The raw attack-table roll, for display. It landed in one of AttackTable's consecutive segments.
        public double? AttackRoll { get; init; }

        public DamageEvent(double timestamp, double damage, DamageType damageType, AttackTable attackTable) : base(timestamp)
        {
            Damage = damage;
            DamageType = damageType;
            AttackTable = attackTable;
        }

        public double MissChance => AttackTable.MissChance;
        public double DodgeChance => AttackTable.DodgeChance;
        public double GlancingChance => AttackTable.GlancingChance;
        public double CritChance => AttackTable.CritChance;
        public double HitChance => AttackTable.HitChance;

        public override string Description => DamageType is DamageType.Miss or DamageType.Dodge ? DamageType.ToString() : $"{DamageType} for {Damage:F0}";

        public override void ProcessEvent(SimulationState state)
        {
            // TODO: Windfury proc
        }

        public override string ToString() => $"[{Timestamp.ToString("F1")}] {DamageType} for {Damage.ToString("F2")} [Miss: {MissChance.ToString("F3")}, Dodge: {DodgeChance.ToString("F3")}, Glancing: {GlancingChance.ToString("F3")}, Crit: {CritChance.ToString("F3")}, Hit: {HitChance.ToString("F3")}]";
    }
}
