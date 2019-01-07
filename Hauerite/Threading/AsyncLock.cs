using System;
using System.Threading.Tasks;

namespace Haurite.Threading
{
    /// <summary>
    /// http://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx
    /// </summary>
    public sealed class AsyncLock
    {
        private readonly System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        private readonly Task<IDisposable> releaser;

        public AsyncLock()
        {
            releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = semaphore.WaitAsync();
            return wait.IsCompleted ?
                    releaser :
                    wait.ContinueWith(
                      (_, state) => (IDisposable)state,
                      releaser.Result,
                      System.Threading.CancellationToken.None,
                      TaskContinuationOptions.ExecuteSynchronously,
                      TaskScheduler.Default
                    );
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.semaphore.Release(); }
        }
    }
}
