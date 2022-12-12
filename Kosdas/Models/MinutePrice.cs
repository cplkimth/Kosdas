using DataVisualizer.Contract;

namespace Kosdas.Models;

public class MinutePrice
{
    public MinutePrice(DateTime at, double close, double volume)
    {
        At = at;
        Close = close;
        Volume = volume;
    }

    [Column(Format = "g")]
    public DateTime At { get; init; }
    public double Close { get; init; }
    public double Volume { get; init; }

    public override string ToString() => $"[{At:yyyyMMdd HHmm}] {Close:N0}";
}