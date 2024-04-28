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
}