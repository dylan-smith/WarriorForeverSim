namespace WarriorForeverSim
{
    // Crit from talents, gear, buffs and consumables. Unlike agility crit, this is what the
    // aura crit suppression against +3 level targets removes.
    // https://github.com/magey/classic-warrior/wiki/Crit-aura-suppression
    public class AuraCritCalculator : BaseStatCalculator
    {
        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<AuraCritCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var critChance = state.Config.Gear.GetStatTotal(x => x.CritRating) / 100; // gear lists crit as a percent
            critChance += state.Config.Gear.GetStatTotal(x => x.MeleeCritRating) / 100;
            critChance += GetTalentRank(state, Talent.Cruelty) * 0.01;

            if (weapon.WeaponType is WeaponType.OneHandedAxe or WeaponType.TwoHandedAxe or WeaponType.Polearm)
            {
                critChance += GetTalentRank(state, Talent.Weaponmaster) * 0.01;
            }

            return critChance;
        }

        private static int GetTalentRank(SimulationState state, Talent talent) => state.Config.Talents.TryGetValue(talent, out var rank) ? rank : 0;
    }
}
