namespace Kosdas.Models
{
    /// <summary>
    ///     가격 정보
    /// </summary>
    public record PriceRecord(DateTime Date, double Open, double High, double Low, double Close, double Volume)
    {
        public override string ToString() => $"{Date:d} {Open:N0} -> {Close:N0}";
    }
}