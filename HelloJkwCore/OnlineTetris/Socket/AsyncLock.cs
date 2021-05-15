using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineTetris.Socket
{
    class AsyncLock
    {
        public struct Unlocker : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public Unlocker(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }

        private long _lockCounter = 0;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            Interlocked.Increment(ref _lockCounter);
            return new Unlocker(_semaphore);
        }

        public long LockCounter => Interlocked.Read(ref _lockCounter);

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}
