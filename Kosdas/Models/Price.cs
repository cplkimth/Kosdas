namespace Kosdas.Models;

/// <summary>
///     가격 정보
/// </summary>
public record Price(PriceType PriceType, DateTime At, double Close, double Volume = 0, double Open = 0, double High = 0, double Low = 0)
{
    public override string ToString() => PriceType switch
    {
        PriceType.Day => $"[{At:yyMMdd}] {Open:N0} -> {Close:N0}",
        PriceType.Minute => $"[{At:yyMMdd HH:mm}] {Close:N0}",
        _ => throw new ArgumentOutOfRangeException()
    };
}

public enum PriceType
{
    Day,
    Minute
}