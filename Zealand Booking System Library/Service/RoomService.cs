using System;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Handles rules for working with rooms.
    ///
    /// What this class does:
    /// - Checks that room data is valid.
    /// - Controls when rooms can be created, updated, or deleted.
    ///
    /// Why this class exists:
    /// - To keep business rules out of the UI.
    /// - To make sure only valid rooms are saved in the database.
    /// </summary>
    public class RoomService
    {
        /// <summary>
        /// Repository used to read and write room data.
        /// </summary>
        private readonly IRoomRepository _roomRepo;

        /// <summary>
        /// Creates the service and receives the room repository.
        /// </summary>
        public RoomService(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        /// <summary>
        /// Returns one room by its ID.
        /// Used when viewing or editing a room.
        /// </summary>
        public Room GetRoomById(int roomID)
        {
            return _roomRepo.GetRoomById(roomID);
        }

        /// <summary>
        /// Creates a new room after checking the input.
        /// Prevents empty or invalid room data.
        /// </summary>
        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (string.IsNullOrWhiteSpace(room.RoomName))
                throw new ArgumentException("Room name is required.");

            if (string.IsNullOrWhiteSpace(room.RoomLocation))
                throw new ArgumentException("Room location is required.");

            if (!Enum.IsDefined(typeof(RoomType), room.RoomType))
                throw new ArgumentException("Invalid room type.");

            _roomRepo.AddRoom(room);
        }

        /// <summary>
        /// Deletes a room using its ID.
        /// Ensures the ID is valid before deleting.
        /// </summary>
        public void DeleteRoom(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid room ID.");

            _roomRepo.DeleteRoom(id);
        }

        /// <summary>
        /// Returns all rooms in the system.
        /// Used for lists and overviews.
        /// </summary>
        public List<Room> GetAllRooms()
        {
            return _roomRepo.GetAllRooms();
        }

        /// <summary>
        /// Updates an existing room.
        /// Checks all values before saving changes.
        /// </summary>
        public void UpdateRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (room.RoomID <= 0)
                throw new ArgumentException("Invalid room ID.");

            if (string.IsNullOrWhiteSpace(room.RoomName))
                throw new ArgumentException("Room name is required.");

            if (string.IsNullOrWhiteSpace(room.RoomLocation))
                throw new ArgumentException("Room location is required.");

            if (!Enum.IsDefined(typeof(RoomType), room.RoomType))
                throw new ArgumentException("Invalid room type.");

            _roomRepo.UpdateRoom(room);
        }
    }
}
