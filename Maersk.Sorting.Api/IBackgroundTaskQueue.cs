using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task<SortJob>> workItem);
        Task<Func<CancellationToken, Task<SortJob>>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
