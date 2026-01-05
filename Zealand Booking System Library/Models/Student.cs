using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Represents a student account. 
    /// Having a dedicated class makes it easy for the system to apply 
    /// student-specific rules and permissions.
    /// </summary>
    public class Student : Account
    {
        /// <summary>
        /// Extra ID used specifically for students, in case the system needs 
        /// to connect accounts to student records or external data.
        /// </summary>
        public int StudentID { get; set; }
        /// <summary>
        /// Overrides the base role so the system instantly knows this user
        /// should get student-level permissions.
        /// </summary>
        public override string Role { get; set; } = "Student";

        public Student() : base() { }
        /// <summary>
        /// Allows creating a student account while reusing the base account setup,
        /// keeping all account initialization consistent across account types.
        /// </summary>
        public Student(int accountID, string username, string passwordHash)
            : base(accountID, username, passwordHash)
        {
        }
    }
}
