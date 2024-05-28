using BusinessLogic.Calculators;
using BusinessLogic.Domain;
using BusinessLogic.DTOs;
using BusinessLogic.Enums;
using BusinessLogic.Repositories;
using BusinessLogic.Services;

namespace BusinessLogic.Test;

[TestClass]
public class BookingServiceTest
{
    private User? Admin { get; set; }
    private Credentials AdminCredentials { get; set; }
    private User? Client { get; set; }
    private Deposit? Deposit { get; set; }
    private User? OtherClient { get; set; }
    private Credentials UserCredentials { get; set; }

    private BookingService BookingService { get; set; } =
        new(new BookingRepository(), new DepositRepository(), new UserRepository());

    private UserService UserService { get; set; } = new(new UserRepository());

    private PromotionService PromotionService { get; set; } = new(new PromotionRepository(), new DepositRepository());

    private DepositService DepositService { get; set; } =
        new(new DepositRepository(), new BookingRepository(), new PromotionRepository());

    [TestInitialize]
    public void Initialize()
    {
        var userRepository = new UserRepository();
        var depositRepository = new DepositRepository();
        var bookingRepository = new BookingRepository();
        var promotionRepository = new PromotionRepository();

        BookingService = new BookingService(bookingRepository, depositRepository, userRepository);
        UserService = new UserService(userRepository);
        DepositService = new DepositService(depositRepository, bookingRepository, promotionRepository);
        PromotionService = new PromotionService(promotionRepository, depositRepository);

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

        UserService.Register(Admin, passwordConfirmation);
        AdminCredentials = UserService.Login("admin@test.com", "12345678@mE");
        UserService.Register(Client, passwordConfirmation);
        UserCredentials = UserService.Login("test@test.com", "12345678@mE");
        UserService.Register(OtherClient, passwordConfirmation);
    }

    private void CreateDeposit()
    {
        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var promotionList = new List<Promotion> { promotion };

        Deposit = new Deposit("Deposit", "A", "Small", true, promotionList);
        PromotionService.AddPromotion(promotion, AdminCredentials);
        DepositService.AddDeposit(Deposit, AdminCredentials);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        BookingService.AddBooking(booking);

        // Assert
        Assert.AreEqual(1, BookingService.GetAllBookings(AdminCredentials).Count());
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        var bookings = BookingService.GetBookingsByEmail("test@test.com", AdminCredentials);

        // Assert
        Assert.AreSame(bookings[0], booking);
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfSameEmail()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        var bookings = BookingService.GetBookingsByEmail("test@test.com", UserCredentials);

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
        BookingService.AddBooking(booking);
        BookingService.AddBooking(otherBooking);

        // Act
        var bookings = BookingService.GetAllBookings(AdminCredentials);

        // Assert
        Assert.AreEqual(2, bookings.Count());
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        BookingService.ApproveBooking(1, AdminCredentials);
        var bookings = BookingService.GetAllBookings(AdminCredentials).ToList();

        // Assert
        Assert.AreEqual(BookingStage.Approved, bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        BookingService.RejectBooking(1, AdminCredentials, "Message");
        var bookings = BookingService.GetAllBookings(AdminCredentials).ToList();

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
        BookingService.AddBooking(booking);

        //Act
        BookingService.RejectBooking(1, AdminCredentials, message);
        var bookings = BookingService.GetAllBookings(AdminCredentials).ToList();

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
        BookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingService.AddBooking(repeatedBooking));

        // Assert
        Assert.AreEqual("User already has a booking for this period.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingService.GetBookingsByEmail("other@test.com", UserCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfClient()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingService.GetBookingsByEmail("other@test.com", UserCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllBookingsIfNotAdministrator()
    {
        // Arrange
        var booking = new Booking(1, Deposit!, OtherClient!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingService.GetAllBookings(UserCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingService.ApproveBooking(1, UserCredentials));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectBookingsIfNotAdministrator()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            BookingService.RejectBooking(1, UserCredentials, "message"));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingService.ApproveBooking(1, AdminCredentials));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingService.RejectBooking(1, AdminCredentials, "message"));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestRejectMessageCannotBeEmpty()
    {
        //Arrange
        var booking = new Booking(1, Deposit!, Client!, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());
        BookingService.AddBooking(booking);

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            BookingService.RejectBooking(1, AdminCredentials));
        //Assert
        Assert.AreEqual("Message cannot be empty.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfDepositDoesNotExist()
    {
        // Arrange
        var booking = new Booking(1, new Deposit("Deposit Two", "A", "Small", true, new List<Promotion>()), Client!,
            DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { BookingService.AddBooking(booking); });

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfUserDoesNotExist()
    {
        // Arrange
        var nonExistentUser = new User("Name Surname", "nonexistent@test.com", "12345678@mE");
        var booking = new Booking(1, Deposit!, nonExistentUser, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new PriceCalculator());

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() => { BookingService.AddBooking(booking); });

        // Assert
        Assert.AreEqual("User not found.", exception.Message);
    }
}