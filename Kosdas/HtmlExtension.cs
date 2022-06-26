using HtmlAgilityPack;

namespace Kosdas
{
    public static class HtmlExtension
    {
        public static double? ParseCell(this HtmlNode td)
        {
            var parsed = double.TryParse(td.InnerText, out double value);
            if (parsed)
                return value;
            return null;
        }
    }
}