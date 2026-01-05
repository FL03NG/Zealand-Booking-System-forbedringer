using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// Responsibility:
    /// - Shows all rooms in the system.
    /// - Supports searching and filtering by room type.
    /// - Allows admins/teachers to create, edit, and delete rooms.
    ///
    /// Why this page exists:
    /// - To give users a clear overview of available rooms.
    /// - To manage rooms in one place instead of multiple pages.
    /// </summary>
    public class RoomModel : PageModel
    {
        /// <summary>
        /// Database connection string used by the repository.
        /// </summary>
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;";

        /// <summary>
        /// The room id currently being edited.
        /// Used by the UI to switch between view mode and edit mode.
        /// </summary>
        [BindProperty]
        public int EditRoomID { get; set; }

        /// <summary>
        /// The room currently being edited.
        /// The user edits this object in the form before saving.
        /// </summary>
        [BindProperty]
        public Room EditRoom { get; set; }

        /// <summary>
        /// List of rooms shown on the page.
        /// This list can be filtered before it is displayed.
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();

        /// <summary>
        /// Search text from the query string (GET).
        /// Used to filter rooms by name.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        /// <summary>
        /// Room type filter from the query string (GET).
        /// Used to filter rooms by room type.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string RoomTypeFilter { get; set; } = string.Empty;

        /// <summary>
        /// Service that contains room logic and calls the repository.
        /// Keeps database code out of the PageModel.
        /// </summary>
        private readonly RoomService _roomService;

        /// <summary>
        /// Data for creating a new room.
        /// Bound from the "create room" form.
        /// </summary>
        [BindProperty]
        public Room NewRoom { get; set; } = new Room();

        /// <summary>
        /// Creates the PageModel and sets up the service.
        /// </summary>
        public RoomModel()
        {
            RoomCollectionRepo repo = new RoomCollectionRepo(_connectionString);
            _roomService = new RoomService(repo);
        }

        /// <summary>
        /// Loads the room list for the page.
        /// Applies search and filter values if the user has selected any.
        /// </summary>
        public void OnGet()
        {
            List<Room> allRooms = _roomService.GetAllRooms();

            // Filter by search text (room name)
            if (!string.IsNullOrEmpty(SearchString))
            {
                allRooms = allRooms
                    .Where(r =>
                        !string.IsNullOrEmpty(r.RoomName) &&
                        r.RoomName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by room type
            if (!string.IsNullOrEmpty(RoomTypeFilter))
            {
                allRooms = allRooms
                    .Where(r => r.RoomType.ToString() == RoomTypeFilter)
                    .ToList();
            }

            Rooms = allRooms;
        }

        /// <summary>
        /// Creates a new room from the form input.
        /// Uses the service layer to validate and save the room.
        /// Redirects after submit to avoid duplicate form submissions.
        /// </summary>
        public IActionResult OnPost()
        {
            try
            {
                _roomService.AddRoom(NewRoom);
                Debug.WriteLine("Room created!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while creating room: " + ex.Message);
            }

            return RedirectToPage();
        }

        /// <summary>
        /// Deletes a room by id.
        /// Uses the service layer to ensure the delete is handled correctly.
        /// Redirects after submit to avoid duplicate form submissions.
        /// </summary>
        public IActionResult OnPostDelete(int roomID)
        {
            try
            {
                _roomService.DeleteRoom(roomID);
                TempData["Message"] = "Room deleted!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error while deleting: " + ex.Message;
            }

            return RedirectToPage();
        }

        /// <summary>
        /// Starts edit mode for a specific room.
        /// Loads the selected room into EditRoom so the UI can show it in input fields.
        /// Returns Page() so edit mode stays active on the same page.
        /// </summary>
        public IActionResult OnPostStartEdit(int roomID)
        {
            EditRoomID = roomID;
            EditRoom = _roomService.GetRoomById(roomID);

            // Reload list so the page can still show all rooms
            Rooms = _roomService.GetAllRooms();

            return Page();
        }

        /// <summary>
        /// Saves changes made to a room.
        /// Updates are passed through the service layer
        /// to enforce business rules consistently.
        /// </summary>
        public IActionResult OnPostSaveEdit()
        {
            try
            {
                _roomService.UpdateRoom(EditRoom);
                TempData["Message"] = "Room updated!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error while editing: " + ex.Message;
            }

            return RedirectToPage();
        }
    }
}
