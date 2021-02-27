using System;
using System.Collections.Generic;
using System.Linq;

namespace Kosdas
{
    public static class PriceExtension
    {
        public static Dictionary<DateTime, Price> ToDictionary(this IEnumerable<Price> source) => source.ToDictionary(x => x.Date, x => x);
    }
}