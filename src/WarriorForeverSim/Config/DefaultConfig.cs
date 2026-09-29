namespace WarriorForeverSim
{
    public class DefaultConfig : SimulationConfig
    {
        public DefaultConfig()
        {
            SimulationSettings.FightLength = 60.0;

            Gear.Head = GearItemFactory.LoadHead("Lionheart Helm");
            Gear.Neck = GearItemFactory.LoadNeck("Choker of the Fire Lord");
            Gear.Shoulder = GearItemFactory.LoadShoulder("Drake Talon Pauldrons");
            Gear.Back = GearItemFactory.LoadBack("Cloak of the Fallen God");
            Gear.Chest = GearItemFactory.LoadChest("Dreadnaught Breastplate");
            Gear.Wrist = GearItemFactory.LoadWrist("Hive Defiler Wristguards");
            Gear.MainHand = GearItemFactory.LoadMainHand("Thunderfury, Blessed Blade of the Windseeker");
            Gear.OffHand = GearItemFactory.LoadOffHand("Maladath, Runed Blade of the Black Flight");
            Gear.Ranged = GearItemFactory.LoadRanged("Striker's Mark");
            Gear.Hands = GearItemFactory.LoadHands("Edgemaster's Handguards");
            Gear.Waist = GearItemFactory.LoadWaist("Onslaught Girdle");
            Gear.Legs = GearItemFactory.LoadLegs("Cloudkeeper Legplates");
            Gear.Feet = GearItemFactory.LoadFeet("Chromatic Boots");
            Gear.Finger1 = GearItemFactory.LoadFinger("Band of Accuria");
            Gear.Finger2 = GearItemFactory.LoadFinger("Quick Strike Ring");
            Gear.Trinket1 = GearItemFactory.LoadTrinket("Diamond Flask");
            Gear.Trinket2 = GearItemFactory.LoadTrinket("Drake Fang Talisman");

            // Arms (17)
            Talents[Talent.ImprovedHeroicStrike] = 3;
            Talents[Talent.ImprovedRend] = 3;
            Talents[Talent.ImprovedTacticalMastery] = 5;
            Talents[Talent.AngerManagement] = 1;
            Talents[Talent.DeepWounds] = 3;
            Talents[Talent.Impale] = 2;

            // Fury (34)
            Talents[Talent.Cruelty] = 5;
            Talents[Talent.UnbridledWrath] = 5;
            Talents[Talent.IronWill] = 1;
            Talents[Talent.ImprovedCleave] = 3;
            Talents[Talent.PiercingHowl] = 1;
            Talents[Talent.DualWieldSpecialization] = 5;
            Talents[Talent.RagingBlows] = 1;
            Talents[Talent.Enrage] = 5;
            Talents[Talent.ImprovedExecute] = 1;
            Talents[Talent.DeathWish] = 1;
            Talents[Talent.Flurry] = 5;
            Talents[Talent.Bloodthirst] = 1;

            BossSettings.Level = 63;
            BossSettings.BossType = BossType.Demon;

            PlayerSettings.Race = Race.Human;
            PlayerSettings.Level = 60;
        }
    }
}
