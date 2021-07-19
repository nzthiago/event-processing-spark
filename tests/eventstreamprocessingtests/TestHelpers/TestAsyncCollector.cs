using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamProcessingTests.TestHelpers
{
    class TestAsyncCollector<T> : IAsyncCollector<T>
    {
        public List<T> Values { get; set; }

        public TestAsyncCollector()
        {
            Values = new List<T>();
        }

        public Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Values.Add(item));
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
