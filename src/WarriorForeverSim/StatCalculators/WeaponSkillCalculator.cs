namespace WarriorForeverSim
{
    public class WeaponSkillCalculator : BaseStatCalculator
    {
        public static double Calculate(GearItem weapon, SimulationState state) => Calculate<WeaponSkillCalculator>(weapon, state);

        protected override double InstanceCalculate(GearItem weapon, SimulationState state)
        {
            var skill = state.Config.PlayerSettings.Level * 5;
            var weaponType = weapon.WeaponType;

            skill += (int)state.Config.Gear.GetStatTotal(x => x.WeaponSkill.TryGetValue(weaponType, out var skill) ? skill : 0.0);

            return skill;
        }
    }
}
