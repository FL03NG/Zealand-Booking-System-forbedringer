using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Represents a message sent to a user, typically used to inform themm
    /// about changes to their bookings (such as cancellations or system updates).
    ///
    /// Responsibility:
    /// - Stores all details related to a notification, including text, time created,
    ///   read status, and the user it belongs to.
    ///
    /// Why this class exists:
    /// - To provide a consistent, strongly typed way of handling user notifications
    ///   throughout the system instead of using raw strings or ad-hoc structures.
    /// - Makes it possible for the UI and services to easily display and manage
    ///   unread messages.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Unique ID of the notification (assigned by the database).
        /// </summary>
        public int NotificationID { get; set; }

        /// <summary>
        /// ID of the user who receives the notification.
        /// Used as a foreign key reference to the Account table.
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// The message content shown to the user.
        /// Example: "Your booking has been deleted."
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates whether the user has read the notification.
        /// Used to control unread notification badges and UI reminders.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Timestamp of when the notification was created.
        /// Helps order notifications chronologically.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property linking to the Account model.
        /// Why:
        /// - Allows higher layers (services/UI) to access username or account details
        ///   without making extra database calls.
        /// </summary>
        public Account Account { get; set; }
    }
}
