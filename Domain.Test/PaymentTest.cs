namespace Domain.Test;

[TestClass]
public class PaymentTest
{
    private const double Amount = 50;

    [TestMethod]
    public void TestCanCreatePayment()
    {
        // Act
        var payment = new Payment(Amount);

        // Assert
        Assert.IsNotNull(payment);
    }

    [TestMethod]
    public void TestCanCapturePayment()
    {
        // Arrange
        var payment = new Payment(Amount);

        // Act
        payment.Capture();

        // Assert
        Assert.IsTrue(payment.IsCaptured());
    }

    [TestMethod]
    public void TestCanGetPaymentAmount()
    {
        // Arrange
        var payment = new Payment(Amount);

        // Act
        var amount = payment.GetAmount();

        // Assert
        Assert.AreEqual(Amount, amount);
    }
}