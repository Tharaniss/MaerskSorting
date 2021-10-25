using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobs
    {
        private readonly ConcurrentDictionary<string, SortJob> _sortJobInfo
           = new ConcurrentDictionary<string, SortJob>();

        public ConcurrentDictionary<string, SortJob> SortJobInfo => _sortJobInfo;

        public SortJob Get(string guidId) {
            SortJob? getJob;
            _sortJobInfo.TryGetValue(guidId.ToString(), out getJob);
            return (getJob ?? new SortJob());
        }

        public SortJob Add(string guidId, SortJob pendingJob)
        {
            _sortJobInfo.TryAdd(guidId.ToString(), pendingJob);
            SortJob? getJob;
            _sortJobInfo.TryGetValue(guidId.ToString(), out getJob);
            return (getJob ?? new SortJob());
        }

        public SortJob Update(string guidId, SortJob oldJob, SortJob newJob)
        {
            _sortJobInfo.TryUpdate(guidId.ToString(), newJob, oldJob);
            SortJob? getJob;
            _sortJobInfo.TryGetValue(guidId.ToString(), out getJob);
            return (getJob ?? new SortJob());
        }

        public SortJob[] List()
        {
            return _sortJobInfo.Select(x => x.Value).ToArray();
        }
    }
}
