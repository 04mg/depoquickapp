namespace BusinessLogic.Test;

[TestClass]
public class PriceCalculatorTest
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    [TestMethod]
    [DataRow("Small", false, 50)]
    [DataRow("Small", true, 70)]
    [DataRow("Medium", false, 75)]
    [DataRow("Medium", true, 95)]
    [DataRow("Large", false, 100)]
    [DataRow("Large", true, 120)]
    public void TestCanCalculateBasePrice(string size, bool climateControl,
        double expectedPrice)
    {
        var deposit = new Deposit(1, "A", size, climateControl, new List<Promotion>());
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(_today, _today.AddDays(1)));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow("Small", false, 7, 332.5)]
    [DataRow("Small", false, 15, 675)]
    [DataRow("Medium", false, 7, 498.75)]
    [DataRow("Medium", false, 15, 1012.5)]
    [DataRow("Large", false, 7, 665)]
    [DataRow("Large", false, 15, 1350)]
    [DataRow("Small", true, 7, 465.5)]
    [DataRow("Small", true, 15, 945)]
    [DataRow("Medium", true, 7, 631.75)]
    [DataRow("Medium", true, 15, 1282.5)]
    [DataRow("Large", true, 7, 798)]
    [DataRow("Large", true, 15, 1620)]
    public void TestCanCalculatePriceWithDurationDiscounts(string size, bool climateControl, int duration,
        double expectedPrice)
    {
        var deposit = new Deposit(1, "A", size, climateControl, new List<Promotion>());
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(_today, _today.AddDays(duration)));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow(5, "Small", false, 1, 47.5)]
    [DataRow(5, "Small", false, 7, 315)]
    [DataRow(5, "Small", false, 15, 637.5)]
    [DataRow(5, "Medium", false, 1, 71.25)]
    [DataRow(5, "Medium", false, 7, 472.5)]
    [DataRow(5, "Medium", false, 15, 956.25)]
    [DataRow(5, "Large", false, 1, 95)]
    [DataRow(5, "Large", false, 7, 630)]
    [DataRow(5, "Large", false, 15, 1275)]
    [DataRow(5, "Small", true, 1, 66.5)]
    [DataRow(5, "Small", true, 7, 441)]
    [DataRow(5, "Small", true, 15, 892.5)]
    [DataRow(5, "Medium", true, 1, 90.25)]
    [DataRow(5, "Medium", true, 7, 598.5)]
    [DataRow(5, "Medium", true, 15, 1211.25)]
    [DataRow(5, "Large", true, 1, 114)]
    [DataRow(5, "Large", true, 7, 756)]
    [DataRow(5, "Large", true, 15, 1530)]
    public void TestCanCalculatePriceWithOnePromotionUnderFullDiscount(int discount, string size, bool climateControl,
        int durationInDays,
        double expectedPrice)
    {
        var promotion = new Promotion(1, "label", discount, _today, _today.AddDays(durationInDays));
        var promotions = new List<Promotion> { promotion };
        var deposit = new Deposit(1, "A", size, climateControl, promotions);
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(_today, _today.AddDays(durationInDays)));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow(5, 5, "Small", false, 1, 45)]
    [DataRow(10, 15, "Small", false, 7, 245)]
    [DataRow(5, 10, "Small", false, 15, 562.5)]
    [DataRow(5, 5, "Medium", false, 1, 67.5)]
    [DataRow(10, 15, "Medium", false, 7, 367.5)]
    [DataRow(5, 10, "Medium", false, 15, 843.75)]
    [DataRow(5, 5, "Large", false, 1, 90)]
    [DataRow(10, 15, "Large", false, 7, 490)]
    [DataRow(5, 10, "Large", false, 15, 1125)]
    [DataRow(5, 5, "Small", true, 1, 63)]
    [DataRow(10, 15, "Small", true, 7, 343)]
    [DataRow(5, 10, "Small", true, 15, 787.5)]
    [DataRow(5, 5, "Medium", true, 1, 85.5)]
    [DataRow(10, 15, "Medium", true, 7, 465.5)]
    [DataRow(5, 10, "Medium", true, 15, 1068.75)]
    [DataRow(5, 5, "Large", true, 1, 108)]
    [DataRow(10, 15, "Large", true, 7, 588)]
    [DataRow(5, 10, "Large", true, 15, 1350)]
    public void TestCanCalculatePriceWithMultiplePromotionsUnderFullDiscount(int discount1, int discount2, string size,
        bool climateControl, int durationInDays, double expectedPrice)
    {
        var promotion1 = new Promotion(1, "label", discount1, _today, _today.AddDays(durationInDays));
        var promotion2 = new Promotion(2, "label", discount2, _today, _today.AddDays(durationInDays));
        var promotions = new List<Promotion> { promotion1, promotion2 };
        var deposit = new Deposit(1, "A", size, climateControl, promotions);
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            new Tuple<DateOnly, DateOnly>(_today, _today.AddDays(durationInDays)));
        Assert.AreEqual(expectedPrice, price);
    }
}