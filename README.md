# �ѱ� �ֽ� ������ ��ũ����

�ѱ� �ֽİ� ���õ� ���� �����͸� ��ũ�����ϴ� �մϴ�.

---------------------------------------
## ����
- �� ���̺귯���� �ܼ��� ���� ����Ʈ � �����Ǿ� �ִ� ������ �о���� ������ �մϴ�.
- �� ���̺귯�� ������� ���� �߻��� �� �ִ� � ���ؿ� ���ؼ��� �����ڴ� å���� �� �� ������, **������ ������ �Ǵ�** �Ŀ� ����Ͻʽÿ�.

## ���
- ���� ����
- �ǽð� ���� ����
- ���� ����
- .Net Standard 2.0�� ����
- ���� LINQ�� �����Ͽ� ���� �ӵ��� ����
- ��� �޼��忡 ���� ���� ����� �񵿱� ���� ����

## ���� ���� - StockLoader
- [���̹� ����](https://finance.naver.com/sise/sise_market_sum.nhn?sosok=0)�� �ִ� ��� ������ ������ ���մϴ�.
- �� ���� �� 23������ ������ ������ �ҷ��ɴϴ�.
  * �����ڵ� / ����� / ���屸�� / �ŷ��� / �ð� / �� / ����
  * �ð��Ѿ� / ����� / �ڻ��Ѱ� / ��ä�Ѱ�
  * �������� / �������� / �ִ������ / �����ֹ���
  * ����������� / �������������� / �ܱ��κ��� / ������
  * PER / ROE / ROA / PBR
- �񵿱� �޼��� ���� `StockLoader.Instance.LoadAsync().Wait();`
- ������ �����ڵ尡 ����� Ŭ������ ������ �� �ֽ��ϴ�. ([������ �ڵ��� ��](/blob/main/Kosdas/Stock.constant.cs))
```csharp
StockLoader.Instance.Load();
StockLoader.Instance.Generate(@"C:\git\KoreanStockLibrary\KoreanStockLibrary\Stock.constant.cs", "KoreanStockLibrary", "Stock");
```
- �� ���� �̺�Ʈ�� �����մϴ�.
  * `ProgressChanged` Load �޼����� �����Ȳ�� ����� �� �߻�
  * �����Ȳ�� ���α׷��� �ٿ� ǥ���ϴ� ��. �񵿱�� ȣ���ϴ� ��쿡�� ũ�ν� ������ ������ ���ϱ� ���� Invoke �޼��带 ���.
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
  * `RequestSending` ������ ��ũ������ �ϱ� ���� �߻�. �̺�Ʈ �ڵ鷯���� Ư�� ������ ������ ��ũ������ �ǳʶ�� �� ���
  * �ڽ��� ������ �ǳʶ�� ��
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
- ���� LINQ�� �����Ͽ� �������� ������ �� �� �ȿ� �ҷ��� �� �ֽ��ϴ�.
- �Ʒ� `PriceLoader` Ŭ������ ������ �� �� �ִ� Ȯ�� �޼��尡 �����˴ϴ�.
  * �Ｚ������ 2021�� 2�� 16���� �ð�/��/����/����/�ŷ����� ���ϴ� �ڵ��� ��
```csharp
var price = StockLoader.Instance[Stock.�Ｚ����].LoadPrice(2021, 2, 16);

Assert.AreEqual(new DateTime(2021, 2, 16), price.Date);
Assert.AreEqual(84500, price.Open);
Assert.AreEqual(86000, price.High);
Assert.AreEqual(84200, price.Low);
Assert.AreEqual(84900, price.Close);
Assert.AreEqual(20373346, price.Volume);
```
- ���� �������� ������ �ҷ��� �� �ֽ��ϴ�.
  * [���̹� ����](https://finance.naver.com/item/coinfo.nhn?code=005930)�� ���� ���������� �ҷ��ɴϴ�.  
  * �޸��� ������ ���������� �ҷ����� �ڵ��� ��
```csharp
var stock = StockLoader.Instance[Stock.�޸�������];
stock.LoadConsensus();

Assert.AreEqual(3.71M, stock.Consensus);
Assert.AreEqual(4443M, stock.TargetPrice);
Assert.AreEqual(7M, stock.ConsensusCount);
```
- �� ������ ���������� ���ķ� �ҷ��� �� �ֽ��ϴ�.
```csharp
var stocks = StockLoader.Instance.ToList();
stocks.AsParallel().ForAll(x => x.LoadConsensus());
```
- ������ ��� ����� [���� �׽�Ʈ �ڵ�](/blob/main/Kosdas.UnitTestProject/StockLoaderTests.cs)���� Ȯ���� �� �ֽ��ϴ�.

## ���� ���� - PriceLoader
- Ư�� ������ Ư�� �Ⱓ ������ �Ϻ� ���� ������ �ҷ��ɴϴ�.
- ���� �������� �Ʒ� �׸��� ���ԵǾ� �ֽ��ϴ�.
  * ���� / �ð� / �� / ���� / ���� / �ŷ���
- �Ｚ������ �ֱ� 2�� ���� ���� ������ �������� �ڵ��� ��
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
- �Ｚ������ 2021�� 2�� 16���� ���� ������ �������� �ڵ��� ��
```csharp
var price = PriceLoader.Instance.Load("005930", 2021, 2, 16);

Assert.AreEqual(new DateTime(2021, 2, 16), price.Date);
Assert.AreEqual(84500, price.Open);
Assert.AreEqual(86000, price.High);
Assert.AreEqual(84200, price.Low);
Assert.AreEqual(84900, price.Close);
Assert.AreEqual(20483100, price.Volume);
```
- �Ｚ������ 2021�� 2�� 5�� ���� 2021�� 2�� 13�� ������ ���� ������ �������� �ڵ��� �� (2021�� 2�� 13�� ���������� �ֱٰ����� 2021�� 2�� 10��)
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
- �Ｚ������ �ֱ� 2�� ���� ���� ������ �������� �ڵ��� ��
```csharp
```

- ������ ��� ����� [���� �׽�Ʈ �ڵ�](/blob/main/Kosdas.UnitTestProject/PriceLoaderTests.cs)���� Ȯ���� �� �ֽ��ϴ�.


## �ǽð� ������ �ü� - RealTimePriceLoader
- 20�� ���� ���� �ǽð� ������ �ü��� �ҷ��ɴϴ�.
  * �� ������ �ǽð� �ü��� �ֿܼ� ����ϴ� �ڵ��� ��
```csharp
RealTimePriceLoader.Instance.Load();
foreach (var stock in StockLoader.Instance)
    Console.WriteLine(RealTimePriceLoader.Instance[stock.Code]);
```
- ���� �ڵ�� �ۼ��Ǿ� �� �� �ȿ� �� ������ �ǽð� �ü��� �ҷ��� �� �ֽ��ϴ�.

## Ȱ�� ����
- �� Ŭ�������� �����Ͽ� �Ʒ��� ���� ���� �� �� �ֽ��ϴ�.
- ��õ ����Ʈ�� 10�� �̻��̰� �����ǰ��� 4.0 �̻��� ������ (���簡/��ǥ�ְ�) ������ ������ �� ���� 10�� ������ ����ϴ� ��
```csharp
// �� ������ ������ ���Ѵ�.
StockLoader.Instance.Load();
var stocks = StockLoader.Instance.ToList();
            
// �� ������ ���������� ���Ѵ�.
stocks.AsParallel().ForAll(x => x.LoadConsensus());
            
// �� ������ �ǽð� �ü��� ���Ѵ�.
RealTimePriceLoader.Instance.Load();

var query = from x in stocks
    // ��õ ����Ʈ�� 10�� �̻��̰� �����ǰ��� 4.0 �̻��� ������ ���͸�
    where x.ConsensusCount > 10 && x.Consensus >= 4.0M
    // (���簡/��ǥ�ְ�) ������ ����
    orderby x.CloseOfTarget
    select x;

// ��� �� �տ��� 10���� ��ȯ
var list = query.Take(10).ToList();

foreach (var stock in list)
{
    // �����ڵ� / ����� / �����ǰ� / ����Ʈ�� / ��ǥ�ְ� / (���簡/��ǥ�ְ�)
    Console.WriteLine($"{stock.Code}\t{stock.Name}\t{stock.Consensus:N2}\t{stock.ConsensusCount}\t{stock.TargetPrice:N0}\t{stock.CloseOfTarget:P2}");
}
```