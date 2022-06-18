using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kosdas.Models;

namespace Kosdas.Extensions
{
    public static class PriceExtension
    {
        public static Dictionary<DateTime, Price> ToDictionary(this IEnumerable<Price> source) => source.ToDictionary(x => x.Date, x => x);
    }
}