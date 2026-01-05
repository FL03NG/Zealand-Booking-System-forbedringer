using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Accounts
{
    /// <summary>
    /// Responsibility:
    /// - Handles user login.
    /// - Creates a session for logged-in users.
    ///
    /// Why this page exists:
    /// - To allow users to access the system.
    /// - To keep login logic separate from other pages.
    /// </summary>
    public class LoginModel : PageModel
    {
        /// <summary>
        /// Used to validate login information.
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Creates the PageModel and receives the UserService.
        /// </summary>
        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Username from the login form.
        /// </summary>
        [BindProperty]
        public string Username { get; set; }

        /// <summary>
        /// Password from the login form.
        /// </summary>
        [BindProperty]
        public string Password { get; set; }

        /// <summary>
        /// Message shown if login fails.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Handles the login form submission.
        /// </summary>
        public IActionResult OnPost()
        {
            var user = _userService.Login(Username, Password);

            if (user == null)
            {
                ErrorMessage = "Wrong username or password.";
                return Page();
            }

            // Save user data in session
            HttpContext.Session.SetInt32("AccountID", user.AccountID);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToPage("/Index");
        }
    }
}
