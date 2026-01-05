using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Accounts
{
    /// <summary>
    /// Responsibility:
    /// - Handles user registration.
    /// - Creates a new user based on the selected role.
    ///
    /// Why this page exists:
    /// - To allow new users to create an account.
    /// - To keep registration logic separate from other pages.
    /// </summary>
    public class RegisterModel : PageModel
    {
        /// <summary>
        /// Used to create and manage users.
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Creates the PageModel and receives the UserService.
        /// </summary>
        public RegisterModel(UserService service)
        {
            _userService = service;
        }

        /// <summary>
        /// Username from the registration form.
        /// </summary>
        [BindProperty]
        public string Username { get; set; }

        /// <summary>
        /// Password from the registration form.
        /// </summary>
        [BindProperty]
        public string Password { get; set; }

        /// <summary>
        /// Selected role for the new user.
        /// </summary>
        [BindProperty]
        public string Role { get; set; }

        /// <summary>
        /// Message shown if something goes wrong.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Message shown when the user is created.
        /// </summary>
        public string SuccessMessage { get; set; }

        /// <summary>
        /// Handles the registration form submission.
        /// </summary>
        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Fill all fields";
                return Page();
            }

            // Create user based on selected role
            Account newUser = Role switch
            {
                "Administrator" => new Administrator(),
                "Teacher" => new Teacher(),
                "Student" => new Student(),
                _ => new Account()
            };

            newUser.Username = Username;
            newUser.PasswordHash = Password; // stored as plain text --> Password hashing is handled in the service layer

            try
            {
                _userService.Create(newUser, Role);
                SuccessMessage = "User created";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
            }

            return RedirectToPage("/Accounts/Login");
        }
    }
}
