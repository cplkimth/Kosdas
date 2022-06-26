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
        public static Dictionary<DateTime, Price> ToDictionary(this IEnumerable<Price> source) => source.ToDictionary(x => x.Date, x => x);

        public static IReadOnlyList<T> ToList<T>(this IEnumerable<Price> source, Func<Price, T> selector) where T : Price => source.Select(selector).ToImmutableList();

        public static Task<IReadOnlyList<T>> ToListAsync<T>(this IEnumerable<Price> source, Func<Price, T> selector) where T : Price => Task.Run(() => ToList(source, selector));

        public static double RateOf(this double sellValue, double buyValue)
        {
            if (buyValue == 0)
                return 0;

            return (sellValue - buyValue) / buyValue;
        }
    }
}