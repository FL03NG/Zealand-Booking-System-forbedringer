using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Represents a teacher account.
    /// This class lets the system apply teacher-specific permissions or behavior
    /// without mixing it into the general account logic.
    /// </summary>
    public class Teacher : Account
    {
        /// <summary>
        /// A separate ID for teachers, useful when linking accounts to teacher records
        /// or external scheduling systems.
        /// </summary>
        public int TeacherID { get; set; }
        /// <summary>
        /// Overrides the base role so the system immediately knows this user 
        /// should have teacher-level permissions.
        /// </summary>
        public override string Role { get; set; } = "Teacher";

        public Teacher() : base() { }
        /// <summary>
        /// Creates a teacher account while still relying on the shared account setup,
        /// keeping consistency across all account types.
        /// </summary>
        public Teacher(int accountID, string username, string passwordHash)
            : base(accountID, username, passwordHash)
        {
        }
    }
}
