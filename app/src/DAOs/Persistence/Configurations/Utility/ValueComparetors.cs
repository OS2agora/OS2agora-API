using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Agora.DAOs.Persistence.Configurations.Utility
{
    internal static class ValueComparetors
    {
        internal static ValueComparer<string[]> StringArrayComparetor =
            new ValueComparer<string[]>(
                (v1, v2) => v1.ToList().SequenceEqual(v2),
                v => v.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                v => v);
    }
}
