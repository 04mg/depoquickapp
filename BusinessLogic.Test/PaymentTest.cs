using BusinessLogic.Domain;
using BusinessLogic.Enums;

namespace BusinessLogic.Test;

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
        Assert.AreEqual(Amount, payment.Amount);
    }

    [TestMethod]
    public void TestCanCapturePayment()
    {
        // Arrange
        var payment = new Payment(Amount);

        // Act
        payment.Capture();

        // Assert
        Assert.AreEqual(PaymentStatus.Captured, payment.Status);
    }
}