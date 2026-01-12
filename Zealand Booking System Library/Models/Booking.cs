using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Enumeration representing the different time slots that a room can be booked for.
    /// Each enum value has a Display attribute so the UI can show readable labels.
    ///
    /// Why this enum exists:
    /// - To avoid hardcoded strings around the system.
    /// - To guarantee consistent booking times across the application.
    /// - To make validation and display formatting easier.
    /// </summary>
    public enum TimeSlot
    {
        [Display(Name = "08:00 - 10:00")]
        Slot08_10,

        [Display(Name = "10:00 - 12:00")]
        Slot10_12,

        [Display(Name = "12:00 - 14:00")]
        Slot12_14,

        [Display(Name = "14:00 - 16:00")]
        Slot14_16
    }

    /// <summary>
    /// Represents a single booking made by a user for a specific room.
    ///
    /// Responsibility:
    /// - Stores all information needed to describe a booking: who booked, which room,
    ///   the date, time slot, and optional description.
    /// - Acts as the core data model used by services, repositories, and Razor Pages.
    ///
    /// Why this class exists:
    /// - To keep booking data structured and strongly typed.
    /// - To allow the application to use the same booking representation in the UI,
    ///   database operations, and business logic.
    /// </summary>
    public class Booking : IComparable<Booking>
    {
        /// <summary>
        /// Unique ID for the booking.
        /// Set by the database.
        /// </summary>
        public int BookingID { get; set; }

        /// <summary>
        /// Optional text describing the purpose of the booking.
        /// Useful for teachers/students to add context.
        /// </summary>
        public string BookingDescription { get; set; }

        /// <summary>
        /// Foreign key reference to the room being booked.
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// Navigation property for accessing room details.
        /// Why:
        /// - Allows the system to load full room objects when needed.
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Foreign key reference to the user who made the booking.
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// Navigation property for accessing the account that created the booking.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// The date the room is booked for.
        /// Stored as DateTime but only the date portion is used.
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// The chosen time slot for the booking.
        /// Uses the TimeSlot enum to ensure only valid times are used.
        /// </summary>
        public TimeSlot TimeSlot { get; set; }

        /// <summary>
        /// Full constructor used when loading a booking with all related data.
        /// Why:
        /// - Allows services and repositories to populate all fields at once.
        /// </summary>
        public Booking(int bookingID, string bookingDescription, int roomID, Room room, int accountID, Account account, DateTime bookingDate, TimeSlot timeSlot)
        {
            BookingID = bookingID;
            BookingDescription = bookingDescription;
            RoomID = roomID;
            Room = room;
            AccountID = accountID;
            Account = account;
            BookingDate = bookingDate;
            TimeSlot = timeSlot;
        }

        /// <summary>
        /// Parameterless constructor required for:
        /// - Model binding in Razor Pages
        /// - JSON/XML serialization
        /// - Entity Framework or manual mapping
        /// </summary>
        public Booking() { }
        public int CompareTo(Booking other)
        {
            if (other == null) return 1;
            return this.BookingDate.CompareTo(other.BookingDate);
        }
    }
}
