using System;

namespace WarriorForeverSim
{
    public class MissChanceCalculator : BaseStatCalculator
    {
        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<MissChanceCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            // TODO: This is main hand white hits only, yellow attacks have a different calc (and I think off hand too)
            var bossDefense = state.Config.BossSettings.Defense;
            var weaponSkill = WeaponSkillCalculator.Calculate(weapon, state);
            var defenseSkillDiff = bossDefense - weaponSkill;

            var baseMissChance = state.Config.Gear.IsDualWielding() ? 0.24 : 0.05;
            var hitSuppression = defenseSkillDiff > 10
                ? defenseSkillDiff * 0.002
                : defenseSkillDiff * 0.001;

            var missChance = baseMissChance + hitSuppression;

            var hitChance = state.Config.Gear.GetStatTotal(x => x.HitRating) / 100;

            if (defenseSkillDiff > 10)
            {
                hitChance = Math.Max(hitChance - 0.01, 0); // first 1% of hit is ignored if weapon skill is 10+ below boss defense
            }

            missChance -= hitChance;

            return missChance.Normalize();
        }
    }
}

/*
bossDefense = 315;
weaponSkill = 300;
defenseSkillDiff = bossDefense - weaponSkill = 15;
baseMissChance = 0.05 (for single wielding)
hitSuppression = defenseSkillDiff > 10 ? defenseSkillDiff * 0.002 = 15 * 0.002 = 0.03
missChance = baseMissChance + hitSuppression = 0.05 + 0.03 = 0.08
hitChance = 0.07 (from gear) - 0.01 = 0.06

missChance = 0.08 - 0.06 = 0.02 (2% miss chance)

*/

/*
bossDefense = 315;
weaponSkill = 305;
defenseSkillDiff = bossDefense - weaponSkill = 10;
baseMissChance = 0.05 (for single wielding)
hitSuppression = defenseSkillDiff <= 10 ? defenseSkillDiff * 0.001 = 10 * 0.001 = 0.01
missChance = baseMissChance + hitSuppression = 0.05 + 0.01 = 0.06
hitChance = 0.07 (from gear)

missChance = 0.06 - 0.07 = -0.01 (0% miss chance)

*/
