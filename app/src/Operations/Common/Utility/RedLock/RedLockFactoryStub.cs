using RedLockNet;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Utility.RedLock
{
    public class RedLockFactoryStub : IDistributedLockFactory
    {
        public IRedLock CreateLock(string resource, TimeSpan expiryTime)
            => CreateLockAsync(resource, expiryTime, TimeSpan.Zero, TimeSpan.FromMilliseconds(25))
                .GetAwaiter().GetResult();

        public IRedLock CreateLock(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, CancellationToken? cancellationToken = null)
            => CreateLockAsync(resource, expiryTime, waitTime, retryTime, cancellationToken)
                .GetAwaiter().GetResult();

        public Task<IRedLock> CreateLockAsync(string resource, TimeSpan expiryTime)
            => CreateLockAsync(resource, expiryTime, TimeSpan.Zero, TimeSpan.FromMilliseconds(25), default);

        public async Task<IRedLock> CreateLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, CancellationToken? cancellationToken = null)
        {
            var ct = cancellationToken ?? CancellationToken.None;
            if (retryTime <= TimeSpan.Zero)
                retryTime = TimeSpan.FromMilliseconds(25);

            var start = DateTimeOffset.UtcNow;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                if (RedLockStub.TryAcquire(resource, expiryTime, out var redLock))
                    return redLock;

                if (waitTime <= TimeSpan.Zero)
                    return RedLockStub.NotAcquired(resource);

                var elapsed = DateTimeOffset.UtcNow - start;
                if (elapsed >= waitTime)
                    return RedLockStub.NotAcquired(resource);

                var remaining = waitTime - elapsed;
                var delay = (retryTime < remaining) ? retryTime : remaining;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, ct);
            }
        }
    }
}
