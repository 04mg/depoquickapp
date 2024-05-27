using BusinessLogic.Domain;

namespace BusinessLogic.Test;

[TestClass]
public class DateRangeTests
{

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);

    [TestMethod]
    public void TestCantCreateDateRangeIfStartDateIsGreaterThanEndDate()
    {
        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            new DateRange(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DateOnly.FromDateTime(DateTime.Now)));

        // Assert
        Assert.AreEqual("Date range is invalid.", exception.Message);
    }

    [TestMethod]
    public void TestSubtractNonOverlappingDateRanges()
    {
        var range1 = new DateRange(Today, Today.AddDays(2));
        var range2 = new DateRange(Today.AddDays(3), Today.AddDays(5));

        var result = range1.Subtract(range2);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void TestSubtractStartDateIsEqualToOtherStartDateAndEndDateIsEqualToOtherEndDate()
    {
        var range1 = new DateRange(Today, Today.AddDays(2));
        var range2 = new DateRange(Today, Today.AddDays(2));

        var result = range1.Subtract(range2);

        Assert.IsNull(result);
    }
    
    [TestMethod]
    public void TestSubtractStartDateIsLesserThanOtherStartDateAndEndDateIsLesserOrEqualToOtherEndDate()
    {
        var range1 = new DateRange(Today, Today.AddDays(5));
        var range2 = new DateRange(Today.AddDays(3), Today.AddDays(5));

        range1.Subtract(range2);

        Assert.AreEqual(Today, range1.StartDate);
        Assert.AreEqual(Today.AddDays(2), range1.EndDate);
    }
    
    [TestMethod]
    public void TestSubtractStartDateIsGreaterOrEqualThanOtherStartDateAndEndDateIsGreaterThanOtherEndDate()
    {
        var range1 = new DateRange(Today.AddDays(1), Today.AddDays(5));
        var range2 = new DateRange(Today, Today.AddDays(3));

        range1.Subtract(range2);

        Assert.AreEqual(Today.AddDays(4), range1.StartDate);
        Assert.AreEqual(Today.AddDays(5), range1.EndDate);
    }
    
    [TestMethod]
    public void TestSubtractOtherDateRangeIsIncludedInThisDateRange()
    {
        var range1 = new DateRange(Today, Today.AddDays(6));
        var range2 = new DateRange(Today.AddDays(3), Today.AddDays(4));

        var result = range1.Subtract(range2);

        Assert.AreEqual(Today, range1.StartDate);
        Assert.AreEqual(Today.AddDays(2), range1.EndDate);
        Assert.AreEqual(Today.AddDays(5), result.StartDate);
        Assert.AreEqual(Today.AddDays(6), result.EndDate);
    }
}