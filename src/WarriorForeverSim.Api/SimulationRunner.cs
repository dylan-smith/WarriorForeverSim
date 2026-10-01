namespace WarriorForeverSim.Api
{
    public static class SimulationRunner
    {
        public const int MaxIterations = 1000;
        public const double MaxFightLength = 600.0;

        // Races whose base stats are not implemented yet (PlayerSettings throws for them).
        private static readonly Race[] _unsupportedRaces = [Race.NotSet, Race.WindShaperSkyborne, Race.HighOrderSkyborne];

        // RandomGenerator and the stat calculator cache are process-wide singletons, so
        // simulations must not run concurrently.
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public static SimulationOptions GetOptions()
        {
            var defaults = new DefaultConfig();

            return new SimulationOptions(
                Enum.GetValues<Race>().Except(_unsupportedRaces).ToList(),
                Enum.GetValues<BossType>(),
                new SimulationDefaults(
                    defaults.PlayerSettings.Race,
                    defaults.BossSettings.Level,
                    defaults.BossSettings.BossType,
                    defaults.SimulationSettings.FightLength,
                    100));
        }

        public static Dictionary<string, string[]> Validate(SimulateRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request.Iterations is < 1 or > MaxIterations)
            {
                errors[nameof(request.Iterations)] = [$"Iterations must be between 1 and {MaxIterations}."];
            }

            if (request.FightLength is < 1 or > MaxFightLength)
            {
                errors[nameof(request.FightLength)] = [$"Fight length must be between 1 and {MaxFightLength} seconds."];
            }

            if (request.BossLevel is < 1 or > 63)
            {
                errors[nameof(request.BossLevel)] = ["Boss level must be between 1 and 63."];
            }

            if (_unsupportedRaces.Contains(request.Race))
            {
                errors[nameof(request.Race)] = [$"Race {request.Race} is not supported yet."];
            }

            return errors;
        }

        public static async Task<SimulateResponse> RunAsync(SimulateRequest request, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);

            try
            {
                var reports = new List<SimulationReport>();
                IReadOnlyList<SimulationLogEntry> firstRunLog = [];

                for (var i = 0; i < request.Iterations; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var state = new Simulation(BuildConfig(request)).Run();
                    var report = SimulationReport.FromState(state);

                    if (report.Errors.Count > 0)
                    {
                        return new SimulateResponse(0, 0, 0, 0, 0, 0, 0, 0, 0, report.Warnings, report.Errors, []);
                    }

                    if (i == 0)
                    {
                        firstRunLog = SimulationLogEntry.FromState(state);
                    }

                    reports.Add(report);
                }

                return new SimulateResponse(
                    reports.Count,
                    reports.Average(r => r.Dps),
                    reports.Min(r => r.Dps),
                    reports.Max(r => r.Dps),
                    reports.Average(r => r.Hits),
                    reports.Average(r => r.Crits),
                    reports.Average(r => r.Misses),
                    reports.Average(r => r.Dodges),
                    reports.Average(r => r.Glancings),
                    reports[0].Warnings,
                    reports[0].Errors,
                    firstRunLog);
            }
            finally
            {
                _lock.Release();
            }
        }

        private static DefaultConfig BuildConfig(SimulateRequest request)
        {
            var config = new DefaultConfig();

            config.PlayerSettings.Race = request.Race;
            config.BossSettings.Level = request.BossLevel;
            config.BossSettings.BossType = request.BossType;
            config.SimulationSettings.FightLength = request.FightLength;

            return config;
        }
    }
}
