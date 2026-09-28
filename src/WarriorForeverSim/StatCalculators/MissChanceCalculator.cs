using System;
using System.Linq;

namespace WarriorForeverSim
{
    public class MissChanceCalculator : BaseStatCalculator
    {
        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<MissChanceCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var bossDefense = state.Config.BossSettings.Defense;
            var weaponSkill = WeaponSkillCalculator.Calculate(weapon, state);
            var missChance = bossDefense - weaponSkill > 10
                ? 0.07 + ((bossDefense - weaponSkill - 10) * 0.004)
                : 0.05 + ((bossDefense - weaponSkill) * 0.001);

            missChance -= state.Config.Gear.GetAllGear().Sum(x => x.HitRating);
            missChance -= state.Config.Gear.GetAllEnchants().Sum(x => x.HitRating);

            missChance = Math.Max(missChance, 0.0);

            return missChance;
        }
    }
}
