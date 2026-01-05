using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// PageModel for user notifications.
    ///
    /// What this class does:
    /// - Loads unread notifications for the logged-in user.
    /// - Lets the user mark notifications as read.
    ///
    /// Why this class exists:
    /// - To show important system messages in one place.
    /// - To give users an easy way to clear notifications.
    /// </summary>
    public class NotificationsModel : PageModel
    {
        /// <summary>
        /// Service used to get and update notifications.
        /// </summary>
        private readonly NotificationService _service;

        /// <summary>
        /// List of unread notifications shown on the page.
        /// </summary>
        public List<Notification> Notifications { get; set; }

        /// <summary>
        /// Creates the PageModel and receives the notification service.
        /// </summary>
        public NotificationsModel(NotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Runs when the page is loaded (GET).
        /// Loads unread notifications for the logged-in user.
        /// If the user is not logged in, the list is empty.
        /// </summary>
        public void OnGet()
        {
            int? accountId = HttpContext.Session.GetInt32("AccountID");

            if (accountId.HasValue)
            {
                Notifications = _service.GetUnread(accountId.Value);
            }
            else
            {
                Notifications = new List<Notification>();
            }
        }

        /// <summary>
        /// Marks a notification as read.
        /// Then reloads the page to update the list.
        /// </summary>
        public IActionResult OnPostMarkRead(int notificationID)
        {
            _service.MarkAsRead(notificationID);
            return RedirectToPage();
        }
    }
}
