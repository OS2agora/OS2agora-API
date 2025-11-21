using RedLockNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Utility.RedLock
{
    public class RedLockStub : IRedLock
    {
        private class Entry
        {
            public Entry(string lockId, DateTimeOffset expiresAt)
            {
                LockId = lockId;
                ExpiresAt = expiresAt;
            }

            public string LockId { get; }
            public DateTimeOffset ExpiresAt { get; }
        }

        private static readonly ConcurrentDictionary<string, Entry> _locks = new ConcurrentDictionary<string, Entry>();
        private bool _disposed;

        private RedLockStub(string resource, bool acquired, string lockId, DateTime createdAtUtc, TimeSpan validity, RedLockStatus status)
        {
            Resource = resource;
            IsAcquired = acquired;
            LockId = lockId;
            CreatedAt = createdAtUtc;
            ValidityTime = validity;
            Status = status;
            ExpirationTime = acquired ? CreatedAt.Add(validity) : (DateTime?)null;

            InstanceSummary = new RedLockInstanceSummary(); // neutral summary
            ExtendCount = 0;
            ExtendErrors = Array.Empty<Exception>();
            UnlockErrors = Array.Empty<Exception>();
            DisposeErrors = Array.Empty<Exception>();
        }

        internal static bool TryAcquire(string resource, TimeSpan expiry, out RedLockStub redLock)
        {
            var now = DateTimeOffset.UtcNow;

            Entry existing;
            if (_locks.TryGetValue(resource, out existing) && existing.ExpiresAt <= now)
            {
                _locks.TryRemove(resource, out _);
            }

            var entry = new Entry(Guid.NewGuid().ToString("N"), now.Add(expiry));
            if (_locks.TryAdd(resource, entry))
            {
                redLock = new RedLockStub(resource, true, entry.LockId, now.UtcDateTime, expiry, RedLockStatus.Acquired);
                return true;
            }

            redLock = new RedLockStub(resource, false, string.Empty, now.UtcDateTime, TimeSpan.Zero, RedLockStatus.Unlocked);
            return false;
        }

        internal static RedLockStub NotAcquired(string resource)
        {
            return new RedLockStub(resource, false, string.Empty, DateTime.UtcNow, TimeSpan.Zero, RedLockStatus.Unlocked);
        }

        public static async Task<IRedLock> CreateAsync()
        {
            await Task.Yield();
            return new RedLockStub("stub", true, Guid.NewGuid().ToString("N"), DateTime.UtcNow, TimeSpan.FromSeconds(30), RedLockStatus.Acquired);
        }

        public string Resource { get; }
        public string LockId { get; }
        public bool IsAcquired { get; }
        public RedLockStatus Status { get; }
        public RedLockInstanceSummary InstanceSummary { get; }
        public int ExtendCount { get; private set; }

        public TimeSpan ValidityTime { get; }
        public DateTime CreatedAt { get; }
        public DateTime? ExpirationTime { get; }
        public IList<Exception> ExtendErrors { get; }
        public IList<Exception> UnlockErrors { get; }
        public IList<Exception> DisposeErrors { get; }

        public bool ExtendExpiry(TimeSpan additionalValidity)
        {
            if (!IsAcquired || _disposed) return false;

            Entry entry;
            if (_locks.TryGetValue(Resource, out entry) && entry.LockId == LockId)
            {
                var newExp = DateTimeOffset.UtcNow.Add(additionalValidity);
                _locks[Resource] = new Entry(entry.LockId, newExp);
                ExtendCount++;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (!IsAcquired) return;

            Entry entry;
            if (_locks.TryGetValue(Resource, out entry) && entry.LockId == LockId)
            {
                _locks.TryRemove(Resource, out _);
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return new ValueTask();
        }
    }
}
