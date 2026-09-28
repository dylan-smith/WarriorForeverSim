using System.Linq;

namespace WarriorForeverSim
{
    public class DefaultConfig : SimulationConfig
    {
        public DefaultConfig()
        {
            SimulationSettings.FightLength = 60.0;

            Gear.Head = GearItemFactory.LoadHead("Beast Lord Helm");
            Gear.Head.Enchant = GearItemFactory.LoadHeadEnchant("Glyph of Ferocity");

            Gear.Neck = GearItemFactory.LoadNeck("Worgen Claw Necklace");

            Gear.Shoulder = GearItemFactory.LoadShoulder("Beast Lord Mantle");
            Gear.Shoulder.Enchant = GearItemFactory.LoadShoulderEnchant("Greater Inscription of the Blade");

            Gear.Back = GearItemFactory.LoadBack("Blood Knight War Cloak");
            Gear.Back.Enchant = GearItemFactory.LoadBackEnchant("Greater Agility");

            Gear.Chest = GearItemFactory.LoadChest("Beast Lord Cuirass");
            Gear.Chest.Enchant = GearItemFactory.LoadChestEnchant("Exceptional Stats");

            Gear.Wrist = GearItemFactory.LoadWrist("Nightfall Wristguards");
            Gear.Wrist.Enchant = GearItemFactory.LoadWristEnchant("Assault");

            Gear.Hands = GearItemFactory.LoadHands("Beast Lord Handguards");
            Gear.Hands.Enchant = GearItemFactory.LoadHandEnchant("Superior Agility");

            Gear.Waist = GearItemFactory.LoadWaist("Girdle of Treachery");

            Gear.Legs = GearItemFactory.LoadLegs("Skulker's Greaves");
            Gear.Legs.Enchant = GearItemFactory.LoadLegEnchant("Nethercobra Leg Armor");

            Gear.Feet = GearItemFactory.LoadFeet("Edgewalker Longboots");
            Gear.Feet.Enchant = GearItemFactory.LoadFeetEnchant("Dexterity");

            Gear.Finger1 = GearItemFactory.LoadFinger("Ring of the Recalcitrant");
            Gear.Finger2 = GearItemFactory.LoadFinger("Garona's Signet Ring");
            Gear.Trinket1 = GearItemFactory.LoadTrinket("Bloodlust Brooch");
            Gear.Trinket2 = GearItemFactory.LoadTrinket("Dragonspine Trophy");

            Gear.MainHand = GearItemFactory.LoadMainHand("Big Bad Wolf's Paw");
            Gear.MainHand.Enchant = GearItemFactory.LoadOneHandEnchant("Greater Agility");

            Gear.OffHand = GearItemFactory.LoadOffHand("Blade of the Unrequited");
            Gear.OffHand.Enchant = GearItemFactory.LoadOneHandEnchant("Greater Agility");

            Gear.Ranged = GearItemFactory.LoadRanged("Sunfury Bow of the Phoenix");
            Gear.Ranged.Enchant = GearItemFactory.LoadRangedEnchant("Stabilized Eternium Scope");

            Gear.Ammo = GearItemFactory.LoadAmmo("Warden's Arrow");
            Gear.Quiver = GearItemFactory.LoadQuiver("Worg Hide Quiver");

            BossSettings.Level = 73;
            BossSettings.BossType = BossType.Demon;

            PlayerSettings.Race = Race.Dwarf;
            PlayerSettings.Level = 70;

            //Buffs.Add(Buff.BlessingOfKings);
            //Buffs.Add(Buff.ImprovedBlessingOfMight);
            //Buffs.Add(Buff.LeaderOfThePack);
            //Buffs.Add(Buff.ImprovedGraceOfAirTotem);
            //Buffs.Add(Buff.ImprovedStrengthOfEarthTotem);
            //Buffs.Add(Buff.ArcaneBrilliance);
            //Buffs.Add(Buff.ImprovedMarkOfTheWild);
            //Buffs.Add(Buff.GrilledMudfish);
            //Buffs.Add(Buff.ElixirOfMajorAgility);
            //Buffs.Add(Buff.ScrollOfAgilityV);
            //Buffs.Add(Buff.ScrollOfStrengthV);

            // Marksmanship (20)
            Talents.Add(Talent.LethalShots, 5);
            Talents.Add(Talent.ImprovedHuntersMark, 5);
            Talents.Add(Talent.GoForTheThroat, 2);
            Talents.Add(Talent.AimedShot, 1);
            Talents.Add(Talent.RapidKilling, 2);
            Talents.Add(Talent.MortalShots, 5);

            // Survival (41)
            Talents.Add(Talent.MonsterSlaying, 3);
            Talents.Add(Talent.HumanoidSlaying, 3);
            Talents.Add(Talent.HawkEye, 3);
            Talents.Add(Talent.SavageStrikes, 2);
            Talents.Add(Talent.CleverTraps, 2);
            Talents.Add(Talent.Survivalist, 2);
            Talents.Add(Talent.Surefooted, 3);
            Talents.Add(Talent.ImprovedFeignDeath, 2);
            Talents.Add(Talent.SurvivalInstincts, 2);
            Talents.Add(Talent.KillerInstinct, 3);
            Talents.Add(Talent.LightningReflexes, 5);
            Talents.Add(Talent.ThrillOfTheHunt, 2);
            Talents.Add(Talent.ExposeWeakness, 3);
            Talents.Add(Talent.MasterTactician, 5);
            Talents.Add(Talent.Readiness, 1);
        }
    }
}
