using System;

namespace WarriorForeverSim
{
    public enum GearSource
    {
        Dungeon,
        MoltenCore,
        BlackWingLair,
        TempleOfAhnQiraj,
        RuinsOfAhnQiraj,
        Crafting,
        Reputation,
        AuctionHouse,
        Vendor,
        WorldBoss,
        ZulGurub,
        Naxxramas,
        Honor
    }

    public static class GearSourceExtensions
    {
        public static GearSource ToGearSource(this string value)
        {
            return value switch
            {
                "dungeon" => GearSource.Dungeon,
                "mc" => GearSource.MoltenCore,
                "bwl" => GearSource.BlackWingLair,
                "aq40" => GearSource.TempleOfAhnQiraj,
                "aq20" => GearSource.RuinsOfAhnQiraj,
                "crafting" => GearSource.Crafting,
                "rep" => GearSource.Reputation,
                "ah" => GearSource.AuctionHouse,
                "vendor" => GearSource.Vendor,
                "worldboss" => GearSource.WorldBoss,
                "zg" => GearSource.ZulGurub,
                "naxx" => GearSource.Naxxramas,
                "honor" => GearSource.Honor,
                _ => throw new ArgumentException($"Unrecognized gear source {value}"),
            };
        }
    }
}
