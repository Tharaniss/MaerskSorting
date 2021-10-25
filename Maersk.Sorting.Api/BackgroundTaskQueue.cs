using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task<SortJob>>> _workItems =
       new ConcurrentQueue<Func<CancellationToken, Task<SortJob>>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, Task<SortJob>> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task<SortJob>>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem ?? (async token =>
            {
                await Task.Delay(5000);
                return new SortJob();
            });
        }
    }
}
