using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AsyncMethodLibrary;
using Kosdas.Models;

namespace Kosdas.Extensions
{
    public static class PriceExtension
    {
        public static Dictionary<DateTime, PriceBase> ToDictionary(this IEnumerable<PriceBase> source) => source.ToDictionary(x => x.Date, x => x);

        public static IReadOnlyList<T> ToList<T>(this IEnumerable<PriceBase> source, Func<PriceBase, T> selector) where T : PriceBase => source.Select(selector).ToImmutableList();

        public static Task<IReadOnlyList<T>> ToListAsync<T>(this IEnumerable<PriceBase> source, Func<PriceBase, T> selector) where T : PriceBase => Task.Run(() => ToList(source, selector));
    }
}