using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Handles database access for users.
    ///
    /// Responsibility:
    /// - Load, create, update and delete users.
    /// - Convert database rows into Account objects.
    ///
    /// Why this class exists:
    /// - To keep all SQL code in one place.
    /// - To keep UI and services independent of the database.
    /// </summary>
    public class UserCollectionRepo : IUserRepository
    {
        /// <summary>
        /// Connection string used to connect to the database.
        /// Stored once so it is easy to change.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Creates the repository with a database connection string.
        /// </summary>
        public UserCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets all users from the database.
        /// Used for admin user lists.
        /// </summary>
        public List<Account> GetAllUsers()
        {
            List<Account> users = new List<Account>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT AccountID, Username, PasswordHash, AccountRole FROM Account";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(MapToAccount(reader));
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Gets a user by ID.
        /// Used when editing or displaying a specific user.
        /// </summary>
        public Account GetById(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account WHERE AccountID = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return MapToAccount(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a user by username.
        /// Used during login.
        /// </summary>
        public Account GetByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account WHERE Username = @username";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return MapToAccount(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new user in the database.
        /// Used when registering users.
        /// </summary>
        public void CreateUser(Account user, string role)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "INSERT INTO Account (Username, PasswordHash, AccountRole) " +
                    "VALUES (@u, @p, @r)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@p", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@r", role);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates username and role for a user.
        /// Does not update password.
        /// </summary>
        public void UpdateUser(Account user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "UPDATE Account " +
                    "SET Username = @Username, AccountRole = @AccountRole " +
                    "WHERE AccountID = @AccountID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@AccountRole", user.Role);
                    cmd.Parameters.AddWithValue("@AccountID", user.AccountID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes a user and all related data.
        /// This prevents orphaned records.
        /// </summary>
        public void DeleteUser(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                new SqlCommand("DELETE FROM Notifications WHERE AccountID = @id", conn)
                {
                    Parameters = { new SqlParameter("@id", accountId) }
                }.ExecuteNonQuery();

                new SqlCommand("DELETE FROM Booking WHERE AccountID = @id", conn)
                {
                    Parameters = { new SqlParameter("@id", accountId) }
                }.ExecuteNonQuery();

                new SqlCommand("DELETE FROM Administrator WHERE AdministratorID = @id", conn)
                {
                    Parameters = { new SqlParameter("@id", accountId) }
                }.ExecuteNonQuery();

                new SqlCommand("DELETE FROM Teacher WHERE TeacherID = @id", conn)
                {
                    Parameters = { new SqlParameter("@id", accountId) }
                }.ExecuteNonQuery();

                new SqlCommand("DELETE FROM Student WHERE StudentID = @id", conn)
                {
                    Parameters = { new SqlParameter("@id", accountId) }
                }.ExecuteNonQuery();

                new SqlCommand("DELETE FROM Account WHERE AccountID = @id", conn)
                {
                    Parameters = { new SqlParameter("@id", accountId) }
                }.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Converts database data into the correct Account type.
        /// Ensures correct role (Admin / Teacher / Student).
        /// </summary>
        private Account MapToAccount(SqlDataReader reader)
        {
            string role = reader.GetString(reader.GetOrdinal("AccountRole"));

            Account account = role switch
            {
                "Administrator" => new Administrator(),
                "Teacher" => new Teacher(),
                "Student" => new Student(),
                _ => new Account()
            };

            account.AccountID = reader.GetInt32(reader.GetOrdinal("AccountID"));
            account.Username = reader.GetString(reader.GetOrdinal("Username"));
            account.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
            account.Role = role;

            return account;
        }
    }
}
