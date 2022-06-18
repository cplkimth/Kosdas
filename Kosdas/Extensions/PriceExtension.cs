using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kosdas.Models;

namespace Kosdas.Extensions
{
    public static class PriceExtension
    {
        public static Dictionary<DateTime, PriceRecord> ToDictionary(this IEnumerable<PriceRecord> source) => source.ToDictionary(x => x.Date, x => x);

        public static IReadOnlyList<T> ToList<T>(this IEnumerable<PriceRecord> source, Func<PriceRecord, T> selector) => source.Select(selector).ToImmutableList();
    }
}