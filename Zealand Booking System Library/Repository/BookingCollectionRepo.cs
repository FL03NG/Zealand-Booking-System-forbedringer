using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Repository responsible for reading and writing booking data to the database.
    /// This class encapsulates all SQL logic for bookings, so the rest of the system
    /// does not depend on SQL queries or database schema details.
    ///
    /// Why this class exists:
    /// - To provide a single place to load, create, update and delete bookings.
    /// - To make it easier to change the database schema without touching UI or services.
    /// </summary>
    public class BookingCollectionRepo : IBookingRepository
    {
        /// <summary>
        /// Connection string is injected so this repository can be reused
        /// in different environments (development, test, production)
        /// without code changes.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Creates a new BookingCollectionRepo with the given connection string.
        /// </summary>
        public BookingCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all bookings from the database, including related
        /// account and room information.
        ///
        /// Responsibility:
        /// - Joins Booking, Account and Room tables.
        /// - Maps raw SQL data into strongly typed Booking objects.
        ///
        /// Why:
        /// - Keeps data access logic in one place.
        /// - Avoids scattered SQL joins throughout the codebase.
        /// </summary>
        public List<Booking> GetAll()
        {
            List<Booking> bookings = new List<Booking>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        b.BookingID,
                        b.BookingDescription,
                        b.BookingDate,
                        b.TimeSlot,
                        b.AccountID,
                        b.RoomID,
                        a.Username,
                        r.RoomName,
                        r.RoomDescription,
                        r.RoomLocation
                    FROM Booking b
                    INNER JOIN Account a ON b.AccountID = a.AccountID
                    INNER JOIN Room r ON b.RoomID = r.RoomID";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Booking booking = new Booking();
                    booking.BookingID = (int)reader["BookingID"];
                    booking.BookingDescription = reader["BookingDescription"] != DBNull.Value
                        ? reader["BookingDescription"].ToString()
                        : string.Empty;
                    booking.BookingDate = (DateTime)reader["BookingDate"];

                    int timeSlotValue = (int)reader["TimeSlot"];
                    booking.TimeSlot = (TimeSlot)timeSlotValue;

                    booking.AccountID = (int)reader["AccountID"];
                    booking.RoomID = (int)reader["RoomID"];

                    // Build related Account object so higher layers
                    // can access username without doing extra queries.
                    Account account = new Account();
                    account.AccountID = booking.AccountID;
                    account.Username = reader["Username"].ToString();
                    booking.Account = account;

                    // Build related Room object so UI can show room details.
                    Room room = new Room();
                    room.RoomID = booking.RoomID;
                    room.RoomName = reader["RoomName"].ToString();
                    //room.Size = reader["Size"].ToString();
                    room.RoomDescription = reader["RoomDescription"].ToString();
                    room.RoomLocation = reader["RoomLocation"].ToString();
                    booking.Room = room;

                    // Important: add the booking to the list before reading the next row.
                    bookings.Add(booking);
                }
            }

            return bookings;
        }

        /// <summary>
        /// Inserts a new booking into the database.
        ///
        /// Responsibility:
        /// - Writes a new row in the Booking table based on the Booking model.
        ///
        /// Why:
        /// - Centralizes write logic so SQL and column names are not spread across the code.
        /// </summary>
        public void Add(Booking booking)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO Booking (BookingDescription, RoomID, AccountID, BookingDate, TimeSlot) " +
                    "VALUES (@BookingDescription, @RoomID, @AccountID, @BookingDate, @TimeSlot)",
                    connection);

                command.Parameters.AddWithValue("@BookingDescription",
                    string.IsNullOrEmpty(booking.BookingDescription) ? string.Empty : booking.BookingDescription);
                command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                command.Parameters.AddWithValue("@AccountID", booking.AccountID);
                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@TimeSlot", (int)booking.TimeSlot);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates an existing booking in the database.
        ///
        /// Responsibility:
        /// - Changes date, account and timeslot for a given booking.
        ///
        /// Why:
        /// - Keeps update logic in one place, so rules and schema changes
        ///   are easier to maintain.
        /// </summary>
        public void Update(Booking booking)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "UPDATE Booking " +
                    "SET BookingDate = @BookingDate, " +
                    "    AccountID   = @AccountID, " +
                    "    TimeSlot    = @TimeSlot " +
                    "WHERE BookingID = @BookingID",
                    connection);

                command.Parameters.AddWithValue("@BookingID", booking.BookingID);
                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@AccountID", booking.AccountID);
                command.Parameters.AddWithValue("@TimeSlot", (int)booking.TimeSlot);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a booking from the database by ID.
        ///
        /// Responsibility:
        /// - Removes a single booking row.
        ///
        /// Why:
        /// - Prevents unused or old bookings from cluttering the system.
        /// - Keeps delete logic consistent and in one location.
        /// </summary>
        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Booking WHERE BookingID = @BookingID", connection);
                command.Parameters.AddWithValue("@BookingID", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves a single booking by its ID, including the related
        /// account and room data.
        ///
        /// Responsibility:
        /// - Loads all needed data for a detailed booking view or edit.
        ///
        /// Why:
        /// - Allows pages and services to work with one object instead of
        ///   doing multiple database calls for user and room details.
        /// </summary>
        public Booking GetBookingById(int bookingID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        b.BookingID,
                        b.BookingDescription,
                        b.BookingDate,
                        b.TimeSlot,
                        b.AccountID,
                        b.RoomID,
                        a.Username,
                        r.RoomName,
                        r.RoomDescription,
                        r.RoomLocation
                    FROM Booking b
                    INNER JOIN Account a ON b.AccountID = a.AccountID
                    INNER JOIN Room r ON b.RoomID = r.RoomID
                    WHERE b.BookingID = @BookingID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingID", bookingID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Booking booking = new Booking
                            {
                                BookingID = (int)reader["BookingID"],
                                BookingDescription = reader["BookingDescription"] != DBNull.Value
                                    ? reader["BookingDescription"].ToString()
                                    : string.Empty,
                                BookingDate = (DateTime)reader["BookingDate"],
                                TimeSlot = (TimeSlot)(int)reader["TimeSlot"],
                                AccountID = (int)reader["AccountID"],
                                RoomID = (int)reader["RoomID"],
                                Account = new Account
                                {
                                    AccountID = (int)reader["AccountID"],
                                    Username = reader["Username"].ToString()
                                },
                                Room = new Room
                                {
                                    RoomID = (int)reader["RoomID"],
                                    RoomName = reader["RoomName"].ToString(),
                                    //Size = reader["Size"].ToString(),
                                    RoomDescription = reader["RoomDescription"].ToString(),
                                    RoomLocation = reader["RoomLocation"].ToString()
                                }
                            };

                            return booking;
                        }
                    }
                }
            }

            // Returning null allows the service layer to decide how to handle
            // a missing booking (e.g. show error message or redirect).
            return null;
        }
    }
}
