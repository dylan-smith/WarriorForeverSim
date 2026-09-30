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
            var swingSpeed = state.Config.Gear.MainHand.Speed / MeleeHasteCalculator.Calculate(state);
            state.Events.Add(new SwingTimerCompletedEvent(Timestamp + swingSpeed));
            state.Auras.Add(Aura.SwingTimerCooldown);

            double autoAttackDamage;
            DamageType damageType;

            var missChance = MissChanceCalculator.Calculate(state.Config.Gear.MainHand, state);
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

                autoAttackDamage = (state.Config.Gear.MainHand.MinDamage + state.Config.Gear.MainHand.MaxDamage) / 2;
                autoAttackDamage += (meleeAP / 14) * state.Config.Gear.MainHand.Speed; // 14 RAP = 1 DPS
                autoAttackDamage *= DamageMultiplierCalculator.Calculate(state);

                damageType = DamageType.Hit;

                // Assuming crit uses a 2-roll system as per this article:
                // https://wowwiki-archive.fandom.com/wiki/Attack_table#Ranged_attacks
                var critRoll = RandomGenerator.Roll(RollType.MeleeCrit);

                if (critRoll <= critChance)
                {
                    autoAttackDamage *= 2;
                    autoAttackDamage *= MeleeCritDamageMultiplierCalculator.Calculate(state);
                    damageType = DamageType.Crit;
                }
            }

            // TODO: Boss armor reduction

            // TODO: If we do these calcs earlier we can do just one roll instead of 2
            critChance *= (1 - missChance);
            var hitChance = 1 - missChance - critChance;

            DamageEvent = new DamageEvent(Timestamp, autoAttackDamage, damageType, missChance, critChance, hitChance);
            state.Events.Add(DamageEvent);
        }
    }
}
