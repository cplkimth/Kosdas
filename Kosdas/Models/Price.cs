namespace Kosdas.Models;

/// <summary>
///     가격 정보
/// </summary>
public class Price : MinutePrice
{
    /// <summary>
    ///     가격 정보
    /// </summary>
    public Price(DateTime at, double open, double high, double low, double close, double volume) : base(at, close, volume)
    {
        Open = open;
        High = high;
        Low = low;
    }

    public double Open { get; init; }
    public double High { get; init; }
    public double Low { get; init; }

    public override string ToString() => $"[{At:d}] {Open:N0} -> {Close:N0}";
}