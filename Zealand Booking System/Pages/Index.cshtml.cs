using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// Front page of the system.
    ///
    /// Responsibility:
    /// - Displays the start page.
    /// - Shows different UI depending on login state.
    ///
    /// Why this page exists:
    /// - To give users a clear starting point.
    /// - To guide users to login, register, or book rooms.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Logger for this page.
        /// </summary>
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Creates the PageModel and receives the logger.
        /// </summary>
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Loads the page.
        /// </summary>
        public void OnGet()
        {
            // No logic needed here
        }
    }
}
