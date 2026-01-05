using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// The type of room. Helps the system know how the room is meant to be used,
    /// which can affect booking rules and how it's shown in the UI.
    /// </summary>
    public enum RoomType
    {
        ClassRoom = 1,
        MeetingRoom = 2
    }
    /// <summary>
    /// Represents a room that can be booked. 
    /// This class keeps all basic room details in one place, so other parts of the system
    /// don’t have to store or guess this information.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Unique ID for the room, used to reliably reference it in the system.
        /// </summary>
        public int RoomID { get; set; }
        /// <summary>
        /// The room’s name, so users can easily identify it when booking.
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// Extra info about the room that helps users choose the right space.
        /// </summary>
        public string RoomDescription { get; set; }
        /// <summary>
        /// Where the room is located. 
        /// So people know where to go.
        /// </summary>
        public string RoomLocation { get; set; }
        /// <summary>
        /// The general category of the room. Helps the system manage rules and UI display.
        /// </summary>
        public RoomType RoomType { get; set; }
        /// <summary>
        /// Indicates whether the room has a SmartBoard.
        /// Useful when users need specific equipment.
        /// </summary>
        public bool HasSmartBoard { get; set; }
    

        public Room() { }
        /// <summary>
        /// Creates a room with the most relevant details already set,
        /// making it easier to create valid room entries.
        /// </summary>
        public Room(string roomName, string roomDescription, string roomLocation, RoomType roomType, bool hasSmartBoard)
        {
            RoomName = roomName;
            RoomDescription = roomDescription;
            RoomLocation = roomLocation;
            RoomType = roomType;
            HasSmartBoard = hasSmartBoard;
        }
    }
}
