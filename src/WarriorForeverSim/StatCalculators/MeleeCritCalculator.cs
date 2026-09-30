using System;

namespace WarriorForeverSim
{
    public class MeleeCritCalculator : BaseStatCalculator
    {
        private const double AuraCritSuppression = 0.018;

        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<MeleeCritCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var critChance = CritCalculator.Calculate(weapon, state);
            critChance += GetWeaponSkillModifier(weapon, state);
            critChance -= GetAuraCritSuppression(weapon, state);

            critChance = Math.Max(critChance, 0);
            critChance = Math.Min(critChance, 1.0);

            return critChance;
        }

        // Crit changes by 0.2% per point of weapon skill below the target's defense, or 0.04% per point
        // above it, and against mobs no more than level * 5 weapon skill counts toward crit.
        // https://github.com/magey/classic-warrior/wiki/Attack-table
        private static double GetWeaponSkillModifier(GearItem weapon, SimulationState state)
        {
            var weaponSkill = Math.Min(WeaponSkillCalculator.Calculate(weapon, state), state.Config.PlayerSettings.Level * 5);
            var skillDiff = weaponSkill - state.Config.BossSettings.Defense;

            return skillDiff < 0 ? skillDiff * 0.002 : skillDiff * 0.0004;
        }

        // Against targets 3 levels higher, a flat 1.8% is removed from crit gained through auras
        // (talents, gear, buffs), but never from agility crit.
        // https://github.com/magey/classic-warrior/wiki/Crit-aura-suppression
        private static double GetAuraCritSuppression(GearItem weapon, SimulationState state)
        {
            if (state.Config.BossSettings.Level - state.Config.PlayerSettings.Level < 3)
            {
                return 0.0;
            }

            return Math.Min(AuraCritCalculator.Calculate(weapon, state), AuraCritSuppression);
        }
    }
}
