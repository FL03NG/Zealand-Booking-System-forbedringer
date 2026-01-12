using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Shared
{
    /// <summary>
    /// PageModel responsible for displaying and managing the booking list.
    /// Supports:
    /// - Loading bookings with related room/user data
    /// - Creating a booking
    /// - Searching bookings by username
    /// - Deleting bookings (with role-based rules) and creating notifications
    /// - Editing bookings (start edit + save edit)
    ///
    /// The page keeps UI concerns here, while business rules (e.g. 3-day rule, double booking)
    /// are delegated to the service layer.
    /// </summary>
  
    public class BookingListModel : PageModel
    {
        

        /// <summary>
        /// Bookings shown on the page.
        /// </summary>
        public List<Booking> Bookings { get; private set; }

        /// <summary>
        /// Rooms used for display and dropdowns.
        /// </summary>
        public List<Room> Rooms { get; private set; }

        /// <summary>
        /// Users used for display (if needed).
        /// </summary>
        public List<Account> Users { get; private set; }

        /// <summary>
        /// New booking from the create form.
        /// </summary>
        [BindProperty]
        public Booking NewBooking { get; set; }

        /// <summary>
        /// Search text from the form.
        /// </summary>
        [BindProperty]
        public string SearchName { get; set; }

        /// <summary>
        /// Booking id currently in edit mode.
        /// </summary>
        [BindProperty]
        public int EditBookingID { get; set; }

        /// <summary>
        /// Booking being edited.
        /// </summary>
        [BindProperty]
        public Booking EditBooking { get; set; }

        // ----------------------------
        // Services
        // ----------------------------

        /// <summary>
        /// Handles booking rules and actions (create/update/delete/availability).
        /// </summary>
        private readonly BookingService _bookingService;

        /// <summary>
        /// Loads users and returns the correct type (Student/Teacher/Admin).
        /// </summary>
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly RoomService _roomService;
        /// <summary>
        /// Message shown after actions (create/edit/delete).
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Creates repositories and services for this page.
        /// </summary>
        public BookingListModel(BookingService bookingService, UserService userService, NotificationService notificationService, RoomService roomService)
        {
            _bookingService = bookingService;
            _userService = userService;
            _notificationService = notificationService;
            _roomService = roomService;
        }

        /// <summary>
        /// Loads data for the page.
        /// </summary>
        public void OnGet()
        {
            LoadData();
        }

        /// <summary>
        /// Creates a booking from the form.
        /// </summary>
        public void OnPost()
        {
            try
            {
                _bookingService.Add(NewBooking);
                Message = "Booking created!";
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }

            LoadData();
        }

        /// <summary>
        /// Searches bookings by username.
        /// </summary>
        public void OnPostSearch()
        {
            LoadData();

            if (string.IsNullOrWhiteSpace(SearchName))
            {
                return;
            }

            string searchLower = SearchName.ToLower();
            List<Booking> filtered = new List<Booking>();

            foreach (Booking booking in Bookings)
            {
                string username = booking.Account != null ? booking.Account.Username : null;

                if (!string.IsNullOrEmpty(username) &&
                    username.ToLower().Contains(searchLower))
                {
                    filtered.Add(booking);
                }
            }

            Bookings = filtered;
        }
        /// <summary>
        /// Sorts bookings by date (earliest first).
        /// </summary>
        public void OnPostSortByDate()
        {
            LoadData();

            Bookings = Bookings
                .OrderBy(b => b.BookingDate)
                .ToList();
        }

        /// <summary>
        /// Deletes a booking and creates a notification for the user.
        /// </summary>
        public IActionResult OnPostDelete(int bookingID)
        {
            try
            {
                string role = HttpContext.Session.GetString("Role");

                // Get booking first so we can use the data in the notification
                Booking booking = _bookingService.GetBookingById(bookingID);

                _bookingService.Delete(bookingID, role);



            _notificationService.Create(
                booking.AccountID,
                $"Your booking at {booking.BookingDate:dd-MM-yyyy} has been deleted."
            );

                Message = "Booking deleted!";
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }

            LoadData();
            return Page();
        }

        /// <summary>
        /// Starts edit mode for one booking.
        /// </summary>
        public IActionResult OnPostStartEdit(int bookingID)
        {
            Message = "Editing started";
            EditBookingID = bookingID;
            EditBooking = _bookingService.GetBookingById(bookingID);

            LoadData();
            return Page();
        }

        /// <summary>
        /// Saves the edited booking.
        /// </summary>
        public IActionResult OnPostSaveEdit()
        {
            try
            {
                string role = HttpContext.Session.GetString("Role");

                _bookingService.Update(EditBooking, role);
                Message = "Booking updated!";
            }
            catch (Exception ex)
            {
                Message = "Error during editing: " + ex.Message;
                LoadData();
                return Page();
            }

            LoadData();
            return Page();
        }

        /// <summary>
        /// Loads bookings and rooms, and attaches user/room objects to each booking.
        /// </summary>
        private void LoadData()
        {
            Bookings = _bookingService.GetAll();

            
            Rooms = _roomService.GetAllRooms();

            foreach (Booking booking in Bookings)
            {
                booking.Account = _userService.GetById(booking.AccountID);
                booking.Room = _roomService.GetRoomById(booking.RoomID);
            }
        }
    }
}
