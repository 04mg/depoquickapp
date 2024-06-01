using BusinessLogic.Domain;

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
}