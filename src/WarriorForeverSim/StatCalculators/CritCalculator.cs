using System;

namespace WarriorForeverSim
{
    public class CritCalculator : BaseStatCalculator
    {
        private const double BaseCritChance = 0.0; // warriors have no base crit in Classic
        private const double AgilityPerCritPercent = 20; // level 60 value; other levels aren't modelled
        private const double AuraCritSuppression = 0.018;

        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<CritCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var auraCrit = GetAuraCrit(weapon, state);

            var critChance = BaseCritChance;
            critChance += AgilityCalculator.Calculate(state) / AgilityPerCritPercent / 100;
            critChance += auraCrit;
            critChance += GetWeaponSkillModifier(weapon, state);
            critChance -= GetAuraCritSuppression(auraCrit, state);

            return critChance.Normalize();
        }

        // Crit from talents, gear, buffs and consumables. Unlike agility crit, this is what the
        // aura crit suppression against +3 level targets removes.
        private static double GetAuraCrit(GearItem weapon, SimulationState state)
        {
            var critChance = state.Config.Gear.GetStatTotal(x => x.CritRating) / 100; // gear lists crit as a percent
            critChance += GetTalentRank(state, Talent.Cruelty) * 0.01;

            if (weapon.WeaponType is WeaponType.OneHandedAxe or WeaponType.TwoHandedAxe or WeaponType.Polearm)
            {
                critChance += GetTalentRank(state, Talent.Weaponmaster) * 0.01;
            }

            return critChance;
        }

        // Crit changes by 0.2% per point of weapon skill below the target's defense, or 0.04% per point
        // above it, and against mobs no more than level * 5 weapon skill counts toward crit.
        // https://github.com/magey/classic-warrior/wiki/Attack-table
        private static double GetWeaponSkillModifier(GearItem weapon, SimulationState state)
        {
            var weaponSkill = state.Config.PlayerSettings.Level * 5;
            var skillDiff = weaponSkill - state.Config.BossSettings.Defense;

            return skillDiff < 0 ? skillDiff * 0.002 : skillDiff * 0.0004;
        }

        // Against targets 3 levels higher, a flat 1.8% is removed from crit gained through auras
        // (talents, gear, buffs), but never from agility crit.
        // https://github.com/magey/classic-warrior/wiki/Crit-aura-suppression
        private static double GetAuraCritSuppression(double auraCrit, SimulationState state)
        {
            if (state.Config.BossSettings.Level - state.Config.PlayerSettings.Level < 3)
            {
                return 0.0;
            }

            return Math.Min(auraCrit, AuraCritSuppression);
        }

        private static int GetTalentRank(SimulationState state, Talent talent) => state.Config.Talents.TryGetValue(talent, out var rank) ? rank : 0;
    }
}
