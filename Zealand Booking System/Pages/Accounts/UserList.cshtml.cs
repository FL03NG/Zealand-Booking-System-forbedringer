using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System.Pages.Accounts
{
    /// <summary>
    /// Responsibility:
    /// - Shows a list of all users.
    /// - Allows admins to search, edit, and delete accounts.
    ///
    /// Why this page exists:
    /// - To manage user accounts in one place.
    /// - To give administrators full control over users.
    /// </summary>
    public class UserListModel : PageModel
    {
        /// <summary>
        /// Database connection string used by the repository.
        /// </summary>
        private readonly string _connectionString =
            "Server =(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        /// <summary>
        /// Users shown in the list.
        /// </summary>
        public List<Account> Users { get; private set; }

        /// <summary>
        /// Search text from the form.
        /// </summary>
        [BindProperty]
        public string SearchName { get; set; }

        /// <summary>
        /// The user id currently being edited.
        /// </summary>
        [BindProperty]
        public int EditUserID { get; set; }

        /// <summary>
        /// The user being edited.
        /// </summary>
        [BindProperty]
        public Account EditUser { get; set; }

        /// <summary>
        /// Message shown after actions (save/delete).
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Loads the page and shows all users.
        /// </summary>
        public void OnGet()
        {
            LoadData();
        }

        /// <summary>
        /// Filters users by username.
        /// </summary>
        public void OnPostSearch()
        {
            LoadData();

            if (string.IsNullOrWhiteSpace(SearchName))
            {
                return;
            }

            string searchLower = SearchName.ToLower();
            List<Account> filtered = new List<Account>();

            foreach (Account user in Users)
            {
                if (user.Username != null &&
                    user.Username.ToLower().Contains(searchLower))
                {
                    filtered.Add(user);
                }
            }

            Users = filtered;
        }

        /// <summary>
        /// Starts edit mode for the selected user.
        /// </summary>
        public IActionResult OnPostStartEdit(int accountID)
        {
            EditUserID = accountID;

            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            EditUser = repo.GetById(accountID);

            LoadData();
            return Page();
        }

        /// <summary>
        /// Saves the edited user.
        /// </summary>
        public IActionResult OnPostSaveEdit()
        {
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            repo.UpdateUser(EditUser);

            Message = "User saved";
            LoadData();
            return Page();
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        public IActionResult OnPostDelete(int accountID)
        {
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            repo.DeleteUser(accountID);

            Message = "User deleted";
            LoadData();
            return Page();
        }

        /// <summary>
        /// Loads all users from the database.
        /// </summary>
        private void LoadData()
        {
            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
    }
}
