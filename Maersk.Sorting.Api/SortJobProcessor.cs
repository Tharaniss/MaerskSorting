using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;
        private readonly SortJobs _sortJob;
        public SortJobProcessor(ILogger<SortJobProcessor> logger, SortJobs sortJobs)
        {
            _logger = logger;
            _sortJob = sortJobs;
        }

        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(5000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            return _sortJob.Update(job.Id.ToString(), job, new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output)
            );
        }
    }
}
