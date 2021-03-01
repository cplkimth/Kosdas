using System;

namespace Kosdas.Extensions
{
    public static class DateTimeExtension
    {
        private const int V = 86400;

        private static readonly DateTime Orign = new DateTime(1970, 1, 1);

        public static int ToInt(this DateTime date)
        {
            return (int) ((date - Orign).TotalDays * V);
        }
    }
}