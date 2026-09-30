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
            double? critRoll = null;
            var damageDetails = new List<(string Label, string Value)>();

            var missChance = MissChanceCalculator.Calculate(weapon, state);
            var critChance = MeleeCritCalculator.Calculate(state);

            var missRoll = RandomGenerator.Roll(RollType.MeleeMiss);

            if (missRoll <= missChance)
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

                // Assuming crit uses a 2-roll system as per this article:
                // https://wowwiki-archive.fandom.com/wiki/Attack_table#Ranged_attacks
                critRoll = RandomGenerator.Roll(RollType.MeleeCrit);

                if (critRoll <= critChance)
                {
                    var critDamageMultiplier = MeleeCritDamageMultiplierCalculator.Calculate(state);

                    autoAttackDamage *= 2;
                    autoAttackDamage *= critDamageMultiplier;
                    damageType = DamageType.Crit;

                    damageDetails.Add(("Crit multiplier", Multiplier(2 * critDamageMultiplier)));
                }
            }

            // TODO: Boss armor reduction

            var critRollChance = critChance;

            // TODO: If we do these calcs earlier we can do just one roll instead of 2
            critChance *= (1 - missChance);
            var hitChance = 1 - missChance - critChance;

            DamageEvent = new DamageEvent(Timestamp, autoAttackDamage, damageType, missChance, critChance, hitChance)
            {
                MissRoll = missRoll,
                CritRoll = critRoll,
                CritRollChance = critRollChance,
            };

            DamageEvent.AddDetail("Attack table", "Outcome chances", $"Hit {Percent(hitChance)} · Crit {Percent(critChance)} · Miss {Percent(missChance)}");

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

        private static string Percent(double value) => (value * 100).ToString("F1", CultureInfo.InvariantCulture) + "%";
    }
}
