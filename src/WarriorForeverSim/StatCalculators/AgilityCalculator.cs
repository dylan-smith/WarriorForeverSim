namespace WarriorForeverSim
{
    public class AgilityCalculator : BaseStatCalculator
    {
        public static double Calculate(SimulationState state) => Calculate<AgilityCalculator>(state);

        protected override double InstanceCalculate(SimulationState state)
        {
            var agility = state.Config.PlayerSettings.Agility;
            agility += state.Config.Gear.GetStatTotal(x => x.Agility);

            if (state.Config.Buffs.Contains(Buff.MarkOfTheWild))
            {
                agility += 14;
            }

            if (state.Config.Buffs.Contains(Buff.ImprovedMarkOfTheWild))
            {
                agility += (14 * 1.35).Floor();
            }

            if (state.Config.Buffs.Contains(Buff.GraceOfAirTotem))
            {
                agility += 77;
            }

            if (state.Config.Buffs.Contains(Buff.ImprovedGraceOfAirTotem))
            {
                agility += (77 * 1.15).Floor();
            }

            if (state.Config.Buffs.Contains(Buff.GrilledMudfish))
            {
                agility += 20;
            }

            if (state.Config.Buffs.Contains(Buff.ElixirOfMajorAgility))
            {
                agility += 35;
            }

            if (state.Config.Buffs.Contains(Buff.ScrollOfAgilityV))
            {
                agility += 20;
            }

            if (state.Config.Talents.TryGetValue(Talent.LightningReflexes, out var lightningReflexesRank))
            {
                agility *= 1 + (lightningReflexesRank * 0.03);
                agility = agility.Floor();
            }

            if (state.Config.Talents.TryGetValue(Talent.CombatExperience, out var combatExperienceRank))
            {
                agility *= 1 + (0.01 * combatExperienceRank);
                agility = agility.Floor();
            }

            if (state.Config.Buffs.Contains(Buff.BlessingOfKings))
            {
                agility *= 1.1;
                agility = agility.Floor();
            }

            return agility;
        }
    }
}
