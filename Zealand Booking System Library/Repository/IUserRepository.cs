using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Defines the contract for user persistence.
    /// By using an interface, the system can switch between
    /// different data sources (e.g. database, mock data, in-memory)
    /// without affecting business logic.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a single user by ID.
        /// This allows higher layers to work with users
        /// without knowing how or where they are stored.
        /// </summary>
        Account GetById(int accountId);
        /// <summary>
        /// Retrieves a user by username.
        /// Primarily used for authentication, where usernames
        /// act as a unique identifier.
        /// </summary>
        Account GetByUsername(string username);
        /// <summary>
        /// Persists a new user.
        /// Role is passed explicitly to keep role handling flexible
        /// and avoid hard coupling to specific account subclasses.
        /// </summary>
        void CreateUser(Account user, string role);
        /// <summary>
        /// Updates existing user information.
        /// The concrete repository decides how changes are stored,
        /// allowing the rest of the system to remain unchanged.
        /// </summary>
        void UpdateUser(Account user);
        /// <summary>
        /// Removes a user and all related data.
        /// The implementation is responsible for maintaining
        /// data integrity and handling dependencies.
        /// </summary>
        void DeleteUser(int accountId);
        /// <summary>
        /// Retrieves all users.
        /// This supports administration features without
        /// exposing persistence details.
        /// </summary>
        List<Account> GetAllUsers();

    }
}
