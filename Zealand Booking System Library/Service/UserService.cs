using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Handles all user-related logic.
    ///
    /// Responsibility:
    /// - Logs users in.
    /// - Creates, updates and deletes users.
    /// - Makes sure passwords are handled securely.
    ///
    /// Why this class exists:
    /// - To keep security and user rules out of the UI.
    /// - To avoid calling repositories directly from Razor Pages.
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Repository used to access user data in the database.
        /// </summary>
        private readonly IUserRepository _repo;

        /// <summary>
        /// Creates the service and receives the user repository.
        /// </summary>
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Checks username and password and logs the user in.
        /// Returns the user if credentials are correct, otherwise null.
        /// </summary>
        public Account Login(string username, string password)
        {
            Account user = _repo.GetByUsername(username);

            if (user == null)
            {
                return null;
            }

            // Password is verified here so only valid users are returned
            if (PasswordHasher.Verify(user.PasswordHash, password))
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// Creates a new user.
        /// The password is hashed before saving to the database.
        /// </summary>
        public void Create(Account user, string role)
        {
            user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            _repo.CreateUser(user, role);
        }

        /// <summary>
        /// Updates basic user information.
        /// </summary>
        public void UpdateUser(Account user)
        {
            _repo.UpdateUser(user);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        public void DeleteUser(int id)
        {
            _repo.DeleteUser(id);
        }

        /// <summary>
        /// Returns a single user by ID.
        /// </summary>
        public Account GetById(int id)
        {
            return _repo.GetById(id);
        }

        /// <summary>
        /// Returns all users in the system.
        /// </summary>
        public List<Account> GetAllUsers()
        {
            return _repo.GetAllUsers();
        }
    }
}
