# 한국 주식 데이터 스크래퍼

한국 주식과 관련된 각종 데이터를 스크래핑하는 합니다.

---------------------------------------
## 주의
- 본 라이브러리는 단순히 포털 사이트 등에 공개되어 있는 정보를 읽어오는 역할을 합니다.
- 본 라이브러리 사용으로 인해 발생할 수 있는 어떤 피해에 대해서도 개발자는 책임을 질 수 없으니, **본인의 신중한 판단** 후에 사용하십시오.

## 기능
- 종목 정보
- 실시간 가격 정보
- 가격 정보
- .Net Standard 2.0을 지원
- 병렬 LINQ로 구현하여 실행 속도가 빠름
- 모든 메서드에 대해 각각 동기와 비동기 버전 제공

## 종목 정보 - StockLoader
- [네이버 금융](https://finance.naver.com/sise/sise_market_sum.nhn?sosok=0)에 있는 모든 종목의 정보를 구합니다.
- 각 종목 당 23가지의 정보를 가지고 불러옵니다.
  * 종목코드 / 종목명 / 시장구분 / 거래량 / 시가 / 고가 / 저가
  * 시가총액 / 매출액 / 자산총계 / 부채총계
  * 영업이익 / 당기순이익 / 주당순이익 / 보통주배당금
  * 매출액증가율 / 영업이익증가율 / 외국인비율 / 유보율
  * PER / ROE / ROA / PBR
- 비동기 메서드 지원 `StockLoader.Instance.LoadAsync().Wait();`
- 종목명과 종목코드가 저장된 클래스를 생성할 수 있습니다. ([생성된 코드의 예](/blob/main/Kosdas/Stock.constant.cs))
```csharp
StockLoader.Instance.Load();
StockLoader.Instance.Generate(@"C:\git\KoreanStockLibrary\KoreanStockLibrary\Stock.constant.cs", "KoreanStockLibrary", "Stock");
```
- 두 개의 이벤트를 제공합니다.
  * `ProgressChanged` Load 메서드의 진행상황이 변경될 때 발생
  * 진행상황을 프로그래스 바에 표시하는 예. 비동기로 호출하는 경우에는 크로스 쓰레드 문제를 피하기 위해 Invoke 메서드를 사용.
```csharp
protected override void OnLoad(EventArgs e)
{
    StockLoader.Instance.ProgressChanged += StockLoader_ProgressChanged;
}

private void StockLoader_ProgressChanged(object sender, StockLoader.ProgressChangedEventArgs e)
{
    prbProgress.Invoke(new Action(() => prbProgress.Value = (int) e.Percent));
}
```
  * `RequestSending` 페이지 스크래핑을 하기 전에 발생. 이벤트 핸들러에서 특정 조건의 페이지 스크래핑을 건너띄는 데 사용
  * 코스닥 종목은 건너띄는 예
```csharp
protected override void OnLoad(EventArgs e)
{
    StockLoader.Instance.RequestSending += StockLoader_RequestSending;
}

private void StockLoader_RequestSending(object sender, StockLoader.RequestSendingEventArgs e)
{
    if (e.Market == Market.KQ)
        e.Cancel = true;
}
```
- 병렬 LINQ로 구현하여 전종목의 정보를 수 초 안에 불러올 수 있습니다.
- 아래 `PriceLoader` 클래스와 연동해 쓸 수 있는 확장 메서드가 제공됩니다.
  * 삼성전자의 2021년 2월 16일의 시가/고가/저가/종가/거래량을 구하는 코드의 예
```csharp
var price = StockLoader.Instance[Stock.삼성전자].LoadPrice(2021, 2, 16);

Assert.AreEqual(new DateTime(2021, 2, 16), price.Date);
Assert.AreEqual(84500, price.Open);
Assert.AreEqual(86000, price.High);
Assert.AreEqual(84200, price.Low);
Assert.AreEqual(84900, price.Close);
Assert.AreEqual(20373346, price.Volume);
```
- 종목별 컨센서스 정보를 불러올 수 있습니다.
  * [네이버 금융](https://finance.naver.com/item/coinfo.nhn?code=005930)의 종목별 컨센서스를 불러옵니다.  
  * 메리츠 증권의 컨센서스를 불러오는 코드의 예
```csharp
var stock = StockLoader.Instance[Stock.메리츠증권];
stock.LoadConsensus();

Assert.AreEqual(3.71M, stock.Consensus);
Assert.AreEqual(4443M, stock.TargetPrice);
Assert.AreEqual(7M, stock.ConsensusCount);
```
- 전 종목의 컨센서스를 병렬로 불러올 수 있습니다.
```csharp
var stocks = StockLoader.Instance.ToList();
stocks.AsParallel().ForAll(x => x.LoadConsensus());
```
- 완전한 사용 방법은 [단위 테스트 코드](/blob/main/Kosdas.UnitTestProject/StockLoaderTests.cs)에서 확인할 수 있습니다.

## 가격 정보 - PriceLoader
- 특정 종목의 특정 기간 동안의 일별 가격 정보를 불러옵니다.
- 가격 정보에는 아래 항목이 포함되어 있습니다.
  * 일자 / 시가 / 고가 / 저가 / 종가 / 거래량
- 삼성전자의 최근 2일 간의 가격 정보를 가져오는 코드의 예
```csharp
var prices = PriceLoader.Instance.Load("005930", 2).ToList();

Assert.AreEqual(2, prices.Count);
Assert.AreEqual(new DateTime(2021, 2, 15), prices[0].Date);
Assert.AreEqual(new DateTime(2021, 2, 16), prices[1].Date);
Assert.AreEqual(83800, prices[0].Open);
Assert.AreEqual(84500, prices[0].High);
Assert.AreEqual(83300, prices[0].Low);
Assert.AreEqual(84200, prices[0].Close);
Assert.AreEqual(23529706, prices[0].Volume);
Assert.AreEqual(84500, prices[1].Open);
Assert.AreEqual(86000, prices[1].High);
Assert.AreEqual(84200, prices[1].Low);
Assert.AreEqual(84900, prices[1].Close);
Assert.AreEqual(20373346, prices[1].Volume);
```
- 삼성전자의 2021년 2월 16일의 가격 정보를 가져오는 코드의 예
```csharp
var price = PriceLoader.Instance.Load("005930", 2021, 2, 16);

Assert.AreEqual(new DateTime(2021, 2, 16), price.Date);
Assert.AreEqual(84500, price.Open);
Assert.AreEqual(86000, price.High);
Assert.AreEqual(84200, price.Low);
Assert.AreEqual(84900, price.Close);
Assert.AreEqual(20483100, price.Volume);
```
- 삼성전자의 2021년 2월 5일 부터 2021년 2월 13일 사이의 가격 정보를 가져오는 코드의 예 (2021년 2월 13일 시점에서의 최근가격은 2021년 2월 10일)
```csharp
var prices = PriceLoader.Instance.Load("005930", new DateTime(2021, 2, 5), new DateTime(2021, 2, 13)).ToList();

Assert.AreEqual(4, prices.Count);
Assert.AreEqual(new DateTime(2021, 2, 5), prices[0].Date);
Assert.AreEqual(new DateTime(2021, 2, 10), prices[^1].Date);
Assert.AreEqual(82500, prices[0].Low);
Assert.AreEqual(84200, prices[1].High);
Assert.AreEqual(84000, prices[2].Open);
Assert.AreEqual(81600, prices[3].Close);
Assert.AreEqual(23025766, prices[3].Volume);
```
- 삼성전자의 최근 2일 간의 가격 정보를 가져오는 코드의 예
```csharp
```

- 완전한 사용 방법은 [단위 테스트 코드](/blob/main/Kosdas.UnitTestProject/PriceLoaderTests.cs)에서 확인할 수 있습니다.


## 실시간 전종목 시세 - RealTimePriceLoader
- 20분 지연 없는 실시간 전종목 시세를 불러옵니다.
  * 전 종목의 실시간 시세를 콘솔에 출력하는 코드의 예
```csharp
RealTimePriceLoader.Instance.Load();
foreach (var stock in StockLoader.Instance)
    Console.WriteLine(RealTimePriceLoader.Instance[stock.Code]);
```
- 병렬 코드로 작성되어 수 초 안에 전 종목의 실시간 시세를 불러올 수 있습니다.

## 활용 예시
- 위 클래스들을 결합하여 아래와 같은 일을 할 수 있습니다.
- 추천 레포트가 10개 이상이고 투자의견이 4.0 이상인 종목을 (현재가/목표주가) 순으로 정렬한 후 상위 10개 종목을 출력하는 예
```csharp
// 전 종목의 정보를 구한다.
StockLoader.Instance.Load();
var stocks = StockLoader.Instance.ToList();
            
// 전 종목의 컨센서스를 구한다.
stocks.AsParallel().ForAll(x => x.LoadConsensus());
            
// 전 종목의 실시간 시세를 구한다.
RealTimePriceLoader.Instance.Load();

var query = from x in stocks
    // 추천 레포트가 10개 이상이고 투자의견이 4.0 이상인 종목을 필터링
    where x.ConsensusCount > 10 && x.Consensus >= 4.0M
    // (현재가/목표주가) 순으로 정렬
    orderby x.CloseOfTarget
    select x;

// 목록 중 앞에서 10개를 반환
var list = query.Take(10).ToList();

foreach (var stock in list)
{
    // 종목코드 / 종목명 / 투자의견 / 레포트수 / 목표주가 / (현재가/목표주가)
    Console.WriteLine($"{stock.Code}\t{stock.Name}\t{stock.Consensus:N2}\t{stock.ConsensusCount}\t{stock.TargetPrice:N0}\t{stock.CloseOfTarget:P2}");
}
```