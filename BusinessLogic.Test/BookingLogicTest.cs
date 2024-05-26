using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;
using BusinessLogic.Logic;
using BusinessLogic.Repositories;

namespace BusinessLogic.Test;

[TestClass]
public class BookingLogicTest
{
    private User? Admin { get; set; }
    private Credentials AdminCredentials { get; set; }
    private User? Client { get; set; }
    private Deposit? Deposit { get; set; }
    private User? OtherClient { get; set; }
    private Credentials UserCredentials { get; set; }

    private BookingLogic BookingLogic { get; set; } =
        new BookingLogic(new BookingRepository(), new DepositRepository());

    private AuthLogic AuthLogic { get; set; } = new AuthLogic(new UserRepository());

    private DepositLogic DepositLogic { get; set; } = new DepositLogic(new DepositRepository(), new BookingRepository());

    [TestInitialize]
    public void Initialize()
    {
        var userRepository = new UserRepository();
        var depositRepository = new DepositRepository();
        var bookingRepository = new BookingRepository();

        BookingLogic = new BookingLogic(bookingRepository, depositRepository);
        AuthLogic = new AuthLogic(userRepository);
        DepositLogic = new DepositLogic(depositRepository, bookingRepository);

        RegisterUsers();
        CreateDeposit();
    }

    private void RegisterUsers()
    {
        const string passwordConfirmation = "12345678@mE";

        Admin = new User(
            "Name Surname",
            "admin@test.com",
            "12345678@mE",
            "Administrator"
        );

        Client = new User(
            "Name Surname",
            "test@test.com",
            "12345678@mE"
        );

        OtherClient = new User(
            "Name Surname",
            "other@test.com",
            "12345678@mE"
        );

        AuthLogic.Register(Admin, passwordConfirmation);
        AdminCredentials = AuthLogic.Login("admin@test.com","12345678@mE");
        AuthLogic.Register(Client, passwordConfirmation);
        UserCredentials = AuthLogic.Login("test@test.com","12345678@mE");
        AuthLogic.Register(OtherClient, passwordConfirmation);
    }

    private void CreateDeposit()
    {
        var promotionList = new List<Promotion>
        {
            new(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        };

        Deposit = new Deposit("Deposit", "A", "Small", true, promotionList);
        DepositLogic.Add(Deposit, AdminCredentials);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        BookingLogic.AddBooking(booking);

        // Assert
        Assert.AreEqual(1, BookingLogic.GetAllBookings(AdminCredentials).Count());
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        var bookings = BookingLogic.GetBookingsByEmail("test@test.com", AdminCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfSameEmail()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        var bookings = BookingLogic.GetBookingsByEmail("test@test.com", UserCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetAllBookings()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        var otherBooking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);
        BookingLogic.AddBooking(otherBooking);

        // Act
        var bookings = BookingLogic.GetAllBookings(AdminCredentials);

        // Assert
        Assert.AreEqual(2, bookings.Count());
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        BookingLogic.ApproveBooking(1, AdminCredentials);
        var bookings = BookingLogic.GetAllBookings(AdminCredentials).ToList();

        // Assert
        Assert.AreEqual(BookingStage.Approved, bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        BookingLogic.RejectBooking(1, AdminCredentials, "Message");
        var bookings = BookingLogic.GetAllBookings(AdminCredentials).ToList();

        // Assert
        Assert.AreEqual(BookingStage.Rejected, bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanAddMessageToRejection()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        const string message = "example";
        BookingLogic.AddBooking(booking);

        //Act
        BookingLogic.RejectBooking(1, AdminCredentials, message);
        var bookings = BookingLogic.GetAllBookings(AdminCredentials).ToList();

        //Assert
        Assert.AreEqual(message, bookings[0].Message);
    }

    [TestMethod]
    public void TestCantAddBookingThatAlreadyExists()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        var repeatedBooking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingLogic.AddBooking(repeatedBooking));

        // Assert
        Assert.AreEqual("User already has a booking for this period.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingLogic.GetBookingsByEmail("other@test.com", UserCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfClient()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingLogic.GetBookingsByEmail("other@test.com", UserCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllBookingsIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingLogic.GetAllBookings(UserCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingLogic.ApproveBooking(1, UserCredentials));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    public void TestCantRejectBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingLogic.RejectBooking(1, UserCredentials, "message"));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingLogic.ApproveBooking(1, AdminCredentials));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingLogic.RejectBooking(1, AdminCredentials, "message"));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestRejectMessageCannotBeEmpty()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingLogic.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingLogic.RejectBooking(1, AdminCredentials));
        //Assert
        Assert.AreEqual("Message cannot be empty.", exception.Message);
    }
}