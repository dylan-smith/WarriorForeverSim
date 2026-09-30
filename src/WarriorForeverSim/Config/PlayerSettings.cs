using System;

namespace WarriorForeverSim
{
    public class PlayerSettings
    {
        public Race Race { get; set; }
        public int Level { get; set; }

        // TODO: Calc stat values modified by level (can't find a good source of info)

        public double Strength
        {
            get
            {
                return Race switch
                {
                    Race.Human => 120,
                    Race.Dwarf => 122,
                    Race.Gnome => 115,
                    Race.NightElf => 117,
                    Race.Orc => 123,
                    Race.Tauren => 125,
                    Race.Troll => 121,
                    Race.Undead => 119,
                    _ => throw new Exception("Race not set"),// TODO: Richer exceptions
                };
            }
        }

        public double Agility
        {
            get
            {
                return Race switch
                {
                    Race.Human => 80,
                    Race.Dwarf => 76,
                    Race.Gnome => 83,
                    Race.NightElf => 85,
                    Race.Orc => 77,
                    Race.Tauren => 75,
                    Race.Troll => 82,
                    Race.Undead => 78,
                    _ => throw new Exception("Race not set"),// TODO: Richer exceptions
                };
            }
        }

        public double Stamina
        {
            get
            {
                return Race switch
                {
                    Race.Human => 110,
                    Race.Dwarf => 113,
                    Race.Gnome => 109,
                    Race.NightElf => 109,
                    Race.Orc => 112,
                    Race.Tauren => 112,
                    Race.Troll => 111,
                    Race.Undead => 111,
                    _ => throw new Exception("Race not set"),// TODO: Richer exceptions
                };
            }
        }

        public double Intellect
        {
            get
            {
                return Race switch
                {
                    Race.Human => 30,
                    Race.Dwarf => 29,
                    Race.Gnome => 35,
                    Race.NightElf => 30,
                    Race.Orc => 27,
                    Race.Tauren => 25,
                    Race.Troll => 26,
                    Race.Undead => 28,
                    _ => throw new Exception("Race not set"),// TODO: Richer exceptions
                };
            }
        }
    }
}
