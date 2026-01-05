using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System_Library.Service.Tests
{
    /// <summary>
    /// Tests for BookingService focusing on booking validation rules and
    /// correct interaction with repository dependencies.
    /// The tests use mocks to isolate business logic from data access.
    /// </summary>
    [TestClass]
    public class BookingServiceTests
    {
        [TestMethod]
        public void Add_ValidBooking_ShouldCallRepositoryAdd_Once()
        {
            // Arrange
            // Mocks are used to isolate the service logic from database behavior
            var bookingRepoMock = new Mock<IBookingRepository>();
            var roomRepoMock = new Mock<IRoomRepository>();
            // A valid room is required for a booking to be accepted
            var room = new Room
            {
                RoomID = 1,
                RoomName = "A101",
                RoomType = RoomType.ClassRoom,
                RoomDescription = "Test",
                RoomLocation = "A-Bygning"
            };

            // The service depends on room existence to validate bookings
            roomRepoMock
                .Setup(r => r.GetRoomById(1))
                .Returns(room);

            // No existing bookings ensures no conflicts during validation
            var existingBookings = new List<Booking>();
            bookingRepoMock
                .Setup(b => b.GetAll())
                .Returns(existingBookings);

            var service = new BookingService(bookingRepoMock.Object, roomRepoMock.Object);

            var booking = new Booking
            {
                RoomID = 1,
                AccountID = 1,
                BookingDate = DateTime.Today,
                TimeSlot = TimeSlot.Slot08_10,
                BookingDescription = "Gruppearbejde"
            };

            // Act
            service.Add(booking);

            // Assert
            // Verifies that a valid booking results in exactly one repository call
            bookingRepoMock.Verify(b => b.Add(It.Is<Booking>(x =>
                x.RoomID == booking.RoomID &&
                x.AccountID == booking.AccountID &&
                x.BookingDate == booking.BookingDate &&
                x.TimeSlot == booking.TimeSlot &&
                x.BookingDescription == booking.BookingDescription
            )), Times.Once);
        }

        [TestMethod]
        public void Add_SameUserSameDaySameSlot_ShouldThrowException()
        {
            // Arrange
            // Mocking repositories allows validation rules to be tested in isolation
            var bookingRepoMock = new Mock<IBookingRepository>();
            var roomRepoMock = new Mock<IRoomRepository>();

            var room = new Room
            {
                RoomID = 2,
                RoomName = "Møderum 1",
                RoomType = RoomType.MeetingRoom,
                RoomDescription = "Møde",
                RoomLocation = "B-Bygning"
            };
            // The room must exist for the service to reach booking validation logic
            roomRepoMock
                .Setup(r => r.GetRoomById(2))
                .Returns(room);

            var dato = DateTime.Today;

            // Existing booking represents a conflict scenario for the same user
            var existingBooking = new Booking
            {
                RoomID = 2,
                AccountID = 1,
                BookingDate = dato,
                TimeSlot = TimeSlot.Slot10_12
            };

            var existingBookings = new List<Booking> { existingBooking };

            bookingRepoMock
                .Setup(b => b.GetAll())
                .Returns(existingBookings);

            var service = new BookingService(bookingRepoMock.Object, roomRepoMock.Object);

            var newBooking = new Booking
            {
                RoomID = 2,
                AccountID = 1,
                BookingDate = dato,
                TimeSlot = TimeSlot.Slot10_12
            };

            // Act + Assert
            // An exception is expected because the business rule forbids double bookings
            var ex = Assert.ThrowsException<Exception>(() => service.Add(newBooking));

            // The message confirms that the correct validation rule triggered the error
            Assert.AreEqual("You already have a booking in this time zone.", ex.Message);

            // Ensures that invalid bookings are never persisted
            bookingRepoMock.Verify(b => b.Add(It.IsAny<Booking>()), Times.Never);
        }
    }
}
