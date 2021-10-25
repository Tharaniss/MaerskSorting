using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        public IBackgroundTaskQueue _queue { get; }
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SortJobs _sortJob;

        public SortController(ISortJobProcessor sortJobProcessor, IBackgroundTaskQueue queue, IServiceScopeFactory serviceScopeFactory, SortJobs sortJobs)
        {
            _sortJobProcessor = sortJobProcessor;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
            _sortJob = sortJobs;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost]
        public ActionResult<SortJob> EnqueueJob(int[] values)
        {
            var guidId = Guid.NewGuid();

            var pendingJob = _sortJob.Add(guidId.ToString(), new SortJob(
                id: guidId,
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null)
            );

            _queue.QueueBackgroundWorkItem(async token =>
            {
                return await _sortJobProcessor.Process(pendingJob);
            });

            return Ok(pendingJob);
        }

        [HttpGet]
        public ActionResult<SortJob[]> GetJobs()
        {
            // TODO: Should return all jobs that have been enqueued (both pending and completed).
            return _sortJob.List();
        }

        [HttpGet("{jobId}")]
        public ActionResult<SortJob> GetJob(Guid jobId)
        {
            // TODO: Should return a specific job by ID.
            return _sortJob.Get(jobId.ToString());
        }
    }
}
