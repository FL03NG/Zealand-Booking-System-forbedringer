using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Repository: RoomCollectionRepo
    ///
    /// What it does:
    /// - Talks to the SQL database for rooms.
    /// - Can load, create, update, and delete rooms.
    ///
    /// Why it exists:
    /// - Keeps all SQL code in one place.
    /// - So services and UI do not need to know SQL or table structure.
    /// </summary>
    public class RoomCollectionRepo : IRoomRepository
    {
        /// <summary>
        /// Connection string used to connect to the database.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Creates the repository with a connection string.
        /// This makes it easy to reuse with another database later.
        /// </summary>
        public RoomCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets all rooms from the database.
        /// This is used when we need an overview of rooms.
        /// </summary>
        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT RoomID, RoomName, RoomDescription, RoomLocation, RoomType, HasSmartBoard FROM Room";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a Room object from the SQL row
                            Room room = new Room();
                            room.RoomID = Convert.ToInt32(reader["RoomID"]);
                            room.RoomName = reader["RoomName"].ToString();

                            room.RoomDescription = reader["RoomDescription"] != DBNull.Value
                                ? reader["RoomDescription"].ToString()
                                : string.Empty;

                            room.RoomLocation = reader["RoomLocation"].ToString();

                            if (reader["RoomType"] != DBNull.Value)
                            {
                                int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                                room.RoomType = (RoomType)roomTypeValue;
                            }

                            if (reader["HasSmartBoard"] != DBNull.Value)
                            {
                                room.HasSmartBoard = Convert.ToBoolean(reader["HasSmartBoard"]);
                            }

                            rooms.Add(room);
                        }
                    }
                }
            }

            return rooms;
        }

        /// <summary>
        /// Gets one room by id.
        /// Used when editing a room or showing details.
        /// Returns null if the room does not exist.
        /// </summary>
        public Room GetRoomById(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    "SELECT RoomID, RoomName, RoomLocation, RoomDescription, RoomType, HasSmartBoard " +
                    "FROM Room WHERE RoomID = @RoomID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", roomID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Room room = new Room();
                            room.RoomID = Convert.ToInt32(reader["RoomID"]);
                            room.RoomName = reader["RoomName"].ToString();
                            room.RoomLocation = reader["RoomLocation"].ToString();

                            room.RoomDescription = reader["RoomDescription"] != DBNull.Value
                                ? reader["RoomDescription"].ToString()
                                : string.Empty;

                            if (reader["RoomType"] != DBNull.Value)
                            {
                                int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                                room.RoomType = (RoomType)roomTypeValue;
                            }

                            if (reader["HasSmartBoard"] != DBNull.Value)
                            {
                                room.HasSmartBoard = Convert.ToBoolean(reader["HasSmartBoard"]);
                            }

                            return room;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a new room to the database.
        /// Used when an admin/teacher creates a room.
        /// </summary>
        public void AddRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    "INSERT INTO Room (RoomName, RoomDescription, RoomLocation, RoomType, HasSmartBoard) " +
                    "VALUES (@RoomName, @RoomDescription, @RoomLocation, @RoomType, @HasSmartBoard)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomName", room.RoomName);

                    cmd.Parameters.AddWithValue("@RoomDescription",
                        string.IsNullOrEmpty(room.RoomDescription) ? string.Empty : room.RoomDescription);

                    cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);
                    cmd.Parameters.AddWithValue("@RoomType", (int)room.RoomType);
                    cmd.Parameters.AddWithValue("@HasSmartBoard", room.HasSmartBoard);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates a room in the database.
        /// Used when an admin/teacher edits a room.
        /// </summary>
        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    @"UPDATE Room 
                      SET RoomName = @RoomName,
                          RoomDescription = @RoomDescription,
                          RoomLocation = @RoomLocation,
                          RoomType = @RoomType,
                          HasSmartBoard = @HasSmartBoard
                      WHERE RoomID = @RoomID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                    cmd.Parameters.AddWithValue("@RoomName", room.RoomName);

                    cmd.Parameters.AddWithValue("@RoomDescription",
                        string.IsNullOrEmpty(room.RoomDescription) ? string.Empty : room.RoomDescription);

                    cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);
                    cmd.Parameters.AddWithValue("@RoomType", (int)room.RoomType);
                    cmd.Parameters.AddWithValue("@HasSmartBoard", room.HasSmartBoard);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes a room from the database.
        /// Also deletes bookings for that room first (to avoid conflicts).
        /// </summary>
        public void DeleteRoom(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Delete bookings first so the room can be deleted safely
                string deleteBookingsSql = "DELETE FROM Booking WHERE RoomID = @RoomID";
                using (SqlCommand cmdBookings = new SqlCommand(deleteBookingsSql, conn))
                {
                    cmdBookings.Parameters.AddWithValue("@RoomID", roomID);
                    cmdBookings.ExecuteNonQuery();
                }

                // Delete the room
                string deleteRoomSql = "DELETE FROM Room WHERE RoomID = @RoomID";
                using (SqlCommand cmdRoom = new SqlCommand(deleteRoomSql, conn))
                {
                    cmdRoom.Parameters.AddWithValue("@RoomID", roomID);
                    cmdRoom.ExecuteNonQuery();
                }
            }
        }
    }
}
