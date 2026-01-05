using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Interface: IBookingRepository
    ///
    /// What it does:
    /// - Defines the methods a booking repository must have.
    ///
    /// Why this exists:
    /// - So the service layer can work with bookings without caring if data comes from SQL,
    ///   a fake repository, or another storage type.
    /// - Makes the code easier to test and easier to change later.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Adds a new booking to storage.
        /// Used when a user creates a booking.
        /// </summary>
        public void Add(Booking booking);

        /// <summary>
        /// Gets one booking by ID.
        /// Used when editing or showing booking details.
        /// </summary>
        public Booking GetBookingById(int bookingID);

        /// <summary>
        /// Deletes a booking by ID.
        /// Used when a booking is cancelled or removed.
        /// </summary>
        public void Delete(int id);

        /// <summary>
        /// Gets all bookings from storage.
        /// Used to show booking lists for users/admins.
        /// </summary>
        public List<Booking> GetAll();

        /// <summary>
        /// Updates an existing booking.
        /// Used when a booking is edited (date/time/user/etc.).
        /// </summary>
        public void Update(Booking booking);
    }
}
