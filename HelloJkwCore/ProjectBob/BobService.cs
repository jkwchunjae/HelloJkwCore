using Microsoft.Extensions.DependencyInjection;

namespace ProjectBob;

public partial class BobService : IBobService
{
    private readonly IFileSystem _fileSystem;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly BobMenuAvailabilityAnalyzer _availabilityAnalyzer = new();
    private readonly BobWeeklyMealPlanner _weeklyMealPlanner = new();

    public BobService([FromKeyedServices(nameof(BobService))] IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
}
