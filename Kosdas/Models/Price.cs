namespace Kosdas.Models
{
    /// <summary>
    ///     가격 정보
    /// </summary>
    public class Price
    {
        /// <summary>
        ///     가격 정보
        /// </summary>
        public Price(DateTime Date, double Open, double High, double Low, double Close, double Volume)
        {
            this.Date = Date;
            this.Open = Open;
            this.High = High;
            this.Low = Low;
            this.Close = Close;
            this.Volume = Volume;
        }

        public DateTime Date { get; init; }
        
        public double Open { get; init; }
        
        public double High { get; init; }
        
        public double Low { get; init; }
        
        public double Close { get; init; }
        
        public double Volume { get; init; }

        public override string ToString() => $"{Date:d} {Open:N0} -> {Close:N0}";
    }
}