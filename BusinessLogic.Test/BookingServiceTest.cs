using BusinessLogic.DTOs;
using BusinessLogic.Services;
using DataAccess;
using Domain;

namespace BusinessLogic.Test;

[TestClass]
public class BookingServiceTest
{
    private User? _admin;
    private User? _client;
    private User? _otherClient;
    private Credentials _adminCredentials;
    private Credentials _clientCredentials;
    private Credentials _otherClientCredentials;
    private Deposit? _deposit;
    private BookingRepository _bookingRepository = new();
    private DepositRepository _depositRepository = new();
    private UserRepository _userRepository = new();

    private BookingService _bookingService =
        new(new BookingRepository(), new DepositRepository(), new UserRepository());

    [TestInitialize]
    public void Initialize()
    {
        InitializeBookingService();
        RegisterUsers();
        CreateDeposit();
    }

    private void InitializeBookingService()
    {
        _bookingRepository = new BookingRepository();
        _depositRepository = new DepositRepository();
        _userRepository = new UserRepository();
        _bookingService =
            new BookingService(_bookingRepository, _depositRepository, _userRepository);
    }

    private void RegisterUsers()
    {
        _admin = new User(
            "Name Surname",
            "admin@test.com",
            "12345678@mE",
            "Administrator"
        );
        _adminCredentials = new Credentials() { Email = "admin@test.com", Rank = "Administrator" };

        _client = new User(
            "Name Surname",
            "test@test.com",
            "12345678@mE"
        );
        _clientCredentials = new Credentials() { Email = "test@test.com", Rank = "Client" };

        _otherClient = new User(
            "Name Surname",
            "other@test.com",
            "12345678@mE"
        );
        _otherClientCredentials = new Credentials() { Email = "other@test.com", Rank = "Client" };

        _userRepository.Add(_admin);
        _userRepository.Add(_client);
        _userRepository.Add(_otherClient);
    }

    private void CreateDeposit()
    {
        var promotion = new Promotion(1, "label", 50, DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var promotionList = new List<Promotion> { promotion };

        _deposit = new Deposit("Deposit", "A", "Small", true, promotionList);
        _deposit.AddAvailabilityPeriod(new DateRange.DateRange(DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(100))));
        _depositRepository.Add(_deposit);
    }

    [TestMethod]
    public void TestCanAddBooking()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };

        // Act
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        // Assert
        Assert.AreEqual(1, _bookingService.GetAllBookings(_adminCredentials).Count());
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfAdministrator()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email,
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        // Act
        var bookings = _bookingService.GetBookingsByEmail("test@test.com", _adminCredentials);

        // Assert
        Assert.AreEqual(1, bookings.Count());
    }

    [TestMethod]
    public void TestCanGetBookingsByEmailIfSameEmail()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        // Act
        var bookings = _bookingService.GetBookingsByEmail("test@test.com", _clientCredentials);

        // Assert
        Assert.AreEqual(1, bookings.Count());
    }

    [TestMethod]
    public void TestCanGetAllBookings()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        var otherBookingDto = new BookingDto()
        {
            Id = 2,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            DepositName = _deposit!.Name,
            Email = _otherClient!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);
        _bookingService.AddBooking(otherBookingDto, _otherClientCredentials);

        // Act
        var bookings = _bookingService.GetAllBookings(_adminCredentials);

        // Assert
        Assert.AreEqual(2, bookings.Count());
    }

    [TestMethod]
    public void TestCanApproveBooking()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        // Act
        _bookingService.ApproveBooking(1, _adminCredentials);
        var bookings = _bookingService.GetAllBookings(_adminCredentials).ToList();

        // Assert
        Assert.AreEqual("Approved", bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanRejectBooking()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);
        var rejectBookingDto = new BookingDto()
        {
            Id = 1,
            Message = "message"
        };

        // Act
        _bookingService.RejectBooking(rejectBookingDto, _adminCredentials);
        var bookings = _bookingService.GetAllBookings(_adminCredentials).ToList();

        // Assert
        Assert.AreEqual("Rejected", bookings[0].Stage);
    }

    [TestMethod]
    public void TestCanAddMessageToRejection()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        const string message = "example";
        _bookingService.AddBooking(bookingDto, _clientCredentials);
        var rejectBookingDto = new BookingDto()
        {
            Id = 1,
            Message = message
        };

        //Act
        _bookingService.RejectBooking(rejectBookingDto, _adminCredentials);
        var bookings = _bookingService.GetAllBookings(_adminCredentials).ToList();

        //Assert
        Assert.AreEqual(message, bookings[0].Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfNotAdministrator()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _otherClient!.Email
        };
        _bookingService.AddBooking(bookingDto, _otherClientCredentials);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.GetBookingsByEmail("other@test.com", _clientCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetBookingsByEmailOfAnotherUserIfClient()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _otherClient!.Email
        };
        _bookingService.AddBooking(bookingDto, _otherClientCredentials);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.GetBookingsByEmail("other@test.com", _clientCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantGetAllBookingsIfNotAdministrator()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.GetAllBookings(_clientCredentials));

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveBookingsIfNotAdministrator()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.ApproveBooking(1, _clientCredentials));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectBookingsIfNotAdministrator()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);
        var rejectBookingDto = new BookingDto()
        {
            Id = 1,
            Message = "message"
        };

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
            _bookingService.RejectBooking(rejectBookingDto, _clientCredentials));
        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCantApproveNonExistentBooking()
    {
        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingService.ApproveBooking(1, _adminCredentials));
        //Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantRejectNonExistentBooking()
    {
        // Arrange
        var rejectBookingDto = new BookingDto()
        {
            Id = 1,
            Message = "message"
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingService.RejectBooking(rejectBookingDto, _adminCredentials));
        // Assert
        Assert.AreEqual("Booking not found.", exception.Message);
    }

    [TestMethod]
    public void TestRejectMessageCannotBeEmpty()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);
        var rejectBookingDto = new BookingDto()
        {
            Id = 1,
            Message = ""
        };

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _bookingService.RejectBooking(rejectBookingDto, _adminCredentials));
        //Assert
        Assert.AreEqual("Message cannot be empty.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfDepositDoesNotExist()
    {
        // Arrange
        var deposit = new Deposit("Deposit Two", "A", "Small", true, new List<Promotion>());
        var dateRange = new DateRange.DateRange(DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        deposit.AddAvailabilityPeriod(dateRange);
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = "Nonexistent Deposit",
            Email = _client!.Email
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _bookingService.AddBooking(bookingDto, _clientCredentials);
        });

        // Assert
        Assert.AreEqual("Deposit not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfUserDoesNotExist()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = "nonexistent@test.com",
        };

        // Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _bookingService.AddBooking(bookingDto, _clientCredentials);
        });

        // Assert
        Assert.AreEqual("User not found.", exception.Message);
    }

    [TestMethod]
    public void TestCantCreateBookingIfEmailDoesNotMatchCredentials()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email,
        };

        // Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            _bookingService.AddBooking(bookingDto, _otherClientCredentials);
        });

        // Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCanGenerateTxtBookingReportFile()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        //Act
        _bookingService.GenerateReport("txt", _adminCredentials);
        _bookingService.GenerateReport("csv", _adminCredentials);

        //Assert
        Assert.IsTrue(File.Exists("BookingsReport.txt"));
        Assert.IsTrue(File.Exists("BookingsReport.csv"));
    }

    [TestMethod]
    public void TestCantGenerateReportIfInvalidFormat()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        //Act
        var exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            _bookingService.GenerateReport("invalid", _adminCredentials);
        });

        //Assert
        Assert.AreEqual("Invalid format. Supported formats: txt, csv.", exception.Message);
    }

    [TestMethod]
    public void TestCantGenerateBookingsReportIfNotAdministrator()
    {
        //Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        //Act
        var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            _bookingService.GenerateReport("txt", _clientCredentials);
        });

        //Assert
        Assert.AreEqual("You are not authorized to perform this action.", exception.Message);
    }

    [TestMethod]
    public void TestCanCalculateBookingPrice()
    {
        // Arrange
        var priceDto = new BookingDto
        {
            DepositName = _deposit!.Name,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        // Act
        var price = _bookingService.CalculateBookingPrice(priceDto);

        // Assert
        Assert.AreEqual(35, price);
    }

    [TestMethod]
    public void TestPaymentIsAddedToBooking()
    {
        // Arrange
        var bookingDto = new BookingDto()
        {
            Id = 1,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DepositName = _deposit!.Name,
            Email = _client!.Email
        };

        // Act
        _bookingService.AddBooking(bookingDto, _clientCredentials);

        // Assert
        var booking = _bookingService.GetAllBookings(_adminCredentials).First();
        Assert.AreEqual(35, booking.Payment!.Value.Amount);
        Assert.IsFalse(booking.Payment!.Value.Captured);
    }
}