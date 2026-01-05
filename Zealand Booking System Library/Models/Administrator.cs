using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Represents an administrator account. 
    /// This class exists so the system can easily distinguish admins from regular users
    /// and give them access to features other accounts shouldn't have.
    /// </summary>
    public class Administrator : Account
    {
        /// <summary>
        /// A separate admin ID in case the system needs to track admins differently
        /// from regular accounts.
        /// </summary>
        public int AdministratorID { get; set; }
        /// <summary>
        /// Overrides the base role so the system instantly knows this user
        /// has administrator permissions.
        /// </summary>
        public override string Role { get; set; } = "Administrator";
        public Administrator() : base() { }
        /// <summary>
        /// Allows creating an admin account while still reusing the base account setup,
        /// keeping all account initialization consistent.
        /// </summary>
        public Administrator(int accountID, string username, string passwordHash)
            : base(accountID, username, passwordHash)
        {
        }
    }
}
