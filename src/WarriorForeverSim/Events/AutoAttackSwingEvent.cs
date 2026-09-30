using System;
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

            double autoAttackDamage;
            DamageType damageType;
            var damageDetails = new List<(string Label, string Value)>();

            // White hits make a single roll against one attack table: miss, then crit, then hit.
            // Crit is pushed off the table when the entries above it leave it no room.
            var missChance = MissChanceCalculator.Calculate(weapon, state);
            var critChance = Math.Min(CritCalculator.Calculate(weapon, state), 1 - missChance);
            var hitChance = 1 - missChance - critChance;

            var attackRoll = RandomGenerator.Roll(RollType.MeleeAttackTable);

            if (attackRoll <= missChance)
            {
                autoAttackDamage = 0.0;
                damageType = DamageType.Miss;
            }
            else
            {
                var meleeAP = MeleeAttackPowerCalculator.Calculate(state);
                var weaponDamage = (weapon.MinDamage + weapon.MaxDamage) / 2;
                var attackPowerBonus = (meleeAP / 14) * weapon.Speed; // 14 RAP = 1 DPS
                var damageMultiplier = DamageMultiplierCalculator.Calculate(state);

                autoAttackDamage = weaponDamage;
                autoAttackDamage += attackPowerBonus;
                autoAttackDamage *= damageMultiplier;

                damageType = DamageType.Hit;

                damageDetails =
                [
                    ("Weapon damage (avg)", Number(weaponDamage)),
                    ("Attack power", meleeAP.ToString("F0", CultureInfo.InvariantCulture)),
                    ("Attack power bonus", Number(attackPowerBonus)),
                    ("Damage multiplier", Multiplier(damageMultiplier)),
                ];

                if (attackRoll <= missChance + critChance)
                {
                    var critDamageMultiplier = MeleeCritDamageMultiplierCalculator.Calculate(state);

                    autoAttackDamage *= 2;
                    autoAttackDamage *= critDamageMultiplier;
                    damageType = DamageType.Crit;

                    damageDetails.Add(("Crit multiplier", Multiplier(2 * critDamageMultiplier)));
                }
            }

            // TODO: Boss armor reduction

            DamageEvent = new DamageEvent(Timestamp, autoAttackDamage, damageType, missChance, critChance, hitChance)
            {
                AttackRoll = attackRoll,
            };

            foreach (var (label, value) in damageDetails)
            {
                DamageEvent.AddDetail("Damage", label, value);
            }

            if (damageType != DamageType.Miss)
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
