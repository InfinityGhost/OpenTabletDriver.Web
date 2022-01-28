using System;
using System.Threading.Tasks;

#nullable enable

namespace OpenTabletDriver.Web.Core
{
    public class CachedTask<TResult>
    {
        public CachedTask(Func<Task<TResult>> task, TimeSpan invalidateTime)
        {
            Task = task;
            InvalidateTime = invalidateTime;
        }

        private TResult? cachedResult;
        private DateTime lastRequest;

        public Func<Task<TResult>> Task { get; }
        public TimeSpan InvalidateTime { set; get; }

        public async Task<TResult> Get()
        {
            bool invalidate = DateTime.Now - lastRequest >= InvalidateTime;
            lastRequest = DateTime.Now;

            if (cachedResult == null || invalidate)
                return cachedResult = await Task();
            else
                return cachedResult;
        }

        public static implicit operator Task<TResult>(CachedTask<TResult> task)
        {
            return task.Get();
        }
    }
}