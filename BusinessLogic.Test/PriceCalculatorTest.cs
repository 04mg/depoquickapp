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
}