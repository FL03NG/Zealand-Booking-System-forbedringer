using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Zealand_Booking_System_Library.Models
{
    /// <summary>
    /// Holds information about how available a room is right now.
    /// This keeps all availability logic in one place, so the UI can easily
    /// decide how the room should look and what status to show.
    /// </summary>
    public class RoomAvailability
    {
        /// <summary>
        /// The room these availability details belong to.
        /// Keeping it here avoids having to fetch the room again elsewhere.
        /// </summary>
        public Room Room { get; set; }
        /// <summary>
        /// How many bookings the room currently has, it is used to figure out its status.
        /// </summary>
        public int CurrentBookings { get; set; }
        /// <summary>
        /// The maximum number of bookings the room can handle.
        /// This helps determine how “full” the room is.
        /// </summary>
        public int MaxBookings { get; set; }
        /// <summary>
        /// A color hint for the UI to show the room’s availability at a glance.
        /// Keeping this here keeps the logic consistent everywhere.
        /// </summary>
        public string StatusColor { get; set; }
        /// <summary>
        /// A short text description of the room’s availability,
        /// so users don’t need to interpret numbers themselves.
        /// </summary>
        public string StatusText { get; set; } 

        public RoomAvailability()
        {
        }
    }
}

