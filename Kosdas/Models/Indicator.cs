namespace Kosdas.Models;

public record Indicator(
    int Year,
    double? Price,
    double? PER,
    double? PBR,
    double? PSR,
    double? PCR,
    double? EPS,
    double? BPS,
    double? SPS,
    double? CPS,
    double? ROE,
    double? ROA,
    double? 매출액,
    double? 영업이익률,
    double? 순이익률,
    double? PCH,
    double? PCL,
    double? 당기순이익,
    double? ROIC,
    double? 매출액증가율,
    double? 영업이익증가율,
    double? 순이익증가율,
    double? 총자산증가율,
    double? 유동자산증가율,
    double? 유형자산증가율,
    double? 자기자본증가율,
    double? 부채비율,
    double? 유동부채비율,
    double? 비유동부채비율,
    double? 이자발생부채,
    double? 순부채,
    double? 이자보상배율,
    double? 총자산회전율,
    double? 매출채권회전율,
    double? 재고자산회전율,
    double? 매입채무회전율,
    double? 매출채권회전일수,
    double? 재고자산회전일수,
    double? 매입채무회전일수,
    double? CashCycle)
{
    public string StockCode { get; set; }

    public string StockName { get; set; }

    // public override string ToString() => $"Price:{Price:N0} PER:{PER:F2} PBR:{PBR:F2} PSR:{PSR:F2} PCR:{PCR:F2}";

    public Indicator Apply(double price)
    {
        return this with
               {
                   PER = PER * price / Price,
                   PBR = PBR * price / Price,
                   PSR = PSR * price / Price,
                   PCR = PCR * price / Price,
                   Price = price
               };
    }
}