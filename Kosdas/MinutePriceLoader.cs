using Kosdas.Models;
using System.Net;
using System.Text;

namespace Kosdas;

public class MinutePriceLoader : NaverPriceLoader
{
    internal MinutePriceLoader()
    {
    }

    protected override PriceType PriceType => PriceType.Minute;

    protected override Price ParsePriceCore(string[] tokens)
    {
        return new Price(
            PriceType, 
            DateTime.ParseExact(tokens[0], "yyyyMMddHHmm", null),
            Convert.ToDouble(tokens[4]),
            Convert.ToDouble(tokens[5])
        );
    }
}