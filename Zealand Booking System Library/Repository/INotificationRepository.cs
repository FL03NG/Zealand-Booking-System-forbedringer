using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Defines the contract for storing and retrieving user notifications.
    ///
    /// Responsibility:
    /// - Allows the system to create notifications, fetch unread ones,
    ///   and update their state when they are viewed.
    ///
    /// Why this interface exists:
    /// - Keeps notification logic separate from booking logic.
    /// - Allows the system to switch between different storage methods
    ///   (SQL, in-memory, mock implementations) without affecting the rest of the code.
    /// - Supports clean architecture and improves testability.
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Saves a new notification for a user.
        /// Why:
        /// - Needed when the system wants to alert users about booking updates,
        ///   cancellations, system messages, etc.
        /// </summary>
        void Add(Notification notification);

        /// <summary>
        /// Retrieves all unread notifications for a specific user.
        /// Why:
        /// - Enables the UI to show notification badges or messages
        ///   so users do not miss important updates.
        /// </summary>
        List<Notification> GetUnread(int accountId);

        /// <summary>
        /// Marks a notification as read.
        /// Why:
        /// - Keeps the notification center clean.
        /// - Ensures that only unread messages are shown to the user.
        /// </summary>
        void MarkAsRead(int notificationId);
    }
}
