using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Represents a user account in the system.
    /// This keeps all core login-related details together, so authentication
    /// and authorization can rely on a single, consistent source of truth.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Unique ID for the account, used so the system can reliably track the userr.
        /// </summary>
        public int AccountID { get; set; }
        /// <summary>
        /// The username the person logs in with. Helps tie all their bookings and actions to one identity.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Hashed version of the password. Storing only the hash keeps the system safer
        /// by avoiding plain-text passwords.
        /// </summary>
        public string PasswordHash { get; set; }
        /// <summary>
        /// The user's role. Virtual so subclasses (like Admin, Student, or Teacher) can override it.
        /// This makes permission handling more flexible.
        /// </summary>
        public virtual string Role { get; set; } = "Account";

        public Account() { }
        /// <summary>
        /// Lets you create an account with all key info set right away,
        /// reducing the chance of making incomplete account entries.
        /// </summary>
        public Account(int accountID, string username, string passwordHash)
        {
            AccountID = accountID;
            Username = username;
            PasswordHash = passwordHash;
        }
    }
}
