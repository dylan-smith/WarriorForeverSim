using System.Collections.Generic;
using System.Globalization;

namespace WarriorForeverSim
{
    public class AutoAttackSwingEvent : EventInfo
    {
        public DamageEvent DamageEvent { get; set; }

        public AutoAttackSwingEvent(double timestamp) : base(timestamp)
        { }

        public override string Description => "Main-hand swing";

        public override void ProcessEvent(SimulationState state)
        {
            var weapon = state.Config.Gear.MainHand;
            var haste = MeleeHasteCalculator.Calculate(state);
            var swingSpeed = weapon.Speed / haste;
            state.Events.Add(new SwingTimerCompletedEvent(Timestamp + swingSpeed));
            state.Auras.Add(Aura.SwingTimerCooldown);

            AddDetail("Swing", "Weapon", weapon.Name ?? "Main hand");
            AddDetail("Swing", "Base speed", Seconds(weapon.Speed));
            AddDetail("Swing", "Haste", Multiplier(haste));
            AddDetail("Swing", "Swing speed", Seconds(swingSpeed));
            AddDetail("Swing", "Next swing ready at", Seconds(Timestamp + swingSpeed));

            var autoAttackDamage = 0.0;
            var damageDetails = new List<(string Label, string Value)>();

            // White hits make a single roll against one attack table: miss, dodge, glancing, crit, then hit.
            var attackTable = AttackTable.ForWhiteHit(weapon, state);
            var attackRoll = RandomGenerator.Roll(RollType.MeleeAttackTable);
            var damageType = attackTable.Resolve(attackRoll);

            if (damageType is not (DamageType.Miss or DamageType.Dodge))
            {
                var meleeAP = MeleeAttackPowerCalculator.Calculate(state);
                var weaponDamage = (weapon.MinDamage + weapon.MaxDamage) / 2;
                var attackPowerBonus = (meleeAP / 14) * weapon.Speed; // 14 RAP = 1 DPS
                var damageMultiplier = DamageMultiplierCalculator.Calculate(state);

                autoAttackDamage = weaponDamage;
                autoAttackDamage += attackPowerBonus;
                autoAttackDamage *= damageMultiplier;

                damageDetails =
                [
                    ("Weapon damage (avg)", Number(weaponDamage)),
                    ("Attack power", meleeAP.ToString("F0", CultureInfo.InvariantCulture)),
                    ("Attack power bonus", Number(attackPowerBonus)),
                    ("Damage multiplier", Multiplier(damageMultiplier)),
                ];

                if (damageType == DamageType.Glancing)
                {
                    var glancingDamageMultiplier = GlancingDamageCalculator.Calculate(weapon, state);

                    autoAttackDamage *= glancingDamageMultiplier;

                    damageDetails.Add(("Glancing multiplier", Multiplier(glancingDamageMultiplier)));
                }
                else if (damageType == DamageType.Crit)
                {
                    var critDamageMultiplier = MeleeCritDamageMultiplierCalculator.Calculate(state);

                    autoAttackDamage *= 2;
                    autoAttackDamage *= critDamageMultiplier;

                    damageDetails.Add(("Crit multiplier", Multiplier(2 * critDamageMultiplier)));
                }
            }

            // TODO: Boss armor reduction

            DamageEvent = new DamageEvent(Timestamp, autoAttackDamage, damageType, attackTable)
            {
                AttackRoll = attackRoll,
            };

            foreach (var (label, value) in damageDetails)
            {
                DamageEvent.AddDetail("Damage", label, value);
            }

            if (damageDetails.Count > 0)
            {
                DamageEvent.AddDetail("Damage", "Final damage", Number(autoAttackDamage));
            }

            state.Events.Add(DamageEvent);
        }

        private static string Seconds(double value) => value.ToString("F2", CultureInfo.InvariantCulture) + "s";

        private static string Number(double value) => value.ToString("F1", CultureInfo.InvariantCulture);

        private static string Multiplier(double value) => "×" + value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
