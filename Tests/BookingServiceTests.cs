using System;
using System.Collections.Generic;
using Xunit;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System_Tests
{
    public class BookingServiceTests
    {

        // ===== TEST 1 (FACT) =====
        // Tester happy path: Booking skal gemmes, hvis alt er OK
        [Fact]
        public void Add_ValidBooking_HappyPath()
        {
            // Arrange
            FakeBookingRepository bookingRepo = new FakeBookingRepository();
            FakeRoomRepository roomRepo = new FakeRoomRepository();
            BookingService service = new BookingService(bookingRepo, roomRepo);

            Room room = new Room();
            room.RoomID = 1;
            room.RoomName = "A101";
            room.RoomType = RoomType.ClassRoom;
            roomRepo.AddRoom(room);

            Booking booking = new Booking();
            booking.RoomID = 1;
            booking.AccountID = 1;
            booking.BookingDate = DateTime.Today;
            booking.TimeSlot = TimeSlot.Slot08_10;
            booking.BookingDescription = "Gruppearbejde";

            Boolean noException = true;

            // Act
            try
            {
                service.Add(booking);
            }
            catch (Exception)
            {
                noException = false;
            }

            // Assert
            Assert.True(noException);
            Assert.Equal(1, bookingRepo.GetAll().Count);
        }


        // ===== TEST 2 (THEORY) =====
        // Tester meeting rooms edge case: Max 1 booking pr tidsrum
        [Theory]
        [InlineData(1, 1, "Du har allerede en booking i dette tidsrum.")]
        public void Add_MeetingRoom_DoesNotAllowDoubleBooking(int user1, int user2, string expectedMessage)
        {
            // Arrange
            FakeBookingRepository bookingRepo = new FakeBookingRepository();
            FakeRoomRepository roomRepo = new FakeRoomRepository();
            BookingService service = new BookingService(bookingRepo, roomRepo);

            Room meetingRoom = new Room();
            meetingRoom.RoomID = 2;
            meetingRoom.RoomName = "Meeting Room";
            meetingRoom.RoomType = RoomType.MeetingRoom;
            roomRepo.AddRoom(meetingRoom);

            DateTime dato = DateTime.Today;

            Booking first = new Booking();
            first.RoomID = 2;
            first.AccountID = user1;
            first.BookingDate = dato;
            first.TimeSlot = TimeSlot.Slot10_12;
            bookingRepo.Add(first); // gemmes direkte for at simulere eksisterende booking

            Booking second = new Booking();
            second.RoomID = 2;
            second.AccountID = user2;
            second.BookingDate = dato;
            second.TimeSlot = TimeSlot.Slot10_12;

            Boolean exceptionThrown = false;
            string actualMessage = "";

            // Act
            try
            {
                service.Add(second);
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                actualMessage = ex.Message;
            }

            // Assert
            Assert.True(exceptionThrown);
            Assert.Equal(expectedMessage, actualMessage);
        }
    }

    // ===== FAKE REPOSITORIES (IN MEMORY) =====

    public class FakeBookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = new List<Booking>();

        public List<Booking> GetAll()
        {
            return _bookings;
        }

        public void Add(Booking booking)
        {
            _bookings.Add(booking);
        }

        public Booking GetBookingById(int bookingID)
        {
            return null;
        }

        public void Update(Booking booking)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeRoomRepository : IRoomRepository
    {
        private readonly List<Room> _rooms = new List<Room>();

        public void AddRoom(Room room)
        {
            _rooms.Add(room);
        }

        public Room GetRoomById(int roomID)
        {
            Room found = null;
            foreach (Room room in _rooms)
            {
                if (room.RoomID == roomID)
                {
                    found = room;
                    break;
                }
            }
            return found;
        }

        public void DeleteRoom(int id)
        {
            throw new NotImplementedException();
        }

        public List<Room> GetAllRooms()
        {
            return _rooms;
        }

        public void UpdateRoom(Room room)
        {
            throw new NotImplementedException();
        }
    }

}
