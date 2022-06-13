namespace Kosdas.Models
{
    /// <summary>
    ///     가격 정보
    /// </summary>
    public class Price
    {
        internal Price(DateTime date, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        /// <summary>
        ///     일자
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        ///     시가
        /// </summary>
        public decimal Open { get; }

        /// <summary>
        ///     고가
        /// </summary>
        public decimal High { get; }

        /// <summary>
        ///     저가
        /// </summary>
        public decimal Low { get; }

        /// <summary>
        ///     종가
        /// </summary>
        public decimal Close { get; }

        /// <summary>
        ///     거래량
        /// </summary>
        public decimal Volume { get; }

        public override string ToString() => $"{Date:d} {Open:N0} -> {Close:N0}";
    }
}