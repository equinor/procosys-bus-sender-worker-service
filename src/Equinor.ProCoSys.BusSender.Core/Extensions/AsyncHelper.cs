using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Extensions;

public static class AsyncHelper
{
    /// <summary>
    /// Enumerates a collection in parallel and calls an async method on each item. Useful for making 
    /// parallel async calls, e.g. independent web requests when the degree of parallelism needs to be
    /// limited.
    /// </summary>
    public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> action) =>
        Task.WhenAll(Partitioner.Create(source).GetPartitions(degreeOfParallelism).Select(partition =>
            Task.Run(async () =>
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        await action(partition.Current);
                    }
                }
            })));
}
