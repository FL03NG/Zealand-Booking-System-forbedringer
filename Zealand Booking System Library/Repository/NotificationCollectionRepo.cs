using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Repository responsible for storing and retrieving user notifications.
    ///
    /// Responsibility:
    /// - Handles database access for notification creation, reading unread notifications,
    ///   and marking notifications as read.
    ///
    /// Why this class exists:
    /// - To keep SQL logic in one place.
    /// - To allow services and Razor Pages to work only with strongly typed models.
    /// - To support swapping database implementations without changing business logic.
    /// </summary>
    public class NotificationCollectionRepo : INotificationRepository
    {
        /// <summary>
        /// Connection string injected so the repository can work in different environments
        /// (development, testing, production) without code changes.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new Notification repository using the given connection string.
        /// </summary>
        public NotificationCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Saves a new notification in the database.
        ///
        /// Responsibility:
        /// - Inserts a row into the Notifications table.
        ///
        /// Why:
        /// - Allows the system to alert users about booking changes, warnings,
        ///   reminders, etc.
        /// </summary>
        public void Add(Notification notification)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"INSERT INTO Notifications (AccountID, Message) 
                           VALUES (@AccountID, @Message)";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AccountID", notification.AccountID);
            cmd.Parameters.AddWithValue("@Message", notification.Message);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves all unread notifications for a specific user.
        ///
        /// Responsibility:
        /// - Selects notifications where IsRead = false.
        ///
        /// Why:
        /// - Enables UI elements such as notification badges
        ///   and ensures users do not miss important updates.
        /// </summary>
        public List<Notification> GetUnread(int accountId)
        {
            List<Notification> list = new();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"SELECT * FROM Notifications
                           WHERE AccountID = @AccountID AND IsRead = 0";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AccountID", accountId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Notification
                {
                    NotificationID = reader.GetInt32(0),
                    AccountID = reader.GetInt32(1),
                    Message = reader.GetString(2),
                    IsRead = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }

            return list;
        }

        /// <summary>
        /// Marks a notification as read.
        ///
        /// Responsibility:
        /// - Updates a notification's IsRead column to true.
        ///
        /// Why:
        /// - Prevents the notification from appearing again in "unread" lists.
        /// - Keeps user notification centers clean and accurate.
        /// </summary>
        public void MarkAsRead(int notificationId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"UPDATE Notifications SET IsRead = 1 
                           WHERE NotificationID = @ID";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", notificationId);

            cmd.ExecuteNonQuery();
        }
    }
}
