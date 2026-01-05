using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Handles notifications in the system.
    ///
    /// What this class does:
    /// - Creates notifications for users.
    /// - Gets unread notifications.
    ///
    /// Why this class exists:
    /// - To keep notification logic out of Razor Pages.
    /// - To collect all notification rules in one place.
    /// </summary>
    public class NotificationService
    {
        /// <summary>
        /// Repository that performs actual database operations for notifications.
        /// Injected to support dependency inversion and unit testing.
        /// </summary>
        private readonly INotificationRepository _repo;

        /// <summary>
        /// Creates a new NotificationService with the required repository.
        /// </summary>
        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Creates a new notification for a user.
        ///
        /// Why:
        /// - Used when the system needs to inform a user about a booking change,
        ///   such as deletion or an important update.
        /// </summary>
        public void Create(int accountId, string message)
        {
            _repo.Add(new Notification
            {
                AccountID = accountId,
                Message = message
            });
        }

        /// <summary>
        /// Returns all unread notifications for the given user.
        ///
        /// Why:
        /// - Enables UI components (icons, badges, dropdowns) to show new messages.
        /// - Prevents the user from missing important updates.
        /// </summary>
        public List<Notification> GetUnread(int accountId)
        {
            return _repo.GetUnread(accountId);
        }

        /// <summary>
        /// Marks a notification as read.
        ///
        /// Why:
        /// - Ensures that notifications only appear once.
        /// - Keeps the notification list clean and up to date.
        /// </summary>
        public void MarkAsRead(int id)
        {
            _repo.MarkAsRead(id);
        }
    }
}
