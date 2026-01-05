using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Defines the contract for room data persistence.
    /// By using an interface, the system can switch data sources
    /// (e.g. SQL database, in-memory collection, or mock repository)
    /// without changing the business logic.
    /// </summary>
    public interface IRoomRepository
    {
        /// <summary>
        /// Persists a new room.
        /// Separating this contract from the implementation keeps
        /// room creation consistent across different storage strategies.
        /// </summary>
        public void AddRoom(Room room);
        /// <summary>
        /// Retrieves a single room by its identifier.
        /// Returning a room (or null) allows the service layer
        /// to decide how missing data should be handled.
        /// </summary>
        public Room GetRoomById(int roomID);
        /// <summary>
        /// Removes a room from storage.
        /// This abstraction allows implementations to handle
        /// related data (such as bookings) in a safe, consistent way.
        /// </summary>
        public void DeleteRoom(int id);
        /// <summary>
        /// Retrieves all rooms from storage.
        /// Keeping this operation in the repository prevents
        /// higher layers from depending on storage-specific logic.
        /// </summary>
        public List<Room> GetAllRooms();
        /// <summary>
        /// Updates an existing room.
        /// This ensures all modifications go through a single,
        /// well-defined persistence contract.
        /// </summary>
        public void UpdateRoom(Room room);

    }
}

