namespace Kosdas.Models
{
    /// <summary>
    ///     가격 정보
    /// </summary>
    public record PriceRecord(DateTime Date, decimal Open, decimal High, decimal Low, decimal Close, decimal Volume)
    {
        public override string ToString() => $"{Date:d} {Open:N0} -> {Close:N0}";
    }
}