namespace WarriorForeverSim.Api
{
    public record SimulateRequest(Race Race, int BossLevel, BossType BossType, double FightLength, int Iterations);

    public record SimulateResponse(
        int Iterations,
        double AverageDps,
        double MinDps,
        double MaxDps,
        double AverageHits,
        double AverageCrits,
        double AverageMisses,
        IReadOnlyList<string> Warnings,
        IReadOnlyList<string> Errors);

    public record SimulationDefaults(Race Race, int BossLevel, BossType BossType, double FightLength, int Iterations);

    public record SimulationOptions(IReadOnlyList<Race> Races, IReadOnlyList<BossType> BossTypes, SimulationDefaults Defaults);
}
