using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Service responsible for business rules around bookings.
    ///
    /// Responsibility:
    /// - Validates booking rules before saving to the database.
    /// - Uses repositories to read and write bookings and rooms.
    /// - Ensures no double bookings and enforces room capacity limits.
    ///
    /// Why this class exists:
    /// - To separate business logic from data access and UI.
    /// - To keep booking rules in one place instead of spreading
    ///   if-statements across Razor Pages and repositories.
    /// </summary>
    public class BookingService
    {
        /// <summary>
        /// Repository used to load, create, update and delete bookings.
        /// </summary>
        private readonly IBookingRepository _bookingRepo;

        /// <summary>
        /// Repository used to load room information, including room type.
        /// </summary>
        private readonly IRoomRepository _roomRepo;

        /// <summary>
        /// Creates a new BookingService with the required repositories.
        /// Using interfaces here makes the service easy to unit test
        /// and independent of the actual database implementation.
        /// </summary>
        public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo)
        {
            _bookingRepo = bookingRepo;
            _roomRepo = roomRepo;
        }

        /// <summary>
        /// Adds a new booking after checking all business rules.
        ///
        /// Rules enforced:
        /// - The room must exist.
        /// - Maximum number of bookings per room depends on RoomType.
        ///   - ClassRoom: up to 2 bookings per time slot.
        ///   - MeetingRoom: only 1 booking per time slot.
        /// - A user cannot have two bookings in the same time slot on the same day.
        /// - A user can have at most 5 bookings in total.
        ///
        /// Why:
        /// - Keeps the system realistic and prevents abuse or overbooking.
        /// - Guarantees that rules are always checked before data is saved.
        /// </summary>
        public void Add(Booking booking)
        {
            EnsureNotInPast(booking);
            Room room = _roomRepo.GetRoomById(booking.RoomID);
            if (room == null)
            {
                throw new Exception("The room does not exist.");
            }
            int maxBookingsForRoom = GetMaxBookingsForRoom(room);
            List<Booking> allBookings = _bookingRepo.GetAll();
            // Same user, same day, same slot -> not allowed
            Booking conflictBooking = allBookings.FirstOrDefault(b =>
                b.AccountID == booking.AccountID &&
                b.BookingDate.Date == booking.BookingDate.Date &&
                b.TimeSlot == booking.TimeSlot);

            if (conflictBooking != null)
            {
                throw new Exception("You already have a booking in this time zone.");
            }

            // Max 5 bookings per user (any date)
            int userBookingCount = allBookings.Count(b => b.AccountID == booking.AccountID);
            if (userBookingCount >= 5)
            {
                throw new Exception("You already have 5 bookings. Delete a booking before you make a new one.");
            }

            // Count bookings for same room + same day + same slot
            int sameRoomSameSlotCount = allBookings.Count(b =>
                b.RoomID == booking.RoomID &&
                b.BookingDate.Date == booking.BookingDate.Date &&
                b.TimeSlot == booking.TimeSlot);
           
            if (sameRoomSameSlotCount >= maxBookingsForRoom)
            {
                if (room.RoomType == RoomType.ClassRoom)
                {
                    throw new Exception("This classroom is already booked by 2 users timezone.");
                }

                throw new Exception("This meeting room is already booked in this timezone.");
            }

            _bookingRepo.Add(booking);
        }


        /// <summary>
        /// Retrieves a single booking by ID.
        /// Why:
        /// - Allows UI and other services to load a detailed booking for view or edit.
        /// </summary>
        public Booking GetBookingById(int bookingID)
        {
            return _bookingRepo.GetBookingById(bookingID);
        }

        /// <summary>
        /// Deletes a booking by its ID.
        /// Enforces that Teachers can only delete a booking
        /// if there are at least 3 days until the booking date.
        ///
        /// Administrators are not restricted by this rule.
        /// </summary>
        public void Delete(int id, string role)
        {
            Booking booking = _bookingRepo.GetBookingById(id);

            if (booking == null)
            {
                throw new Exception("Booking does not exist");
            }

            // Kun Teacher skal have 3 dages varsel.
            if (role == "Teacher")
            {
                EnsureThreeDaysNotice(booking);
            }

            _bookingRepo.Delete(id);
        }

        /// <summary>
        /// Retrieves all bookings.
        /// Why:
        /// - Used for admin overviews, lists and reports.
        /// </summary>
        public List<Booking> GetAll()
        {
            DeleteExpiredBookings();
            return _bookingRepo.GetAll();
        }

        /// <summary>
        /// Updates an existing booking after re-checking capacity rules
        /// and enforcing a 3-day notice rule for Teachers.
        ///
        /// Rules enforced:
        /// - The room must still exist.
        /// - ClassRoom: max 2 bookings per time slot.
        /// - Other rooms: max 1 booking per time slot.
        /// - The booking being updated does not count against itself.
        /// - A Teacher can only change a booking if there are at least 3 days
        ///   until the booking date.
        /// </summary>
        public void Update(Booking booking, string role)
        {
            if (role == "Teacher")
                EnsureThreeDaysNotice(booking);

            var room = _roomRepo.GetRoomById(booking.RoomID)
                ?? throw new Exception("The room cannot be found.");

            int maxBookingsForRoom =
                room.RoomType == RoomType.ClassRoom ? 2 : 1;
            //tæller hvor mange andre bookinger der allerede findes i samme rum, 
            //på samme dato og i samme tidsrum som den booking, du er ved at opdatere.
            int sameRoomSameSlotCount = _bookingRepo.GetAll() //henter alle bookinger,
                .Where(b => b.BookingID != booking.BookingID) //bortset fra det du er ved at redigere.
                .Count(b => //tæller kun hvis,
                    b.RoomID == booking.RoomID && //den er i samme rum,
                    b.BookingDate.Date == booking.BookingDate.Date && //på samme dato,
                    b.TimeSlot == booking.TimeSlot); //og i samme tidsrum

            if (sameRoomSameSlotCount >= maxBookingsForRoom) //tjekker om rummet allerede er booket max
                throw new Exception(
                    room.RoomType == RoomType.ClassRoom
                        ? "This classroom is already booked in this timezone."
                        : "This meeting room is already booked in this timezone."
                );

            _bookingRepo.Update(booking);
        }

        /// <summary>
        /// Calculates availability for all rooms on a given date and time slot,
        /// optionally filtered by room type.
        ///
        /// Responsibility:
        /// - Combines room data and booking data to compute how many bookings
        ///   each room has compared to the maximum allowed.
        ///
        /// Why this method exists:
        /// - Used by "Find available rooms" pages to show an overview with
        ///   colors and status texts (green/yellow/red).
        /// - Keeps the availability logic centralized in the service layer.
        /// </summary>
        public List<RoomAvailability> GetRoomAvailability(DateTime date, TimeSlot timeSlot, RoomType? roomType, bool? selectedSmartBoard)
        {
            var bookings = _bookingRepo.GetAll();

            return _roomRepo.GetAllRooms()
                .Where(r =>
                    (!roomType.HasValue || r.RoomType == roomType.Value) &&
                    (!selectedSmartBoard.HasValue || r.HasSmartBoard == selectedSmartBoard.Value))
                .Select(room =>
                {
                    int maxBookings = room.RoomType == RoomType.ClassRoom ? 2 : 1;

                    int currentBookings = bookings.Count(b =>
                        b.RoomID == room.RoomID &&
                        b.BookingDate.Date == date.Date &&
                        b.TimeSlot == timeSlot);

                    return new RoomAvailability
                    {
                        Room = room,
                        MaxBookings = maxBookings,
                        CurrentBookings = currentBookings,
                        StatusColor = currentBookings == 0
                            ? "green"
                            : currentBookings < maxBookings ? "yellow" : "red",
                        StatusText = currentBookings == 0
                            ? "Helt ledig"
                            : currentBookings < maxBookings ? "Delvist booket" : "Fuldt booket"
                    };
                })
                .ToList();
        }
        /// <summary>
        /// Ensures that there are at least 3 days until the booking date.
        /// Why:
        /// - Used by Delete and Update to enforce the "3 dages varsel" rule
        ///   for Teachers.
        /// </summary>
        private void EnsureThreeDaysNotice(Booking booking)
        {
            DateTime today = DateTime.Now.Date;
            DateTime bookingDate = booking.BookingDate.Date;

            TimeSpan difference = bookingDate - today;
            double daysUntilBooking = difference.TotalDays;

            if (daysUntilBooking < 3)
            {
                throw new Exception("The Booking can  only be editet or deleted with 3 days warning.");
            }
        }
        /// <summary>
        /// Ensures that a booking is not made in the past.
        /// Why:
        /// - Prevents invalid or unrealistic bookings.
        /// </summary>
        private void EnsureNotInPast(Booking booking)
        {
            DateTime today = DateTime.Now.Date;

            if (booking.BookingDate.Date < today)
            {
                throw new Exception("You cannot create a booking in the past.");
            }
        }
        private int GetMaxBookingsForRoom(Room room)
        {
            int maxBookingsForRoom;

            if (room.RoomType == RoomType.ClassRoom)
            {
                maxBookingsForRoom = 2;
            }
            else if (room.RoomType == RoomType.MeetingRoom)
            {
                maxBookingsForRoom = 1;
            }
            else
            {
                maxBookingsForRoom = 1;
            }
            return maxBookingsForRoom;
        }
        public void DeleteExpiredBookings()
        {
            DateTime today = DateTime.Now.Date;

            var expiredBookings = _bookingRepo.GetAll()
                .Where(b => b.BookingDate.Date < today)
                .ToList();

            foreach (var booking in expiredBookings)
            {
                _bookingRepo.Delete(booking.BookingID);
            }
        }
    }
}
