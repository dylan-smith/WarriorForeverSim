namespace WarriorForeverSim
{
    public class DefaultConfig : SimulationConfig
    {
        public DefaultConfig()
        {
            SimulationSettings.FightLength = 60.0;

            Gear.MainHand = GearItemFactory.LoadMainHand("Big Bad Wolf's Paw");

            BossSettings.Level = 63;
            BossSettings.BossType = BossType.Demon;

            PlayerSettings.Race = Race.Human;
            PlayerSettings.Level = 60;
        }
    }
}
