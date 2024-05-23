using BusinessLogic.Domain;

namespace BusinessLogic.Test;

[TestClass]
public class DateRangeTests
{
    [TestMethod]
    public void TestCantCreateDateRangeIfStartDateIsGreaterThanEndDate()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new DateRange(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DateOnly.FromDateTime(DateTime.Now)));        
        
        // Assert
        Assert.AreEqual("Date range is invalid.", exception.Message);
    }
}