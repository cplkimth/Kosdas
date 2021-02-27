using HtmlAgilityPack;

namespace Kosdas
{
    public static class HtmlExtension
    {
        public static decimal? ParseCell(this HtmlNode td)
        {
            var parsed = decimal.TryParse(td.InnerText, out decimal value);
            if (parsed)
                return value;
            return null;
        }
    }
}