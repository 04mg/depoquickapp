using Calculators;
using Domain;
using Domain.Enums;

namespace BusinessLogic.Test;

[TestClass]
public class PriceCalculatorTest
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    [TestMethod]
    [DataRow(DepositSize.Small, false, 50)]
    [DataRow(DepositSize.Small, true, 70)]
    [DataRow(DepositSize.Medium, false, 75)]
    [DataRow(DepositSize.Medium, true, 95)]
    [DataRow(DepositSize.Large, false, 100)]
    [DataRow(DepositSize.Large, true, 120)]
    public void TestCanCalculateBasePrice(DepositSize size, bool climateControl,
        double expectedPrice)
    {
        var deposit = new Deposit("Deposit", DepositArea.A, size, climateControl, new List<Promotion>());
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            _today, _today.AddDays(1));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow(DepositSize.Small, false, 7, 332.5)]
    [DataRow(DepositSize.Small, false, 15, 675)]
    [DataRow(DepositSize.Medium, false, 7, 498.75)]
    [DataRow(DepositSize.Medium, false, 15, 1012.5)]
    [DataRow(DepositSize.Large, false, 7, 665)]
    [DataRow(DepositSize.Large, false, 15, 1350)]
    [DataRow(DepositSize.Small, true, 7, 465.5)]
    [DataRow(DepositSize.Small, true, 15, 945)]
    [DataRow(DepositSize.Medium, true, 7, 631.75)]
    [DataRow(DepositSize.Medium, true, 15, 1282.5)]
    [DataRow(DepositSize.Large, true, 7, 798)]
    [DataRow(DepositSize.Large, true, 15, 1620)]
    public void TestCanCalculatePriceWithDurationDiscounts(DepositSize size, bool climateControl, int duration,
        double expectedPrice)
    {
        var deposit = new Deposit("Deposit", DepositArea.A, size, climateControl, new List<Promotion>());
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            _today, _today.AddDays(duration));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow(5, DepositSize.Small, false, 1, 47.5)]
    [DataRow(5, DepositSize.Small, false, 7, 315)]
    [DataRow(5, DepositSize.Small, false, 15, 637.5)]
    [DataRow(5, DepositSize.Medium, false, 1, 71.25)]
    [DataRow(5, DepositSize.Medium, false, 7, 472.5)]
    [DataRow(5, DepositSize.Medium, false, 15, 956.25)]
    [DataRow(5, DepositSize.Large, false, 1, 95)]
    [DataRow(5, DepositSize.Large, false, 7, 630)]
    [DataRow(5, DepositSize.Large, false, 15, 1275)]
    [DataRow(5, DepositSize.Small, true, 1, 66.5)]
    [DataRow(5, DepositSize.Small, true, 7, 441)]
    [DataRow(5, DepositSize.Small, true, 15, 892.5)]
    [DataRow(5, DepositSize.Medium, true, 1, 90.25)]
    [DataRow(5, DepositSize.Medium, true, 7, 598.5)]
    [DataRow(5, DepositSize.Medium, true, 15, 1211.25)]
    [DataRow(5, DepositSize.Large, true, 1, 114)]
    [DataRow(5, DepositSize.Large, true, 7, 756)]
    [DataRow(5, DepositSize.Large, true, 15, 1530)]
    public void TestCanCalculatePriceWithOnePromotionUnderFullDiscount(int discount, DepositSize size,
        bool climateControl,
        int durationInDays,
        double expectedPrice)
    {
        var promotion = new Promotion(1, "label", discount, _today, _today.AddDays(durationInDays));
        var promotions = new List<Promotion> { promotion };
        var deposit = new Deposit("Deposit", DepositArea.A, size, climateControl, promotions);
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            _today, _today.AddDays(durationInDays));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow(5, 5, DepositSize.Small, false, 1, 45)]
    [DataRow(10, 15, DepositSize.Small, false, 7, 245)]
    [DataRow(5, 10, DepositSize.Small, false, 15, 562.5)]
    [DataRow(5, 5, DepositSize.Medium, false, 1, 67.5)]
    [DataRow(10, 15, DepositSize.Medium, false, 7, 367.5)]
    [DataRow(5, 10, DepositSize.Medium, false, 15, 843.75)]
    [DataRow(5, 5, DepositSize.Large, false, 1, 90)]
    [DataRow(10, 15, DepositSize.Large, false, 7, 490)]
    [DataRow(5, 10, DepositSize.Large, false, 15, 1125)]
    [DataRow(5, 5, DepositSize.Small, true, 1, 63)]
    [DataRow(10, 15, DepositSize.Small, true, 7, 343)]
    [DataRow(5, 10, DepositSize.Small, true, 15, 787.5)]
    [DataRow(5, 5, DepositSize.Medium, true, 1, 85.5)]
    [DataRow(10, 15, DepositSize.Medium, true, 7, 465.5)]
    [DataRow(5, 10, DepositSize.Medium, true, 15, 1068.75)]
    [DataRow(5, 5, DepositSize.Large, true, 1, 108)]
    [DataRow(10, 15, DepositSize.Large, true, 7, 588)]
    [DataRow(5, 10, DepositSize.Large, true, 15, 1350)]
    public void TestCanCalculatePriceWithMultiplePromotionsUnderFullDiscount(int discount1, int discount2,
        DepositSize size,
        bool climateControl, int durationInDays, double expectedPrice)
    {
        var promotion1 = new Promotion(1, "label", discount1, _today, _today.AddDays(durationInDays));
        var promotion2 = new Promotion(2, "label", discount2, _today, _today.AddDays(durationInDays));
        var promotions = new List<Promotion> { promotion1, promotion2 };
        var deposit = new Deposit("Deposit", DepositArea.A, size, climateControl, promotions);
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            _today, _today.AddDays(durationInDays));
        Assert.AreEqual(expectedPrice, price);
    }

    [TestMethod]
    [DataRow(50, 51, DepositSize.Small, 1)]
    [DataRow(50, 46, DepositSize.Medium, 7)]
    [DataRow(50, 41, DepositSize.Large, 15)]
    public void TestCanCalculatePriceWithPromotionsOverFullDiscount(int discount1, int discount2, DepositSize size,
        int durationInDays)
    {
        var promotion1 = new Promotion(1, "label", discount1, _today, _today.AddDays(durationInDays));
        var promotion2 = new Promotion(2, "label", discount2, _today, _today.AddDays(durationInDays));
        var promotions = new List<Promotion> { promotion1, promotion2 };
        var deposit = new Deposit("Deposit", DepositArea.A, size, true, promotions);
        var priceCalculator = new PriceCalculator();
        var price = priceCalculator.CalculatePrice(deposit,
            _today, _today.AddDays(durationInDays));
        Assert.AreEqual(0, price);
    }
}