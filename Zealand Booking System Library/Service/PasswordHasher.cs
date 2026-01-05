using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Handles password security.
    ///
    /// What this class does:
    /// - Hashes passwords before they are saved.
    /// - Checks passwords during login.
    ///
    /// Why this class exists:
    /// - To keep all password logic in one place.
    /// - To avoid using plain-text passwords.
    /// - To make it easy to change hashing method later.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes a plain password.
        /// The result is safe to store in the database.
        /// </summary>
        public static string Hash(string password)
        {
            return Argon2.Hash(password);
        }

        /// <summary>
        /// Checks if a password matches a stored hash.
        /// Used during login.
        /// </summary>
        public static bool Verify(string hash, string password)
        {
            return Argon2.Verify(hash, password);
        }
    }
}

