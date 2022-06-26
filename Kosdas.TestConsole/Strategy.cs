using System;
using System.Linq;

namespace Kosdas.TestConsole;

public class Strategy
{
    public void Run(string stockId)
    {
        var indicators = IndicatorLoader.Instance.List.Where(x => x.Year != 2022).GroupBy(x => x.Year).ToDictionary(x => x.Key, x => x.ToList());

        foreach (var list in indicators)
        {
            
        }
        
    }
}